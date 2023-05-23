// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.Api.Commands;
using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.Documents.Tabs;

using Windows.System;
using Windows.UI.Xaml;

namespace GZSkinsX.Extensions.CreatorStudio.Documents.Tabs;

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
        _isEnabled = false;
        _displayName = "Save As";
        _icon = new SegoeFluentIcon { Glyph = "\uE792" };

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
internal sealed class UndoCommand : CommandButtonVM
{
    private readonly IDocumentTabService _documentTabService;
    private IUndoManager? _undoManager;

    [ImportingConstructor]
    public UndoCommand(IDocumentTabService documentTabService)
    {
        _isEnabled = false;
        _displayName = "Undo";
        _icon = new SegoeFluentIcon { Glyph = "\uE7A7" };
        _shortCutKey = new ShortcutKey(VirtualKey.Z, VirtualKeyModifiers.Control);

        _documentTabService = documentTabService;
        _documentTabService.ActiveTabChanged += OnActiveTabChanged;
    }

    private void OnActiveTabChanged(object sender, ActiveDocumentTabChangedEventArgs e)
    {
        _undoManager = e.ActiveTab is not null && e.ActiveTab.Content is IDocumentTabContent3 content3 &&
            content3.UndoManager is not null && content3.UndoManager.CanUndo ? content3.UndoManager : null;

        IsEnabled = _undoManager is not null;
    }

    public override void OnClick(object sender, RoutedEventArgs e)
    {
        if (_undoManager is not null)
        {
            _undoManager.Undo();

            IsEnabled = _undoManager.CanUndo;
        }
        else
        {
            IsEnabled = false;
        }
    }
}

[Shared, ExportCommandItem]
[CommandItemMetadata(OwnerGuid = CommandConstants.CREATOR_STUDIO_CB_GUID, Group = CommandConstants.GROUP_CREATORSTUDIO_CB_MAIN_EDIT, Order = 1)]
internal sealed class RedoCommand : CommandButtonVM
{
    private readonly IDocumentTabService _documentTabService;
    private IUndoManager? _undoManager;

    [ImportingConstructor]
    public RedoCommand(IDocumentTabService documentTabService)
    {
        _isEnabled = false;
        _displayName = "Redo";
        _icon = new SegoeFluentIcon { Glyph = "\uE7A6" };
        _shortCutKey = new ShortcutKey(VirtualKey.Y, VirtualKeyModifiers.Control);

        _documentTabService = documentTabService;
        _documentTabService.ActiveTabChanged += OnActiveTabChanged;
    }

    private void OnActiveTabChanged(object sender, ActiveDocumentTabChangedEventArgs e)
    {
        _undoManager = e.ActiveTab is not null && e.ActiveTab.Content is IDocumentTabContent3 content3 &&
            content3.UndoManager is not null && content3.UndoManager.CanRedo ? content3.UndoManager : null;

        IsEnabled = _undoManager is not null;
    }

    public override void OnClick(object sender, RoutedEventArgs e)
    {
        if (_undoManager is not null)
        {
            _undoManager.Redo();

            IsEnabled = _undoManager.CanRedo;
        }
        else
        {
            IsEnabled = false;
        }
    }
}
