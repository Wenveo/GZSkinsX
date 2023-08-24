// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;

using GZSkinsX.Contracts.Appx;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace GZSkinsX.Contracts.Themes;

public static class ThemedFlyoutHelper
{
    public static readonly DependencyProperty FixFlyoutThemeProperty =
        DependencyProperty.RegisterAttached("FixFlyoutTheme", typeof(bool),
            typeof(ThemedFlyoutHelper), new PropertyMetadata(null, OnFixFlyoutThemeChangedCallback));

    private static void OnFixFlyoutThemeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var flyout = d as FlyoutBase;
        if (flyout is not null)
        {
            flyout.Opening -= OnFlyoutOpening;

            if (e.NewValue is bool b && b)
            {
                flyout.Opening += OnFlyoutOpening;
            }
        }
    }

    private static void OnMenuFlyoutOpening(object sender, object e)
    {
        static void SyncThemeCore(IEnumerable<MenuFlyoutItemBase> items, ElementTheme requestedTheme)
        {
            foreach (var item in items)
            {
                if (item is MenuFlyoutSubItem subItem)
                {
                    SyncThemeCore(subItem.Items, requestedTheme);
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
    }


    private static void OnFlyoutOpening(object sender, object e)
    {
        static void SyncThemeCore(IEnumerable<FrameworkElement> items, ElementTheme requestedTheme)
        {
            foreach (var item in items)
            {
                if (item is MenuFlyoutSubItem subItem)
                {
                    SyncThemeCore(subItem.Items, requestedTheme);
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
        else if (sender is Microsoft.UI.Xaml.Controls.MenuBarItemFlyout muxcMenuBarItemFlyout)
        {
            SyncThemeCore(muxcMenuBarItemFlyout.Items.OfType<FrameworkElement>(), actualTheme);
        }
        else if (sender is Microsoft.UI.Xaml.Controls.CommandBarFlyout muxcCommandBarFlyout)
        {
            SyncThemeCore(muxcCommandBarFlyout.PrimaryCommands.OfType<FrameworkElement>(), actualTheme);
            SyncThemeCore(muxcCommandBarFlyout.SecondaryCommands.OfType<FrameworkElement>(), actualTheme);
        }
    }

    public static bool GetFixFlyoutTheme(FlyoutBase obj)
    {
        return (bool)obj.GetValue(FixFlyoutThemeProperty);
    }

    public static void SetFixFlyoutTheme(FlyoutBase obj, bool value)
    {
        obj.SetValue(FixFlyoutThemeProperty, value);
    }
}
