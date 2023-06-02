// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using Windows.Storage;

namespace GZSkinsX.SDK.CreatorStudio.Documents;

public sealed class DocumentInfo
{
    public object Data { get; }

    public string Name { get; }

    public string FullPath { get; }

    public DocumentDataType DataType { get; }

    public Guid TypedGuid { get; }

    private DocumentInfo(object data, string name, string fullPath, DocumentDataType dataType, Guid typedGuid)
    {
        Data = data;
        Name = name;
        FullPath = fullPath;
        DataType = dataType;
        TypedGuid = typedGuid;
    }

    public static DocumentInfo CreateEmpty(string name, Guid typedGuid)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        return new(string.Empty, name, string.Empty, DocumentDataType.Empty, typedGuid);
    }

    public static DocumentInfo CreateEmpty(string name, string typedGuidString)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var typedGuid = Guid.Parse(typedGuidString);
        return new(string.Empty, name, string.Empty, DocumentDataType.Empty, typedGuid);
    }

    public static DocumentInfo CreateFromFile(StorageFile file, Guid typedGuid)
    {
        if (file is null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        return new(file, file.Name, file.Path, DocumentDataType.File, typedGuid);
    }

    public static DocumentInfo CreateFromFile(StorageFile file, string typedGuidString)
    {
        if (file is null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        var typedGuid = Guid.Parse(typedGuidString);
        return new(file, file.Name, file.Path, DocumentDataType.File, typedGuid);
    }

    public static DocumentInfo CreateFromData(byte[] data, string name, Guid typedGuid)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        return new(data, name, string.Empty, DocumentDataType.RawData, typedGuid);
    }

    public static DocumentInfo CreateFromData(byte[] data, string name, string typedGuidString)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var typedGuid = Guid.Parse(typedGuidString);
        return new(data, name, string.Empty, DocumentDataType.RawData, typedGuid);
    }
}
