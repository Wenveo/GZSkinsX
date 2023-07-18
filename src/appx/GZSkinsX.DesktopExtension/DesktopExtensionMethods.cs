// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

using Windows.Storage;

namespace GZSkinsX.DesktopExtension;

internal sealed partial class DesktopExtensionMethods : IDesktopExtensionMethods
{
    public Task<bool> CheckUpdateForMounter()
    {
        return Task.FromResult(true);
    }

    public async Task UpdateMounter()
    {
        const string FILE = "http://pan.x1.skn.lol/d/%20PanGZSkinsX/Mounter/MotClientAgent.mtpkg";

        var targetFolderPath = Path.Combine(ApplicationData.Current.RoamingFolder.Path, "Mounter");
        var tempFolderPath = Path.Combine(ApplicationData.Current.TemporaryFolder.Path, "MT_Temp");

        try
        {
            ClearDirectory(tempFolderPath);

            using (var httpClient = new HttpClient())
            using (var zipArchive = new ZipArchive(await httpClient.GetStreamAsync(FILE)))
            {
                foreach (var entry in zipArchive.Entries)
                {
                    // Directory
                    if (entry.FullName[^1] == Path.AltDirectorySeparatorChar)
                    {
                        continue;
                    }

                    var outputPath = Path.Combine(tempFolderPath, entry.FullName);
                    var parentDirectory = Path.GetDirectoryName(outputPath);

                    if (!Directory.Exists(parentDirectory))
                        Directory.CreateDirectory(parentDirectory);

                    using var entryStream = entry.Open();
                    using var outputStream = File.Create(outputPath);

                    await entryStream.CopyToAsync(outputStream);
                }
            }

            DeleteDirectoryIfExists(targetFolderPath);
            Directory.Move(tempFolderPath, targetFolderPath);
        }
        finally
        {
            // Clean
            DeleteDirectoryIfExists(tempFolderPath);
        }
    }

    private static void ClearDirectory(string path)
    {
        DeleteDirectoryIfExists(path);
        Directory.CreateDirectory(path);
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
}
