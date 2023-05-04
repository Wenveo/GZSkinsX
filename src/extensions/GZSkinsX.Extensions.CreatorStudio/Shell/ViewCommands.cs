// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.Commands;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.Shell;

[Shared, ExportCommandItem]
[CommandItemMetadata(Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_VIEW, Order = 0d)]
internal sealed class ShowOrHideAssetsExplorerCommand : CommandToggleButtonBase
{
    private readonly SegoeFluentIcon _icon;
    private readonly string _displayName;

    public ShowOrHideAssetsExplorerCommand()
    {
        _displayName = "Assets Explorer";
        _icon = new SegoeFluentIcon { Glyph = "\uE179" };
    }

    public override string GetDisplayName() => _displayName;

    public override IconElement GetIcon() => _icon;

    public override bool IsChecked() => ShellViewControl.Instance.AssetsExplorerIsVisible;

    public override void OnToggle(bool newValue, ICommandUIContext ctx)
    {
        ShellViewControl.Instance.ShowOrHideAssetsExplorer(newValue);
    }
}
