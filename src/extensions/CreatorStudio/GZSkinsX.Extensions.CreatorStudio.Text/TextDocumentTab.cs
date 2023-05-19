// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.Documents;
using GZSkinsX.Api.CreatorStudio.Documents.Tabs;

namespace GZSkinsX.Extensions.CreatorStudio.Text;

internal sealed class TextDocumentTab : DocumentTabVM
{
    private readonly IDocument _document;

    public TextDocumentTab(IDocument document)
    {
        _document = document;

        _title = _document.FileName;
        _toolTip = _document.Info.FullPath;
        _iconSource = new SegoeFluentIconSource { Glyph = "\xE8A5" };
        _content = new TextDocumentTabContent(document);
    }
}
