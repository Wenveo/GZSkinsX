// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.Storage;

namespace GZSkinsX.Api.CreatorStudio.DocumentTabs;

public sealed class DocumentInfo
{
    public string Name { get; }

    public object Data { get; }

    public string FullPath { get; }

    public string FileType { get; }

    public DocumentItemType Type { get; }

    public DocumentInfo(StorageFile file)
    {
        Name = file.Name;
        Data = file;
        FullPath = file.Path;
        FileType = file.FileType;
    }

    public DocumentInfo(string name, byte[] data, string fullPath, string fileType)
    {
        Name = name;
        Data = data;
        FullPath = fullPath;
        FileType = fileType;
        Type = DocumentItemType.InMemory;
    }
}
