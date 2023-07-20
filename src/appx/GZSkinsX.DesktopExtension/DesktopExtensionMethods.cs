// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Windows.Data.Json;
using Windows.Storage;

namespace GZSkinsX.DesktopExtension;

internal sealed partial class DesktopExtensionMethods : IDesktopExtensionMethods
{
    private readonly struct PackageManifest
    {
        public static readonly PackageManifest Empty = new();

        public readonly string Path;

        public readonly string Version;

        public readonly bool IsEmpty;

        public PackageManifest()
        {
            Path = string.Empty;
            Version = string.Empty;
            IsEmpty = true;
        }

        public PackageManifest(string path, string version)
        {
            Path = path;
            Version = version;
            IsEmpty = false;
        }
    }

    public static readonly string MounterRootPath = Path.Combine(ApplicationData.Current.RoamingFolder.Path, "Mounter");

    public static readonly string MounterTempPath = Path.Combine(ApplicationData.Current.TemporaryFolder.Path, "MT_Temp");

    public const string PackageManifestPath = "PackageManifest.json";

    public const string PackageMetadataPath = "_metadata/package.json";

    public const string PackageBlockMapPath = "_metadata/blockmap.json";

    public static readonly Uri[] Servers = new Uri[]
    {
        new Uri("http://pan.x1.skn.lol/d/%20PanGZSkinsX/")
    };

    public Task<PackageMetadata> GetLocalMTPackageMetadata()
    {
        var metadataFilePath = Path.Combine(MounterRootPath, PackageMetadataPath);
        if (File.Exists(metadataFilePath))
        {
            try
            {
                var metadata = ParseMetadataFromString(File.ReadAllText(metadataFilePath));
                return Task.FromResult(metadata);
            }
            catch
            {
            }
        }

        return Task.FromResult(PackageMetadata.Empty);
    }

