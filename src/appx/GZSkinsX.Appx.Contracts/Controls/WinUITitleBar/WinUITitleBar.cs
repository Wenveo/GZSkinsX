// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Contracts.Controls;

public partial class WinUITitleBar : Grid
{
    public static readonly DependencyProperty TargetWindowProperty =
        DependencyProperty.Register(nameof(TargetWindow), typeof(Window),
            typeof(WinUITitleBar), new PropertyMetadata(null, OnTargetWindowChangedCallback));

    public Window TargetWindow
    {
        get => (Window)GetValue(TargetWindowProperty);
        set => SetValue(TargetWindowProperty, value);
    }

    public WinUITitleBar()
    {
        Loaded += (s, e) => UpdateDragRegionForCustomTitleBar();
        SizeChanged += (s, e) => UpdateDragRegionForCustomTitleBar();
    }

    private void UpdateDragRegionForCustomTitleBar()
    {
        if (IsLoaded is false || XamlRoot is null)
        {
            return;
        }

        if (TargetWindow is not { } targetWindow)
        {
            return;
        }

        var scaleAdjustment = XamlRoot.RasterizationScale;
        var dragRectsList = new List<Windows.Graphics.RectInt32>();

        int x = 0, height = (int)(ActualHeight * scaleAdjustment);
        for (var i = 0; i < ColumnDefinitions.Count; i++)
        {
            var column = ColumnDefinitions[i];
            var physicalWidth = (int)(column.ActualWidth * scaleAdjustment);

            if (GetUIContentType(column) is WinUITitleBarUIContentType.Caption)
            {
                dragRectsList.Add(new() { X = x, Height = height, Width = physicalWidth });
            }

            x += physicalWidth;
        }

        var dragRects = dragRectsList.ToArray();
        targetWindow.AppWindow.TitleBar.SetDragRectangles(dragRects);
    }

    private static void OnTargetWindowChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var titleBar = d as WinUITitleBar;
        Debug.Assert(titleBar is not null);
        if (titleBar is null)
        {
            return;
        }

        titleBar.UpdateDragRegionForCustomTitleBar();
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
