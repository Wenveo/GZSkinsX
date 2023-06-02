// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.SDK.CreatorStudio.Documents;

namespace GZSkinsX.Extensions.CreatorStudio.Text;

[Shared, ExportDocumentProvider]
[DocumentProviderMetadata(TypedGuid = "2E464B27-C4BC-4819-A8C4-DCF622ED4863",
    FileType = "Text File", SupportedExtensions = ".txt;.cfg;.json;.cs")]
internal sealed class TextDocumentProvider : IDocumentProvider
{
    public IDocument CreateDocument(DocumentInfo documentInfo)
    {
        return new TextDocument(documentInfo, CreateKey(documentInfo));
    }

    public IDocumentKey CreateKey(DocumentInfo documentInfo)
    {
        return new FilePathKey(documentInfo.FullPath);
    }
}
