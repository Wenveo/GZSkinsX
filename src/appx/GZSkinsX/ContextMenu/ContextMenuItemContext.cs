// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Api.ContextMenu;

namespace GZSkinsX.ContextMenu;

internal sealed class ContextMenuItemContext
{
    private readonly Lazy<IContextMenuItem, ContextMenuItemMetadataAttribute> _lazy;

    public IContextMenuItem Value => _lazy.Value;

    public ContextMenuItemMetadataAttribute Metadata => _lazy.Metadata;

    public ContextMenuItemContext(Lazy<IContextMenuItem, ContextMenuItemMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
