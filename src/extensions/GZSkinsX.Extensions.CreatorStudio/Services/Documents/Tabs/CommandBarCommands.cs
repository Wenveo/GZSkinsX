// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

using GZSkinsX.Extensions.CreatorStudio.Contracts.Documents;
using GZSkinsX.Extensions.CreatorStudio.Contracts.Documents.Tabs;

using GZSkinsX.SDK.Appx;
using GZSkinsX.SDK.Commands;
using GZSkinsX.SDK.Controls;

using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;

namespace GZSkinsX.Extensions.CreatorStudio.Services.Documents.Tabs;

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
        _isEnabled = false;
        _displayName = "Save";
        _icon = new SegoeFluentIcon { Glyph = "\uE105" };
        _shortCutKey = new ShortcutKey(VirtualKey.S, VirtualKeyModifiers.Control);

        _saveService = saveService;
        _documentTabService = documentTabService;
        _documentTabService.ActiveTabChanged += OnActiveTabChanged;
    }

    private void OnActiveTabChanged(object sender, ActiveDocumentTabChangedEventArgs e)
    {
        if (e.ActiveTab is not null)
        {
            IsEnabled = _saveService.CanSave(e.ActiveTab);
        }
        else
        {
            IsEnabled = false;
        }
    }

    public override void OnClick(object sender, RoutedEventArgs e)
    {
        if (_documentTabService.ActiveTab is not null)
        {
            _saveService.Save(_documentTabService.ActiveTab);
        }
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
        _isEnabled = false;
        _displayName = "Save As";
        _icon = new SegoeFluentIcon { Glyph = "\uE792" };

        _saveService = saveService;
        _documentTabService = documentTabService;
        _documentTabService.ActiveTabChanged += OnActiveTabChanged;
    }

    private void OnActiveTabChanged(object sender, ActiveDocumentTabChangedEventArgs e)
    {
        if (e.ActiveTab is not null)
        {
            IsEnabled = _saveService.CanSave(e.ActiveTab);
        }
        else
        {
            IsEnabled = false;
        }
    }

    public override void OnClick(object sender, RoutedEventArgs e)
    {
        if (_documentTabService.ActiveTab is not null)
        {
            _saveService.SaveAs(_documentTabService.ActiveTab);
        }
    }
}
