// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Commands;
using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.Documents;
using GZSkinsX.Api.CreatorStudio.Documents.Tabs;

using Windows.Storage.Pickers;
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

[Shared, ExportCommandItem, Export]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 0)]
internal sealed class OpenFileCommand : CommandButtonVM
{
    private IDocumentProviderService? _documentProviderService;
    private IDocumentService? _documentService;

    public OpenFileCommand()
    {
        DisplayName = "Open File";
        Icon = new SegoeFluentIcon { Glyph = "\uE197" };
    }

    public override async void OnClick(object sender, RoutedEventArgs e)
    {
        _documentProviderService ??= AppxContext.Resolve<IDocumentProviderService>();
        _documentService ??= AppxContext.Resolve<IDocumentService>();

        var picker = new FileOpenPicker();
        foreach (var item in _documentProviderService.AllSuppportedExtensions)
        {
            picker.FileTypeFilter.Add(item);
        }

        var files = await picker.PickMultipleFilesAsync();
        foreach (var file in files)
        {
            if (_documentProviderService.TryGetTypedGuid(file.FileType, out var typedGuid))
            {
                var document = _documentService.CreateDocument(DocumentInfo.CreateFromFile(file, typedGuid));

                if (document is not null)
                {
                    _documentService.GetOrAdd(document);
                }
            }
        }
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 1)]
internal sealed class SaveFileCommand : CommandButtonVM
{
    private readonly IDocumentTabService _documentTabService;
    private readonly ISaveService _saveService;

    [ImportingConstructor]
    public SaveFileCommand(IDocumentTabService documentTabService, ISaveService saveService)
    {
        _displayName = "Save";
        _icon = new SegoeFluentIcon { Glyph = "\uE105" };
        _isEnabled = false;

        _saveService = saveService;
        _documentTabService = documentTabService;
        _documentTabService.ActiveTabChanged += OnActiveTabChanged;
    }

    private void OnActiveTabChanged(object sender, ActiveDocumentTabChangedEventArgs e)
    {
        IsEnabled = _saveService.CanSave(e.ActiveTab);
    }

    public override void OnClick(object sender, RoutedEventArgs e)
    {
        _saveService.Save(_documentTabService.ActiveTab);
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_FILE, Order = 2)]
internal sealed class SaveAsFileCommand : CommandButtonVM
{
    private readonly IDocumentTabService _documentTabService;
    private readonly ISaveService _saveService;

    [ImportingConstructor]
    public SaveAsFileCommand(IDocumentTabService documentTabService, ISaveService saveService)
    {
        _displayName = "Save As";
        _icon = new SegoeFluentIcon { Glyph = "\uE792" };
        _isEnabled = false;

        _saveService = saveService;
        _documentTabService = documentTabService;
        _documentTabService.ActiveTabChanged += OnActiveTabChanged;
    }

    private void OnActiveTabChanged(object sender, ActiveDocumentTabChangedEventArgs e)
    {
        IsEnabled = _saveService.CanSave(e.ActiveTab);
    }

    public override void OnClick(object sender, RoutedEventArgs e)
    {
        _saveService.SaveAs(_documentTabService.ActiveTab);
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
