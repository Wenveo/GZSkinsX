// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.SDK.ContextMenu;
using GZSkinsX.SDK.Controls;
using GZSkinsX.SDK.CreatorStudio.Documents.Tabs;

using Windows.System;

namespace GZSkinsX.Extensions.CreatorStudio.Documents.Tabs;

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(OwnerGuid = DocumentTabConstants.DOCUMENT_TAB_CM_GUID, Order = 10d,
    Group = "1000, 74405FE3-39DA-4D0B-9B08-77CDFD8BF278", Guid = "BC21340B-6264-4F74-98D1-D7BBD1585E4D")]
internal sealed class CloseTabCommand : ContextMenuItemBase<DocumentTabContextMenuUIContext>
{
    private readonly IDocumentTabService _documentTabService;

    [ImportingConstructor]
    public CloseTabCommand(IDocumentTabService documentTabService)
    {
        _documentTabService = documentTabService;

        Header = "Close";
        Icon = new SegoeFluentIcon { Glyph = "\uE106" };
        ShortcutKey = new ShortcutKey(VirtualKey.F4, VirtualKeyModifiers.Control);
    }

    public override void OnExecute(DocumentTabContextMenuUIContext context)
    {
        if (context.Parameter is not null)
        {
            _documentTabService.Close(context.Parameter);
        }
    }
}

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(OwnerGuid = DocumentTabConstants.DOCUMENT_TAB_CM_GUID, Order = 20d,
    Group = "1000, 74405FE3-39DA-4D0B-9B08-77CDFD8BF278", Guid = "BC4F1A89-DD96-4B7F-A5CE-22C910DF390A")]
internal sealed class CloseAllTabsCommand : ContextMenuItemBase<DocumentTabContextMenuUIContext>
{
    private readonly IDocumentTabService _documentTabService;

    [ImportingConstructor]
    public CloseAllTabsCommand(IDocumentTabService documentTabService)
    {
        _documentTabService = documentTabService;

        Header = "Close All Tabs";
    }

    public override void OnExecute(DocumentTabContextMenuUIContext context)
    {
        _documentTabService.CloseAllTabs();
    }
}

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(OwnerGuid = DocumentTabConstants.DOCUMENT_TAB_CM_GUID, Order = 30d,
    Group = "1000, 74405FE3-39DA-4D0B-9B08-77CDFD8BF278", Guid = "07451969-E309-49C5-8748-5924F6B35EF8")]
internal sealed class CloseAllButThisCommand : ContextMenuItemBase<DocumentTabContextMenuUIContext>
{
    private readonly IDocumentTabService _documentTabService;

    [ImportingConstructor]
    public CloseAllButThisCommand(IDocumentTabService documentTabService)
    {
        _documentTabService = documentTabService;

        Header = "Close All But This";
    }

    public override bool IsEnabled(DocumentTabContextMenuUIContext context)
    {
        var count = 0;
        foreach (var item in _documentTabService.DocumentTabs)
        {
            if (++count > 1)
            {
                return true;
            }
        }

        return false;
    }

    public override void OnExecute(DocumentTabContextMenuUIContext context)
    {
        if (context.Parameter is not null)
        {
            _documentTabService.CloseAllButThis(context.Parameter);
        }
    }
}
