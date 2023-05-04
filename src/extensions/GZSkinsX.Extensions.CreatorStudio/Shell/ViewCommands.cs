// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.Commands;

using Windows.System;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.Shell;

[Shared, ExportCommandItem]
[CommandItemMetadata(Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_VIEW, Order = 0d)]
internal sealed class ShowOrHideAssetsExplorerCommand : CommandToggleButtonBase
{
    public override string GetDisplayName() => "Assets Explorer";

    public override IconElement GetIcon() => new SegoeFluentIcon { Glyph = "\uE179" };

    public override CommandHotKey GetHotKey() => new() { Key = VirtualKey.E, Modifiers = VirtualKeyModifiers.Control };

    public override bool IsChecked() => ShellViewControl.Instance.AssetsExplorerIsVisible;

    public override void OnToggle(bool newValue, ICommandUIContext ctx)
    {
        ShellViewControl.Instance.ShowOrHideAssetsExplorer(newValue);
    }
}