    public async Task<bool> CheckUpdatesForMounter()
    {
        var metadata = await GetLocalMTPackageMetadata();
        if (metadata.IsEmpty is false)
        {
            foreach (var host in Servers)
            {
                try
                {
                    var packageManifestUri = new Uri(host, PackageManifestPath);
                    var packageManifest = await DownloadPackageManifestAsync(packageManifestUri);

                    if (StringComparer.Ordinal.Equals(metadata.Version, packageManifest.Version))
                    {
                        return false;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        return true;
    }

    public async Task<bool> TryUpdateMounterAsync()
    {
        foreach (var host in Servers)
        {
            try
            {
                var packageManifestUri = new Uri(host, PackageManifestPath);
                var packageManifest = await DownloadPackageManifestAsync(packageManifestUri);

                var mtPackageUri = new Uri(host, packageManifest.Path);
                await DownloadMTPackageAsync(mtPackageUri);

                return true;
            }
            catch
            {
                continue;
            }
        }

        return false;
    }

    private static async Task<PackageManifest> DownloadPackageManifestAsync(Uri requestUri)
    {
        using var httpClient = new HttpClient();

        var stream = await httpClient.GetStreamAsync(requestUri);
        using var reader = new StreamReader(stream);

        var rawJsonData = await reader.ReadToEndAsync();
        if (JsonObject.TryParse(rawJsonData, out var jsonObj))
        {
            return new PackageManifest(
                jsonObj[nameof(PackageManifest.Path)].GetString(),
                jsonObj[nameof(PackageManifest.Version)].GetString());
        }
        else
        {
            return PackageManifest.Empty;
        }
    }

    private static async Task DownloadMTPackageAsync(Uri requestUri)
    {
        try
        {
            ClearDirectory(MounterTempPath);

            var metadata = PackageMetadata.Empty;

            using (var httpClient = new HttpClient())
            using (var zipArchive = new ZipArchive(await httpClient.GetStreamAsync(requestUri)))
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

                    if (StringComparer.OrdinalIgnoreCase.Equals(entry.FullName, PackageMetadataPath))
                    {
                        outputStream.Seek(0L, SeekOrigin.Begin);
                        metadata = ParseMetadataFromStream(outputStream);
                    }
                }
            }

            if (metadata.IsEmpty)
            {
                // ??
                throw new FileNotFoundException("该包不存在有效的元数据信息！");
            }

            var previousSettingsFilePath = Path.Combine(MounterRootPath, metadata.SettingsFile);
            if (File.Exists(previousSettingsFilePath))
            {
                var newSettingsFilePath = Path.Combine(MounterTempPath, metadata.SettingsFile);
                File.Move(previousSettingsFilePath, newSettingsFilePath);
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

    private static PackageMetadata ParseMetadataFromString(string input)
    {
        if (JsonObject.TryParse(input, out var packageJsonData))
        {
            string GetStringOrEmpty(string memberName)
            {
                return packageJsonData.TryGetValue(memberName, out var value) ? value.GetString() : string.Empty;
            }

            PackageMetadataStartUpArg[] GetStartUpArgsFromJson(string propertyName)
            {
                if (packageJsonData.TryGetValue(propertyName, out var startArgsJsonData) &&
                    startArgsJsonData.ValueType is JsonValueType.Array)
                {
                    var collection = new List<PackageMetadataStartUpArg>();
                    foreach (var item in startArgsJsonData.GetArray())
                    {
                        var startUpArgData = item.GetObject();
                        if (startUpArgData.TryGetValue("Name", out var name) &&
                            startUpArgData.TryGetValue("Value", out var value))
                        {
                            collection.Add(new(name.GetString(), value.GetString()));
                        }
                    }

                    return collection.ToArray();
                }

                return Array.Empty<PackageMetadataStartUpArg>();
            }

            var packageMetadata = new PackageMetadata(
                GetStringOrEmpty(nameof(PackageMetadata.Author)),
                GetStringOrEmpty(nameof(PackageMetadata.Version)),
                GetStringOrEmpty(nameof(PackageMetadata.SettingsFile)),
                GetStringOrEmpty(nameof(PackageMetadata.ExecutableFile)),
                GetStringOrEmpty(nameof(PackageMetadata.ProcStartupArgs)),
                GetStringOrEmpty(nameof(PackageMetadata.ProcTerminateArgs)),
                GetStartUpArgsFromJson(nameof(PackageMetadata.OtherStartupArgs)));

            return packageMetadata;
        }

        return PackageMetadata.Empty;
    }

    private static PackageMetadata ParseMetadataFromStream(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, true, 2048, true);
        return ParseMetadataFromString(reader.ReadToEnd());
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

    public Task<bool> VerifyLocalMTPackageIntegrityAsync()
    {
        var blockmapFilePath = Path.Combine(MounterRootPath, PackageBlockMapPath);
        if (File.Exists(blockmapFilePath))
        {
            try
            {
                var rawJsonData = File.ReadAllText(blockmapFilePath);
                if (JsonObject.TryParse(rawJsonData, out var blockMapJson) &&
                    blockMapJson.TryGetValue("Blocks", out var blocksArray) &&
                    blocksArray.ValueType is JsonValueType.Array)
                {
                    var blockMap = blocksArray.GetArray().ToFrozenDictionary(
                        a => ulong.Parse(a.GetObject()["Hash"].GetString(), CultureInfo.InvariantCulture),
                        b => ulong.Parse(b.GetObject()["Checksum"].GetString(), CultureInfo.InvariantCulture));


                    var rootPathLength = MounterRootPath.Length + 1;
                    var localBlockMap = Directory.EnumerateFiles(MounterRootPath, "*", SearchOption.AllDirectories).ToFrozenDictionary(
                        a => XxHash64.HashToUInt64(Encoding.UTF8.GetBytes(a[rootPathLength..].Replace('\\', '/'))),
                        b => XxHash3.HashToUInt64(File.ReadAllBytes(b)));


                    foreach (var item in blockMap)
                    {
                        if (!localBlockMap.TryGetValue(item.Key, out var checksum) || checksum != item.Value)
                        {
                            return Task.FromResult(false);
                        }
                    }

                    return Task.FromResult(true);
                }
            }
            catch (Exception)
            {
            }
        }

        return Task.FromResult(false);
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
}
