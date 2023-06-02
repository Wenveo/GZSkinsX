// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.SDK.Controls;
using GZSkinsX.SDK.CreatorStudio.Documents;
using GZSkinsX.SDK.CreatorStudio.Documents.Tabs;

namespace GZSkinsX.Extensions.CreatorStudio.Text;

internal sealed class TextDocumentTab : DocumentTabVM
{
    public override IDocument Document { get; }

    public override IDocumentTabContent Content { get; }

    public TextDocumentTab(IDocument document)
    {
        Document = document;
        Content = new TextDocumentTabContent(document);

        _title = document.FileName;
        _toolTip = document.Info.FullPath;
        _iconSource = new SegoeFluentIconSource { Glyph = "\xE8A5" };
    }
}
