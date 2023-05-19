// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

namespace GZSkinsX.Api.CreatorStudio.Documents;

public sealed class FilePathKey : IDocumentKey, IEquatable<FilePathKey>
{
    private readonly string _filePath;

    public FilePathKey(string filePath)
    {
        _filePath = filePath;
    }

    public bool Equals(FilePathKey? other)
    {
        return other != null && StringComparer.OrdinalIgnoreCase.Equals(_filePath, other._filePath);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as FilePathKey);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(_filePath);
    }

    public override string ToString()
    {
        return _filePath;
    }
}
