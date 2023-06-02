// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using GZSkinsX.SDK.CreatorStudio.Documents;
using GZSkinsX.SDK.CreatorStudio.Documents.Tabs;

using Windows.Storage;

namespace GZSkinsX.Extensions.CreatorStudio.Text;

internal sealed class TextDocumentTabContent : DocumentTabContentVM
{
    private readonly IDocument _document;

    public TextDocumentTabContent(IDocument document)
    {
        _document = document;
    }

    public override async Task OnInitializeAsync()
    {
        var text = _document.Info.DataType == DocumentDataType.Empty ? string.Empty
            : Encoding.UTF8.GetString(_document.Info.DataType == DocumentDataType.File
            ? WindowsRuntimeBufferExtensions.ToArray(await FileIO.ReadBufferAsync((StorageFile)_document.Info.Data))
            : (byte[])_document.Info.Data);

        var textControlBox = new TextControlBox.TextControlBox();
        textControlBox.SetText(text);

        if (_document.Info.Name.EndsWith(".cs"))
        {
            textControlBox.CodeLanguage = TextControlBox.TextControlBox.GetCodeLanguageFromId("C#");
        }

        _uiObject = textControlBox;
    }
}
