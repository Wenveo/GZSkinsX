// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;

using GZSkinsX.Api.CreatorStudio.Documents;
using GZSkinsX.Api.CreatorStudio.Documents.Tabs;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace GZSkinsX.Extensions.CreatorStudio.Text;

internal sealed class TextSaver : ITabSaver
{
    private readonly IDocumentTab _tab;
    private StorageFile? _savedFile;

    public bool CanSave => true;

    public TextSaver(IDocumentTab tab)
    {
        _tab = tab;

        var info = tab.Document.Info;
        if (info.DataType == DocumentDataType.File)
        {
            _savedFile = info.Data as StorageFile;
        }
    }

    private async void SaveImpl(bool isSaveAs)
    {
        if (isSaveAs || _savedFile is null)
        {
            var savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });

            var savedFile = await savePicker.PickSaveFileAsync();
            if (savedFile is not null)
                _savedFile = savedFile;
            else
                return;

        }

        var tabContent = (TextDocumentTabContent)_tab.Content;
        var textControl = (TextControlBox.TextControlBox)tabContent.UIObject!;

        var text = textControl.GetText();
        await FileIO.WriteTextAsync(_savedFile, text, Windows.Storage.Streams.UnicodeEncoding.Utf8);
    }

    public void Save()
    {
        SaveImpl(false);
    }

    public void SaveAs()
    {
        SaveImpl(true);
    }
}
