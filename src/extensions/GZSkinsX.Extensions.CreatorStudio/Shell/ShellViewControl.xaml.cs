﻿// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Commands;
using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.AssetsExplorer;
using GZSkinsX.Api.Tabs;
using GZSkinsX.Extensions.CreatorStudio.AssetsExplorer;

using Microsoft.UI.Xaml.Controls;

using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Extensions.CreatorStudio.Shell;

public sealed partial class ShellViewControl : Grid
{
    private static readonly Lazy<ShellViewControl> s_lazy = new(() => new());

    public static ShellViewControl Instance => s_lazy.Value;

    private readonly AssetsExplorerService _assetsExplorerService;
    private readonly ICommandBarService _commandBarService;
    private readonly ShellViewSettings _shellViewSettings;
    private readonly ITabViewManager _tabViewManager;

    public bool AssetsExplorerIsVisible => AssetsExplorerHost.Visibility == Visibility.Visible;

    private ShellViewControl()
    {
        _assetsExplorerService = (AssetsExplorerService)AppxContext.Resolve<IAssetsExplorerService>();
        _commandBarService = AppxContext.Resolve<ICommandBarService>();
        _shellViewSettings = AppxContext.Resolve<ShellViewSettings>();

        var tabViewService = AppxContext.Resolve<ITabViewService>();
        _tabViewManager = tabViewService.CreateTabViewManager(
            new TabViewManagerOptions("B0783923-1256-4D56-8286-2F37BF108EE7"));

        ((TabView)_tabViewManager.UIObject).TabItemsChanged += OnTabItemsChanged;

        InitializeComponent();
        InitializeUIObject();
        UpdateTabVisible();
        UpdateUIState();
    }

    private void InitializeUIObject()
    {
        var commandBar = _commandBarService.CreateCommandBar(CommandConstants.CREATOR_STUDIO_CB_GUID);

        commandBar.HorizontalAlignment = HorizontalAlignment.Left;
        commandBar.HorizontalContentAlignment = HorizontalAlignment.Center;
        commandBar.VerticalContentAlignment = VerticalAlignment.Center;
        commandBar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
        commandBar.Background = new SolidColorBrush(Colors.Transparent);
        commandBar.IsOpen = false;

        CommandBarHost.Content = commandBar;

        AssetsExplorerHost.Content = _assetsExplorerService.UIObject;
        AssetsExplorerHost.Visibility = Bool2Visibility(!_shellViewSettings.IsHideAssetsExplorer);
        AssetsExplorerHost.SizeChanged += (_, _) => SaveUIState();

        DocumentTabHost.Content = _tabViewManager.UIObject;

        _tabViewManager.Add(new TestTabContent("AAA.txt"));
        _tabViewManager.Add(new TestTabContent("BBB.txt"));
        _tabViewManager.Add(new TestTabContent("CCC.txt"));
    }

    private void OnTabItemsChanged(TabView sender, IVectorChangedEventArgs args)
    {
        UpdateTabVisible();
    }

    private void UpdateTabVisible()
    {
        if (((TabView)_tabViewManager.UIObject).TabItems.Count > 0)
        {
            DocumentTabHost.Visibility = Visibility.Visible;
            DocumentTabBackgrondMask.Visibility = Visibility.Collapsed;
        }
        else
        {
            DocumentTabHost.Visibility = Visibility.Collapsed;
            DocumentTabBackgrondMask.Visibility = Visibility.Visible;
        }
    }

    private void UpdateUIState()
    {
        const double MIN_WIDTH = 240d;

        if (AssetsExplorerHost.Visibility == Visibility.Visible)
        {
            PART_ColumnLeft.MinWidth = MIN_WIDTH;
            PART_ColumnLeft.Width = new GridLength(_shellViewSettings.LeftColumnWidth >= MIN_WIDTH
                ? _shellViewSettings.LeftColumnWidth : MIN_WIDTH);
        }
        else
        {
            PART_ColumnLeft.MinWidth = 0d;
            PART_ColumnLeft.Width = GridLength.Auto;
        }

        MiddleSplitter.Visibility = AssetsExplorerHost.Visibility;
    }

    private void SaveUIState()
    {
        _shellViewSettings.LeftColumnWidth = PART_ColumnLeft.ActualWidth;
        _shellViewSettings.IsHideAssetsExplorer = AssetsExplorerHost.Visibility == Visibility.Collapsed;
    }

    public void ShowOrHideAssetsExplorer(bool isShow = false)
    {
        AssetsExplorerHost.Visibility = Bool2Visibility(isShow);

        if (!isShow)
            SaveUIState();

        UpdateUIState();
    }

    private static Visibility Bool2Visibility(bool value)
    {
        return value ? Visibility.Visible : Visibility.Collapsed;
    }
}

internal sealed class TestTabContent : TabContentVM
{
    public TestTabContent(string name)
    {
        Title = name;
        IconSource = new SegoeFluentIconSource { Glyph = "\xE8A5" };
    }
}
