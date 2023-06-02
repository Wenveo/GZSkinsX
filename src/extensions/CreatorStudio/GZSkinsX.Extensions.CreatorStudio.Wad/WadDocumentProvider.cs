// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

using GZSkinsX.SDK.CreatorStudio.Documents;

namespace GZSkinsX.Extensions.CreatorStudio.Wad;

[Shared, ExportDocumentProvider]
[DocumentProviderMetadata(TypedGuid = "85E5EDDB-DE5C-4EFF-A50C-AD455DD4CE45",
    FileType = "Riot Wrapper Asset Data", SupportedExtensions = ".wad;.client")]
internal class WadDocumentProvider : IDocumentProvider
{
    public IDocument CreateDocument(DocumentInfo documentInfo)
    {
        return new WadDocument(documentInfo, CreateKey(documentInfo));
    }

    public IDocumentKey CreateKey(DocumentInfo documentInfo)
    {
        return new FilePathKey(documentInfo.FullPath);
    }
}
