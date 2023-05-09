// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
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
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_NEW)]
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

        UIObject = dropDownButton;
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 0)]
internal sealed class OpenFileCommand : CommandButtonBase
{
    public static int s_count;

    public OpenFileCommand()
    {
        DisplayName = "Open File";
        Icon = new SegoeFluentIcon { Glyph = "\uE197" };
    }

    public override bool IsEnabled()
    {
        return s_count % 2 == 0;
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 1)]
internal sealed class SaveFileCommand : CommandButtonBase
{
    public SaveFileCommand()
    {
        DisplayName = "Save";
        Icon = new SegoeFluentIcon { Glyph = "\uE105" };
    }

    public override void OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileCommand.s_count++;
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 2)]
internal sealed class SaveAsFileCommand : CommandButtonBase
{
    public SaveAsFileCommand()
    {
        DisplayName = "Save As";
        Icon = new SegoeFluentIcon { Glyph = "\uE792" };
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_EDIT, Order = 0)]
internal sealed class UndoCommand : CommandButtonBase
{
    public UndoCommand()
    {
        DisplayName = "Undo";
        Icon = new SegoeFluentIcon { Glyph = "\uE7A7" };
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_EDIT, Order = 1)]
internal sealed class RedoCommand : CommandButtonBase
{
    public RedoCommand()
    {
        DisplayName = "Redo";
        Icon = new SegoeFluentIcon { Glyph = "\uE7A6" };
    }
}
