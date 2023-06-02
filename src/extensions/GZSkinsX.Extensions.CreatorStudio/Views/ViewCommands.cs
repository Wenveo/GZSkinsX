// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.SDK.Commands;
using GZSkinsX.SDK.Controls;

using Windows.System;
using Windows.UI.Xaml;

namespace GZSkinsX.Extensions.CreatorStudio.Views;

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_VIEW, Order = 0d)]
internal sealed class ShowOrHideAssetsExplorerCommand : CommandToggleButtonVM
{
    public ShowOrHideAssetsExplorerCommand()
    {
        DisplayName = "Assets Explorer";
        Icon = new SegoeFluentIcon { Glyph = "\uE179" };
        ShortcutKey = new(VirtualKey.E, VirtualKeyModifiers.Control);
    }

    public override void OnInitialize()
    {
        _isChecked = ShellViewControl.Instance.AssetsExplorerIsVisible;
    }

    public override void OnClick(object sender, RoutedEventArgs e)
    {
        ShellViewControl.Instance.ShowOrHideAssetsExplorer(_isChecked);
    }
}
