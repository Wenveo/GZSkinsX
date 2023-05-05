// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace GZSkinsX.Api.ContextMenu;

public readonly struct CreatedContextMenuItem
{
    public ContextMenuItemMetadataAttribute? Metadata { get; }

    public IContextMenuItem? ContextMenuItem { get; }

    [MemberNotNullWhen(false, nameof(Metadata), nameof(ContextMenuItem))]
    public bool IsEmpty { get; }

    public CreatedContextMenuItem()
    {
        IsEmpty = true;
    }

    public CreatedContextMenuItem(ContextMenuItemMetadataAttribute metadata, IContextMenuItem contextMenuItem)
    {
        Metadata = metadata;
        ContextMenuItem = contextMenuItem;
    }
}
