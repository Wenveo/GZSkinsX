// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Search;

namespace GZSkinsX.Contracts.Helpers;

// Adapted from https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/Microsoft.Toolkit.Uwp/Helpers/StorageFileHelper.cs

/// <summary>
/// This class provides static helper methods for <see cref="StorageFile" />.
/// </summary>
public static class StorageFileHelper
{
    /// <summary>
    /// Gets a value indicating whether a file exists in the current folder.
    /// </summary>
    /// <param name="folder">
    /// The <see cref="StorageFolder"/> to look for the file in.
    /// </param>
    /// <param name="fileName">
    /// The <see cref="string"/> filename of the file to search for. Must include the file extension and is not case-sensitive.
    /// </param>
    /// <param name="isRecursive">
    /// The <see cref="bool"/>, indicating if the subfolders should also be searched through.
    /// </param>
    /// <returns>
    /// <c>true</c> if the file exists; otherwise, <c>false</c>.
    /// </returns>
    public static Task<bool> FileExistsAsync(this StorageFolder folder, string fileName, bool isRecursive = false)
        => isRecursive ? FileExistsInSubtreeAsync(folder, fileName) : FileExistsInFolderAsync(folder, fileName);

    /// <summary>
    /// Gets a value indicating whether a filename is correct or not using the Storage feature.
    /// </summary>
    /// <param name="fileName">The filename to test. Must include the file extension and is not case-sensitive.</param>
    /// <returns><c>true</c> if the filename is valid; otherwise, <c>false</c>.</returns>
    public static bool IsFileNameValid(string fileName)
    {
        var illegalChars = Path.GetInvalidFileNameChars();
        return fileName.All(c => !illegalChars.Contains(c));
    }

    /// <summary>
    /// Gets a value indicating whether a file path is correct or not using the Storage feature.
    /// </summary>
    /// <param name="filePath">The file path to test. Must include the file extension and is not case-sensitive.</param>
    /// <returns><c>true</c> if the file path is valid; otherwise, <c>false</c>.</returns>
    public static bool IsFilePathValid(string filePath)
    {
        var illegalChars = Path.GetInvalidPathChars();
        return filePath.All(c => !illegalChars.Contains(c));
    }

    /// <summary>
    /// Gets a value indicating whether a file exists in the current folder.
    /// </summary>
    /// <param name="folder">
    /// The <see cref="StorageFolder"/> to look for the file in.
    /// </param>
    /// <param name="fileName">
    /// The <see cref="string"/> filename of the file to search for. Must include the file extension and is not case-sensitive.
    /// </param>
    /// <returns>
    /// <c>true</c> if the file exists; otherwise, <c>false</c>.
    /// </returns>
    internal static async Task<bool> FileExistsInFolderAsync(StorageFolder folder, string fileName)
    {
        var item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);
        return (item != null) && item.IsOfType(StorageItemTypes.File);
    }

    /// <summary>
    /// Gets a value indicating whether a file exists in the current folder or in one of its subfolders.
    /// </summary>
    /// <param name="rootFolder">
    /// The <see cref="StorageFolder"/> to look for the file in.
    /// </param>
    /// <param name="fileName">
    /// The <see cref="string"/> filename of the file to search for. Must include the file extension and is not case-sensitive.
    /// </param>
    /// <returns>
    /// <c>true</c> if the file exists; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Exception thrown if the <paramref name="fileName"/> contains a quotation mark.
    /// </exception>
    internal static async Task<bool> FileExistsInSubtreeAsync(StorageFolder rootFolder, string fileName)
    {
        if (fileName.IndexOf('"') >= 0)
        {
            throw new ArgumentException(nameof(fileName));
        }

        var options = new QueryOptions
        {
            FolderDepth = FolderDepth.Deep,
            UserSearchFilter = $"filename:=\"{fileName}\"" // “:=” is the exact-match operator
        };

        var files = await rootFolder.CreateFileQueryWithOptions(options).GetFilesAsync().AsTask().ConfigureAwait(false);
        return files.Count > 0;
    }
}
