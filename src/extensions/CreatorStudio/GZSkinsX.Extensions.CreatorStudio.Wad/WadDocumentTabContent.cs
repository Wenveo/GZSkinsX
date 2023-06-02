// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.IO;
using System.Threading.Tasks;

using GZSkinsX.SDK.CreatorStudio.Documents;
using GZSkinsX.SDK.CreatorStudio.Documents.Tabs;
using GZSkinsX.Extensions.CreatorStudio.Wad.Parser;
using GZSkinsX.Extensions.CreatorStudio.Wad.Views;

using Windows.Storage;

namespace GZSkinsX.Extensions.CreatorStudio.Wad;

internal sealed class WadDocumentTabContent : DocumentTabContentVM
{
    private readonly IDocument _document;

    public WadDocumentTabContent(IDocument document)
    {
        _document = document;
    }

    public override async Task OnInitializeAsync()
    {
        await WadHashTable.InitializeAsync();

        if (_document.Info.DataType == DocumentDataType.File)
        {
            var inputStream = await ((StorageFile)_document.Info.Data).OpenStreamForReadAsync();
            var wad = new WadFile(inputStream);

            _uiObject = new WadView(wad);
        }
    }
}
