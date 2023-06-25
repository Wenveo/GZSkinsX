// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.Navigation;
using GZSkinsX.Api.Themes;
using GZSkinsX.Api.WindowManager;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace GZSkinsX.Views.WindowFrames.NavigationRoot;

public sealed partial class CustomNavigationView : NavigationView
{
    private readonly IThemeService _themeService;

    public INavigationViewManager NavigationViewManager { get; }

    public CustomNavigationView()
    {
        _themeService = AppxContext.ThemeService;

        NavigationViewManager = AppxContext.NavigationViewManagerFactory
            .CreateNavigationViewManager(NavigationConstants.NAVIGATIONROOT_NV_GUID, this);

        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var topNavArea = (StackPanel)GetTemplateChild("TopNavArea");
        var topRootGrid = (Grid)topNavArea.Parent;

        var navTitleBar = topRootGrid.Children.FirstOrDefault(child =>
        {
            // 查找是否已经在模板中注入了自定义标题栏
            return child is FrameworkElement { Name: "TopNavTitleBar" };
        });

        if (navTitleBar is null)
        {
            // 设置背景色为空，不然会挡住自定义的标题栏
            topNavArea.Background = null;
            navTitleBar = new CustomTitleBar
            {
                Name = "TopNavTitleBar",
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

            /// TODO: set drag region for custom titlebar
            /// navTitleBar.PointerEntered += OnPointerEntered;
        }

        // 清楚内容区域的背景色以及边框
        var contentGrid = (Grid)GetTemplateChild("ContentGrid");
        contentGrid.Background = null;
        contentGrid.BorderBrush = null;
        contentGrid.BorderThickness = default;

        NavigationRoot_MainAppx_Title.Text = ResourceHelper.GetLocalized("Resources/AppName");
    }

    private void OnMainSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.SuggestionChosen)
        {
            var suggestions = new List<QueryNavigationItem>();

            var querySplit = sender.Text.Split(" ");
            foreach (var item in MenuItems.OfType<NavigationViewItem>())
            {
                if (item.SelectsOnInvoked)
                {
                    foreach (var queryToken in querySplit)
                    {
                        var header = item.Content as string;
                        Debug.Assert(header is not null);
                        Debug.Assert(header.Length > 0);

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
            NavigationViewManager.NavigateTo(item.Guid);
        }
    }

    private void OnSettingsInvoke(XamlUICommand sender, ExecuteRequestedEventArgs args)
        => AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);

    private void OnControlFInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        => NavigationRoot_MainSearchBox.Focus(FocusState.Keyboard);

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
            _ = await _themeService.SetElementThemeAsync(newTheme);
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
