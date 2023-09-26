// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Contracts.Controls;

public static class WinUITitleBar
{
    public static readonly DependencyProperty IsWindowTitleBarProperty =
        DependencyProperty.RegisterAttached("IsWindowTitleBar", typeof(bool),
            typeof(WinUITitleBar), new PropertyMetadata(false, OnIsWindowTitleBarChangedCallback));

    public static bool GetIsWindowTitleBar(Grid obj)
    {
        return (bool)obj.GetValue(IsWindowTitleBarProperty);
    }

    public static void SetIsWindowTitleBar(Grid obj, bool value)
    {
        obj.SetValue(IsWindowTitleBarProperty, value);
    }

    private static void OnIsWindowTitleBarChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Grid titleBar)
        {
            void OnTitleBarLayoutUpdated(object? sender, object e)
            {
                // sender is null in here!?
                UpdateDragRegionForCustomTitleBar(titleBar);
            }

            titleBar.LayoutUpdated -= OnTitleBarLayoutUpdated;
            if (e.NewValue is bool b && b)
            {
                titleBar.LayoutUpdated += OnTitleBarLayoutUpdated;
            }
        }
    }

    private static void UpdateDragRegionForCustomTitleBar(Grid titleBar)
    {
        if (titleBar.IsLoaded is false || titleBar.XamlRoot is not { } xamlRoot)
        {
            return;
        }

        var scaleAdjustment = xamlRoot.RasterizationScale;
        var dragRectsList = new List<Windows.Graphics.RectInt32>();

        int x = 0, height = (int)(Math.Round(titleBar.ActualHeight * scaleAdjustment));
        for (var i = 0; i < titleBar.ColumnDefinitions.Count; i++)
        {
            var column = titleBar.ColumnDefinitions[i];
            var physicalWidth = (int)(Math.Round(column.ActualWidth * scaleAdjustment));

            if (GetUIContentType(column) is WinUITitleBarUIContentType.Caption)
            {
                dragRectsList.Add(new() { X = x, Height = height, Width = physicalWidth });
            }

            x += physicalWidth;
        }

        var dragRects = dragRectsList.ToArray();
        var appWindowId = xamlRoot.ContentIslandEnvironment.AppWindowId;
        AppWindow.GetFromWindowId(appWindowId).TitleBar.SetDragRectangles(dragRects);
    }

    public static readonly DependencyProperty UIContentTypeProperty =
        DependencyProperty.RegisterAttached("UIContentType", typeof(WinUITitleBarUIContentType),
            typeof(WinUITitleBar), new PropertyMetadata(WinUITitleBarUIContentType.Caption));

    public static WinUITitleBarUIContentType GetUIContentType(ColumnDefinition obj)
    {
        return (WinUITitleBarUIContentType)obj.GetValue(UIContentTypeProperty);
    }

    public static void SetUIContentType(ColumnDefinition obj, WinUITitleBarUIContentType value)
    {
        obj.SetValue(UIContentTypeProperty, value);
    }
}
