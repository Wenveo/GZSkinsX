﻿// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.Commands;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.Commands;

[Shared, ExportCommandItem]
[CommandItemMetadata(Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_NEW)]
internal sealed class NewCommand : CommandObjectBase
{
    public readonly MUXC.DropDownButton dropDownButton;

    public NewCommand()
    {
        var iconElement = new Viewbox
        {
            Height = 16d,
            Width = 16d,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = new SegoeFluentIcon { Glyph = "\uECC8" }
        };

        var header = new TextBlock
        {
            Margin = new Thickness(8, 0, 0, 1),
            Text = "New"
        };

        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
        stackPanel.Children.Add(iconElement);
        stackPanel.Children.Add(header);

        var menuFlyout = new MenuFlyout { Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft };
        menuFlyout.Items.Add(new MenuFlyoutItem
        {
            Text = "File",
            Icon = new SegoeFluentIcon { Glyph = "\uE8A5" }
        });

        dropDownButton = new MUXC.DropDownButton
        {
            Content = stackPanel,
            Flyout = menuFlyout,
            Margin = new Thickness(4, 0, 2, 0)
        };
    }

    public override FrameworkElement GetUIObject() => dropDownButton;
}

[Shared, ExportCommandItem]
[CommandItemMetadata(Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 0)]
internal sealed class OpenFileCommand : CommandButtonBase
{
    public override string? GetDisplayName() => "Open File";

    public override IconElement? GetIcon() => new SegoeFluentIcon { Glyph = "\uE197" };

    public override void OnClick(ICommandUIContext ctx)
    {
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 1)]
internal sealed class SaveFileCommand : CommandButtonBase
{
    public override string? GetDisplayName() => "Save";

    public override IconElement? GetIcon() => new SegoeFluentIcon { Glyph = "\uE105" };

    public override void OnClick(ICommandUIContext ctx)
    {
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 2)]
internal sealed class SaveAsFileCommand : CommandButtonBase
{
    public override string? GetDisplayName() => "Save As";

    public override IconElement? GetIcon() => new SegoeFluentIcon { Glyph = "\uE792" };

    public override void OnClick(ICommandUIContext ctx)
    {
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_EDIT, Order = 0)]
internal sealed class UndoCommand : CommandButtonBase
{
    public override string? GetDisplayName() => "Undo";

    public override IconElement? GetIcon() => new SegoeFluentIcon { Glyph = "\uE7A7" };

    public override void OnClick(ICommandUIContext ctx)
    {
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_EDIT, Order = 1)]
internal sealed class RedoCommand : CommandButtonBase
{
    public override string? GetDisplayName() => "Redo";

    public override IconElement? GetIcon() => new SegoeFluentIcon { Glyph = "\uE7A6" };

    public override void OnClick(ICommandUIContext ctx)
    {
    }
}
