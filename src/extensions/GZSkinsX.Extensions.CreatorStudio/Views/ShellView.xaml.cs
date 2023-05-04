﻿// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.CreatorStudio.Commands;
using GZSkinsX.Extensions.CreatorStudio.AssetsExplorer;
using GZSkinsX.Extensions.CreatorStudio.Commands;

using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Extensions.CreatorStudio.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellView : Page
{
    public ShellView()
    {
        InitializeComponent();

        var serviceLocator = AppxContext.ServiceLocator;

        var assetsExplorerService = serviceLocator.Resolve<AssetsExplorerService>();
        AssetsExplorerHost.Content = assetsExplorerService.UIObject;

        var commandBarService = (CommandBarService)serviceLocator.Resolve<ICommandBarService>();
        CommandBarHost.Content = commandBarService.UIObject;
    }
}
