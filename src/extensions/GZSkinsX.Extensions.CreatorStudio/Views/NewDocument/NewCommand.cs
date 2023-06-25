// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

using GZSkinsX.Api.Commands;
using GZSkinsX.Api.Controls;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.Views.NewDocument;

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

        var newFileMenuItem = new MenuFlyoutItem
        {
            Text = "File",
            Icon = new SegoeFluentIcon { Glyph = "\uE8A5" }
        };

        newFileMenuItem.Click += async (_, _) =>
        {
            var dialog = new ContentDialog { Title = "New File" };
            dialog.Content = new NewFileView(dialog);

            await dialog.ShowAsync();
        };

        var menuFlyout = new MenuFlyout { Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft };
        menuFlyout.Items.Add(newFileMenuItem);

        dropDownButton = new MUXC.DropDownButton
        {
            Content = stackPanel,
            Flyout = menuFlyout,
            Margin = new Thickness(4, 0, 2, 0)
        };

        UIObject = dropDownButton;
    }
}
