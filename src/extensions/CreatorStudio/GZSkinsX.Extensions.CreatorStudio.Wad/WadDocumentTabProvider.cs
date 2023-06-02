// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.SDK.CreatorStudio.Documents;
using GZSkinsX.SDK.CreatorStudio.Documents.Tabs;

namespace GZSkinsX.Extensions.CreatorStudio.Wad;

[Shared, ExportDocumentTabProvider]
[DocumentTabProviderMetadata(TypedGuid = "85E5EDDB-DE5C-4EFF-A50C-AD455DD4CE45")]
internal sealed class WadDocumentTabProvider : IDocumentTabProvider
{
    public IDocumentTab Create(IDocument document)
    {
        return new WadDocumentTab(document);
    }
}
