// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Text;
using System.Threading.Tasks;

using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.DocumentTabs;
using GZSkinsX.Api.Tabs;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace GZSkinsX.Extensions.CreatorStudio.DocumentTabs.Test;

[Shared, ExportDocumentTabProvider]
[DocumentTabProviderMetadata(FileType = "Text File", SupportedExtensions = ".cfg;.txt;.json")]
internal sealed class TextFileProvider : IDocumentTabProvider
{
    private sealed class TextDocument : TabContentVM, IDocumentTabContent
    {
        private readonly DocumentInfo _documentInfo;

        public object Key { get; }

        public bool CanSave => false;

        public TextDocument(DocumentInfo documentInfo)
        {
            Key = documentInfo.Name;
            _documentInfo = documentInfo;
        }

        public override void OnInitialize()
        {
            _title = _documentInfo.Name;
            _iconSource = new SegoeFluentIconSource { Glyph = "\xE8A5" };
        }

        public void OnSave() { }

        public override async Task OnInitializeAsync()
        {
            var run = new Run
            {
                Text = _documentInfo.Type == DocumentItemType.File
                ? await FileIO.ReadTextAsync((StorageFile)_documentInfo.Data)
                : Encoding.UTF8.GetString((byte[])_documentInfo.Data)
            };

            await Task.Delay(1000);

            var paragraph = new Paragraph();
            paragraph.Inlines.Add(run);

            var richTextBlock = new RichTextBlock();
            richTextBlock.Blocks.Add(paragraph);

            _uiObject = new ScrollViewer { Content = richTextBlock };
        }


    }

    public IDocumentTabContent Create(DocumentInfo documentInfo)
    {
        return new TextDocument(documentInfo);
    }
}
