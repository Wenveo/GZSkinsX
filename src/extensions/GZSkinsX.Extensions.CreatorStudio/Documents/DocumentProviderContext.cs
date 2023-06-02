// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Extensions.CreatorStudio.Contracts.Documents;

namespace GZSkinsX.Extensions.CreatorStudio.Documents;

internal sealed class DocumentProviderContext
{
    private readonly Lazy<IDocumentProvider, DocumentProviderMetadataAttribute> _lazy;

    public IDocumentProvider Value => _lazy.Value;

    public DocumentProviderMetadataAttribute Metadata => _lazy.Metadata;

    public DocumentProviderContext(Lazy<IDocumentProvider, DocumentProviderMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
