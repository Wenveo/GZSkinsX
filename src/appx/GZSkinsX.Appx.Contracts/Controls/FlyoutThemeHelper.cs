// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;

using GZSkinsX.Contracts.Appx;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

namespace GZSkinsX.Contracts.Controls;

public static class FlyoutThemeHelper
{
    public static readonly DependencyProperty FixThemeSyncProperty =
        DependencyProperty.RegisterAttached("FixThemeSync", typeof(bool),
            typeof(FlyoutThemeHelper), new PropertyMetadata(false, OnFixThemeSyncChangedCallback));

    private static void OnFixThemeSyncChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var flyout = d as FlyoutBase;
        if (flyout is not null)
        {
            flyout.Opened -= OnFlyoutOpened;
            flyout.Opening -= OnFlyoutOpening;

            if (e.NewValue is bool b && b)
            {
                flyout.Opened += OnFlyoutOpened;
                flyout.Opening += OnFlyoutOpening;
            }
        }
    }

    private static void OnFlyoutOpened(object? sender, object e)
    {
        // Gets the actual theme of the application
        var actualTheme = AppxContext.ThemeService.ActualTheme;

        // Workaround for known issue with menu themes in WinAppSDK 1.4 (#8678, #8756)
        if (sender is FlyoutBase { XamlRoot: XamlRoot xamlRoot })
        {
            foreach (var popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(xamlRoot))
            {
                popup.RequestedTheme = actualTheme;
            }
        }
    }

    private static void OnFlyoutOpening(object? sender, object e)
    {
        static void SyncThemeCore(IEnumerable<FrameworkElement> items, ElementTheme requestedTheme)
        {
            foreach (var item in items)
            {
                if (item is MenuFlyoutSubItem subItem)
                {
                    SyncThemeCore(subItem.Items, requestedTheme);
                }

                var previousTheme = item.ActualTheme;
                if (previousTheme == requestedTheme)
                {
                    // 如果与先前的主题相同
                    // 那么先取反，切换一遍
                    // 避免相同的主题不生效
                    item.RequestedTheme =
                        requestedTheme is ElementTheme.Light ?
                        ElementTheme.Dark : ElementTheme.Light;
                }

                item.RequestedTheme = requestedTheme;
            }
        }

        // Gets the actual theme of the application
        var actualTheme = AppxContext.ThemeService.ActualTheme;

        // Fix the theme of sub items
        if (sender is MenuFlyout flyout)
        {
            SyncThemeCore(flyout.Items, actualTheme);
        }
        else if (sender is MenuBarItemFlyout muxcMenuBarItemFlyout)
        {
            SyncThemeCore(muxcMenuBarItemFlyout.Items, actualTheme);
        }
        else if (sender is CommandBarFlyout muxcCommandBarFlyout)
        {
            SyncThemeCore(muxcCommandBarFlyout.PrimaryCommands.OfType<FrameworkElement>(), actualTheme);
            SyncThemeCore(muxcCommandBarFlyout.SecondaryCommands.OfType<FrameworkElement>(), actualTheme);
        }
        // More...
    }

    public static bool GetFixThemeSync(FlyoutBase obj)
    {
        return (bool)obj.GetValue(FixThemeSyncProperty);
    }

    public static void SetFixThemeSync(FlyoutBase obj, bool value)
    {
        obj.SetValue(FixThemeSyncProperty, value);
    }
}
