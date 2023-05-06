﻿// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Diagnostics;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.CreatorStudio.AssetsExplorer;
using GZSkinsX.Api.CreatorStudio.Commands;
using GZSkinsX.Extensions.CreatorStudio.AssetsExplorer;
using GZSkinsX.Extensions.CreatorStudio.Commands;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Extensions.CreatorStudio.Shell;

public sealed partial class ShellViewControl : Grid
{
    private static readonly Lazy<ShellViewControl> s_lazy = new(() => new());

    public static ShellViewControl Instance => s_lazy.Value;

    private readonly AssetsExplorerService _assetsExplorerService;
    private readonly CommandBarService _commandBarService;
    private readonly ShellViewSettings _shellViewSettings;

    public bool AssetsExplorerIsVisible => AssetsExplorerHost.Visibility == Visibility.Visible;

    private ShellViewControl()
    {
        var serviceLocator = AppxContext.ServiceLocator;

        _assetsExplorerService = (AssetsExplorerService)serviceLocator.Resolve<IAssetsExplorerService>();
        _commandBarService = (CommandBarService)serviceLocator.Resolve<ICommandBarService>();
        _shellViewSettings = serviceLocator.Resolve<ShellViewSettings>();

        InitializeComponent();
        InitializeUIObject();
        UpdateUIState();
    }

    private void InitializeUIObject()
    {
        AssetsExplorerHost.Content = _assetsExplorerService.UIObject;
        CommandBarHost.Content = _commandBarService.UIObject;

        AssetsExplorerHost.Visibility = Bool2Visibility(!_shellViewSettings.IsHideAssetsExplorer);
        AssetsExplorerHost.SizeChanged += (_, _) => SaveUIState();
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