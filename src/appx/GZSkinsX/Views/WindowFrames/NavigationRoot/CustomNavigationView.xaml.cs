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
using GZSkinsX.Api.Diagnostics;
using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.Navigation;
using GZSkinsX.Api.Themes;
using GZSkinsX.Api.WindowManager;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Views.WindowFrames.NavigationRoot;

public sealed partial class CustomNavigationView : MUXC.NavigationView
{
    private INavigationViewManager? _navigationViewManager;
    private readonly IAppxTitleBar _appxTitleBar;
    private readonly IThemeService _themeService;

    internal CustomNavigationView()
    {
        _appxTitleBar = AppxContext.AppxTitleBar;
        _themeService = AppxContext.ThemeService;

        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    internal void Setup(INavigationViewManager navigationViewManager)
    {
        _navigationViewManager = navigationViewManager;
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
                if (child is FrameworkElement { Name: TopNavTitleBar })
                {
                    return true;
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

        NavigationRoot_MainAppx_Title.Text = ResourceHelper.GetLocalized("Resources/AppName");
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
            foreach (var item in MenuItems.OfType<MUXC.NavigationViewItem>())
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

            if (suggestions.Count == 0)
            {
                suggestions.Add(QueryNavigationItem.Empty);
            }

            sender.ItemsSource = suggestions;
        }
    }

    private void OnMainSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is QueryNavigationItem item && item != QueryNavigationItem.Empty)
        {
            Debug2.Assert(_navigationViewManager is not null);
            _navigationViewManager.NavigateTo(item.Guid);
        }
    }

    private void OnSettingsInvoke(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);
    }

    private void OnControlFInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        NavigationRoot_MainSearchBox.Focus(FocusState.Keyboard);
    }

    private void OnThemeMenuFlyoutOpening(object sender, object e)
    {
        switch (_themeService.CurrentTheme)
        {
            case ElementTheme.Light:
                NavigationRoot_ThemeMenu_LightTheme_MenuItem.IsChecked = true;
                break;
            case ElementTheme.Dark:
                NavigationRoot_ThemeMenu_DarkTheme_MenuItem.IsChecked = true;
                break;
            default:
                NavigationRoot_ThemeMenu_DefaultTheme_MenuItem.IsChecked = true;
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

    public static QueryNavigationItem Empty = new(
        ResourceHelper.GetLocalized("Resources/NavigationRoot_MainSearchBox_Query_NotResultsFound"),
        string.Empty, FontFamily.XamlAutoFontFamily, Guid.Empty);
}
