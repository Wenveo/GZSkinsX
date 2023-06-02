// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.SDK.CreatorStudio.Documents.Tabs;

namespace GZSkinsX.Extensions.CreatorStudio.Documents.Tabs;

internal sealed class DocumentTabProviderContext
{
    private readonly Lazy<IDocumentTabProvider, DocumentTabProviderMetadataAttribute> _lazy;

    public IDocumentTabProvider Value => _lazy.Value;

    public DocumentTabProviderMetadataAttribute Metadata => _lazy.Metadata;

    public DocumentTabProviderContext(Lazy<IDocumentTabProvider, DocumentTabProviderMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
