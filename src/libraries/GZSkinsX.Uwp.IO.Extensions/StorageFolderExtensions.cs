// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
#nullable enable

using System;
using System.Threading.Tasks;

using Windows.Storage;

namespace GZSkinsX.Uwp.IO.Extensions;

/// <summary>
/// 
/// </summary>
public static class StorageFolderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static async Task<IStorageFile?> TryGetFileAsync(this IStorageFolder folder, string name)
    {
        IStorageFile? storageFile;
        try
        {
            storageFile = await folder.GetFileAsync(name);
        }
        catch
        {
            storageFile = null;
        }

        return storageFile;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static async Task<IStorageFolder?> TryGetFolderAsync(this IStorageFolder folder, string name)
    {
        IStorageFolder? storageFolder;
        try
        {
            storageFolder = await folder.GetFolderAsync(name);
        }
        catch
        {
            storageFolder = null;
        }

        return storageFolder;
    }
}
