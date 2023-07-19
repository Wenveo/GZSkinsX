// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

using Windows.Data.Json;
using Windows.Storage;

namespace GZSkinsX.DesktopExtension;

internal sealed partial class DesktopExtensionMethods : IDesktopExtensionMethods
{
    internal string MounterRootPath { get; } = Path.Combine(ApplicationData.Current.RoamingFolder.Path, "Mounter");

    internal string MounterTempPath { get; } = Path.Combine(ApplicationData.Current.TemporaryFolder.Path, "MT_Temp");

    public Task<PackageMetadata> GetMTPackageMetadata()
    {
        var targetFilePath = Path.Combine(MounterRootPath, "_metadata", "package.json");
        if (File.Exists(targetFilePath))
        {
            var rawJsonData = File.ReadAllText(targetFilePath);
            if (JsonObject.TryParse(rawJsonData, out var packageJsonData))
            {
                string GetStringOrEmpty(string memberName)
                {
                    return packageJsonData.TryGetValue(memberName, out var value) ? value.GetString() : string.Empty;
                }

                PackageMetadataStartUpArgs[] GetStartUpArgsFromJson(string propertyName)
                {
                    if (packageJsonData.TryGetValue(propertyName, out var startArgsJsonData) &&
                        startArgsJsonData.ValueType is JsonValueType.Array)
                    {
                        var collection = new List<PackageMetadataStartUpArgs>();
                        foreach (var item in startArgsJsonData.GetArray())
                        {
                            var startUpArgData = item.GetObject();
                            if (startUpArgData.TryGetValue("Name", out var name) &&
                                startUpArgData.TryGetValue("Args", out var value))
                            {
                                collection.Add(new(name.GetString(), value.GetString()));
                            }
                        }

                        return collection.ToArray();
                    }

                    return Array.Empty<PackageMetadataStartUpArgs>();
                }

                var packageMetadata = new PackageMetadata(
                    GetStringOrEmpty(nameof(PackageMetadata.Author)),
                    GetStringOrEmpty(nameof(PackageMetadata.Version)),
                    GetStringOrEmpty(nameof(PackageMetadata.SettingsFile)),
                    GetStringOrEmpty(nameof(PackageMetadata.ExecutableFile)),
                    GetStringOrEmpty(nameof(PackageMetadata.MounterStartUpParm)),
                    GetStringOrEmpty(nameof(PackageMetadata.MounterStopParm)),
                    GetStartUpArgsFromJson(nameof(PackageMetadata.StartUpArgs)));

                return Task.FromResult(packageMetadata);
            }
        }

        return Task.FromResult(PackageMetadata.Empty);
    }

    public Task<bool> CheckUpdateForMounter()
    {
        return Task.FromResult(true);
    }

    public async Task UpdateMounter()
    {
        const string FILE = "http://pan.x1.skn.lol/d/%20PanGZSkinsX/Mounter/MotClientAgent.mtpkg";

        try
        {
            ClearDirectory(MounterTempPath);

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

                    var outputPath = Path.Combine(MounterTempPath, entry.FullName);
                    var parentDirectory = Path.GetDirectoryName(outputPath);

                    if (!Directory.Exists(parentDirectory))
                        Directory.CreateDirectory(parentDirectory);

                    using var entryStream = entry.Open();
                    using var outputStream = File.Create(outputPath);

                    await entryStream.CopyToAsync(outputStream);
                }
            }

            DeleteDirectoryIfExists(MounterRootPath);
            Directory.Move(MounterTempPath, MounterRootPath);
        }
        finally
        {
            // Clean
            DeleteDirectoryIfExists(MounterTempPath);
        }
    }

    public Task SetOwner(int processId)
    {
        try
        {
            var owner = Process.GetProcessById(processId);
            owner.EnableRaisingEvents = true;
            owner.Exited += (_, _) => Program.Exit(0);
        }
        catch
        {

        }

        return Task.CompletedTask;
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
