// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Streams;

namespace GZSkinsX.Uwp.IO.Extensions;

/// <summary>
/// 
/// </summary>
public static class StorageFileExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<IRandomAccessStream> OpenWriteAsync(this IStorageFile item)
    {
        return await item.OpenAsync(FileAccessMode.ReadWrite);
    }
}
