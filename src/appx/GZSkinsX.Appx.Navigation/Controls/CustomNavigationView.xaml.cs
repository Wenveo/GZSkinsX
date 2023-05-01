// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Themes;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.DotNet.Diagnostics;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace GZSkinsX.Appx.Navigation.Controls;

public sealed partial class CustomNavigationView : Microsoft.UI.Xaml.Controls.NavigationView
{
    private readonly NavigationService _navigationService;

    private readonly IAppxTitleBar _appxTitleBar;
    private readonly IThemeService _themeService;

    internal CustomNavigationView(NavigationService navigationService)
    {
        _navigationService = navigationService;

        _appxTitleBar = AppxContext.AppxTitleBar;
        _themeService = AppxContext.ThemeService;

        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (GetTemplateChild("TopNavArea") is StackPanel topNavArea &&
            topNavArea.Parent is Grid topRootGrid)
        {
            const string TopNavTitleBar = "TopNavTitleBar";
            var navTitleBar = topRootGrid.Children.FirstOrDefault(child =>
            {
                // 查找是否已经在模板中注入了自定义标题栏
                if (child is FrameworkElement frameworkElement)
                {
                    return frameworkElement.Name == TopNavTitleBar;
                }

                return false;
            });

            if (navTitleBar is null)
            {
                // 设置背景色为空，不然会挡住自定义的标题栏
                topNavArea.Background = null;
                navTitleBar = new CustomTitleBar
                {
                    Name = TopNavTitleBar,
                    HorizontalAlignment = topNavArea.HorizontalAlignment,
                    VerticalAlignment = topNavArea.VerticalAlignment
                };

                // 更新 UI 元素在显示层中的 Z 轴顺序，以将 navTitleBar 置于底层
                //  Before                      After
                //  0 - ...                     0 - ...
                //  1 - topNavArea              1 - navTitleBar
                //  2 - navTitleBar             2 - topNavArea
                Canvas.SetZIndex(topNavArea, 2);
                Canvas.SetZIndex(navTitleBar, 1);

                topRootGrid.Children.Add(navTitleBar);
            }

            _appxTitleBar.SetTitleBar(navTitleBar);
        }
        else
        {
            // 当注入自定义标题栏失败时启用边距填充
            IsTitleBarAutoPaddingEnabled = true;
        }

        // 清楚内容区域的背景色以及边框
        if (GetTemplateChild("ContentGrid") is Grid contentGrid)
        {
            contentGrid.Background = null;
            contentGrid.BorderBrush = null;
            contentGrid.BorderThickness = default;
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _appxTitleBar.SetTitleBar(null);
    }

    private void OnMainSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.SuggestionChosen)
        {
            var suggestions = new List<QueryNavigationItem>();

            var querySplit = sender.Text.Split(" ");
            foreach (var item in _navigationService._createdNavItems.Values)
            {
                if (item.SelectsOnInvoked)
                {
                    foreach (var queryToken in querySplit)
                    {
                        var header = item.Content as string;
                        Debug2.Assert(header is not null);
                        Debug2.Assert(header.Length > 0);

                        if (header.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) is not -1)
                        {
                            if (item.Icon is FontIcon fontIcon)
                            {
                                suggestions.Add(new(header, fontIcon.Glyph,
                                    fontIcon.FontFamily, (Guid)item.Tag));
                            }
                            else
                            {
                                suggestions.Add(new(header, string.Empty,
                                item.FontFamily, (Guid)item.Tag));
                            }

                            break;
                        }
                    }
                }
            }

            sender.ItemsSource = suggestions.Count > 0 ? suggestions
                : (new string[] { _navigationService.GetLocalizedOrDefault("resx:GZSkinsX.Appx.Navigation/Resources/MainSearchBox_Query_NotResultsFound") });
        }
    }

    private async void OnMainSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is QueryNavigationItem queryNavigationItem)
        {
            await _navigationService.NavigateCoreAsync(queryNavigationItem.Guid, null, null);
        }
    }

    private void OnSettingsInvoke(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        AppxContext.ServiceLocator.Resolve<IWindowManagerService>().NavigateTo(WindowFrameConstants.Preload_Guid);
    }

    private void OnControlFInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        MainSearchBox.Focus(FocusState.Keyboard);
    }

    private void OnThemeMenuFlyoutOpening(object sender, object e)
    {
        switch (_themeService.CurrentTheme)
        {
            case ElementTheme.Light:
                MenuFlyout_LightTheme_MenuItem.IsChecked = true;
                break;
            case ElementTheme.Dark:
                MenuFlyout_DarkTheme_MenuItem.IsChecked = true;
                break;
            default:
                MenuFlyout_DefaultTheme_MenuItem.IsChecked = true;
                break;
        }
    }

    private async void OnSwitchTheme(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        if (args.Parameter is ElementTheme newTheme &&
            _themeService.CurrentTheme != newTheme)
        {
            await _themeService.SetElementThemeAsync(newTheme);
        }
    }
}

internal record class QueryNavigationItem(string Title, string Glyph, FontFamily FontFamily, Guid Guid)
{
    public override string ToString() => Title;
}
