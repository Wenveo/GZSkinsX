// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.SDK.CreatorStudio.Documents;

namespace GZSkinsX.Extensions.CreatorStudio.Text;

internal sealed class TextDocument : IDocument
{
    public DocumentInfo Info { get; }

    public IDocumentKey Key { get; }

    public string FileName { get; }

    public TextDocument(DocumentInfo info, IDocumentKey key)
    {
        Info = info;
        Key = key;

        FileName = info.Name;
    }
}
