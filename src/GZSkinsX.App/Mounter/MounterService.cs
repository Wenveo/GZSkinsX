// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Mounter;

using Windows.Data.Json;
using Windows.Foundation;
using Windows.Storage;
using Windows.Web.Http;

namespace GZSkinsX.Mounter;

/// <inheritdoc cref="IMounterService"/>
[Shared, Export(typeof(IMounterService))]
internal sealed class MounterService : IMounterService
{
    private static Uri[] OnlineManifests { get; } = new Uri[]
    {
        new Uri("http://pan.x1.skn.lol/d/%20PanGZSkinsX/PackageManifest.json")
    };

    private readonly MounterSettings _mounterSettings;

    public event TypedEventHandler<IMounterService, bool>? IsRunningChanged;

    [ImportingConstructor]
    public MounterService(MounterSettings mounterSettings)
    {
        _mounterSettings = mounterSettings;

        var worker = new BackgroundWorker();
        worker.DoWork += DoSomething;
        worker.RunWorkerAsync();
    }

    // ヾ(•ω•`)o
    private async void DoSomething(object sender, DoWorkEventArgs e)
    {
        async Task CheckExitAsync()
        {
            while (true)
            {
                await Task.Delay(1240);

                try
                {
                    if (await App.DesktopExtensionMethods.IsMTRunning() is false)
                    {
                        IsRunningChanged?.Invoke(this, false);
                        break;
                    }
                }
                catch
                {
                }
            }

            await Task.CompletedTask;
        }

        async Task CheckRunAsync()
        {
            while (true)
            {
                await Task.Delay(1240);

                try
                {
                    if (await GetIsRunningAsync())
                    {
                        IsRunningChanged?.Invoke(this, true);
                        break;
                    }
                }
                catch
                {
                }
            }

            await Task.CompletedTask;
        }

        while (true)
        {
            try
            {
                if (await GetIsRunningAsync())
                {
                    IsRunningChanged?.Invoke(this, true);
                    await CheckExitAsync();
                }
                else
                {
                    IsRunningChanged?.Invoke(this, false);
                    await CheckRunAsync();
                }
            }
            catch
            {
            }
        }
    }

    public async Task<bool> CheckForUpdatesAsync()
    {
        var metadata = await TryGetCurrentPackageMetadataAsync();
        if (metadata.IsEmpty is false)
        {
            var httpClient = new HttpClient();

            try
            {
                var onlineManifest = await DownloadPackageManifestAsync(httpClient);
                if (StringComparer.Ordinal.Equals(metadata.Version, onlineManifest.Version))
                {
                    return false;
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError($"MounterService::CheckForUpdatesAsync -> Failed to check for updates. Exception message: \n\"{excp.Message}\".");
                throw;
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        return true;
    }

    public async Task<MTPackageMetadata> GetCurrentPackageMetadataAsync(params string[] filter)
    {
        return await GetLocalMTPackageMetadataAsync(await GetMounterWorkingDirectoryAsync(), filter);
    }

    public async Task<bool> GetIsRunningAsync()
    {
        return await App.DesktopExtensionMethods.IsMTRunning();
    }

    public async Task LaunchAsync()
    {
        var workingDirectory = await GetMounterWorkingDirectoryAsync();

        var localPackageMetadata =
            await GetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile),
                nameof(MTPackageMetadata.ProcStartupArgs));

        var executableFile = Path.Combine(workingDirectory.Path, localPackageMetadata.ExecutableFile);
        await App.DesktopExtensionMethods.ProcessLaunch(executableFile, localPackageMetadata.ProcStartupArgs, true);
    }

    public async Task LaunchAsync(string args)
    {
        var workingDirectory = await GetMounterWorkingDirectoryAsync();

        var localPackageMetadata =
            await GetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile));

        var executableFile = Path.Combine(workingDirectory.Path, localPackageMetadata.ExecutableFile);
        await App.DesktopExtensionMethods.ProcessLaunch(executableFile, args, true);
    }

    public async Task TerminateAsync()
    {
        var workingDirectory = await GetMounterWorkingDirectoryAsync();

        var localPackageMetadata =
            await GetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile),
                nameof(MTPackageMetadata.ProcTerminateArgs));

        var executableFile = Path.Combine(workingDirectory.Path, localPackageMetadata.ExecutableFile);
        await App.DesktopExtensionMethods.ProcessLaunch(executableFile, localPackageMetadata.ProcTerminateArgs, true);
    }

    public async Task<MTPackageMetadata> TryGetCurrentPackageMetadataAsync(params string[] filter)
    {
        try
        {
            return await GetCurrentPackageMetadataAsync(filter);
        }
        catch
        {
            return MTPackageMetadata.Empty;
        }
    }

    public async Task UpdateAsync(IProgress<double>? progress = null)
    {
        progress?.Report(0.0d);
        using var httpClient = new HttpClient();

        progress?.Report(2.0d);
        var workingDirectory = await TryGetMounterWorkingDirectoryAsync();

        progress?.Report(4.0d);

        var previousMetadata = workingDirectory is not null ? await TryGetLocalMTPackageMetadataAsync(workingDirectory,
            nameof(MTPackageMetadata.Version), nameof(MTPackageMetadata.SettingsFile)) : MTPackageMetadata.Empty;

        progress?.Report(9.0d);
        var onlineManifest = await DownloadPackageManifestAsync(httpClient);

        progress?.Report(28.0d);
        if (StringComparer.Ordinal.Equals(previousMetadata.Version, onlineManifest.Version) is false)
        {
            var destFolder = await DownloadMTPackageAsync(httpClient, new(onlineManifest.Path));
            progress?.Report(84.0d);

            // New Metadata
            var newMetadata = await GetLocalMTPackageMetadataAsync(destFolder, nameof(MTPackageMetadata.SettingsFile));

            // Copy settings file
            if (workingDirectory is not null && previousMetadata.IsEmpty is false)
            {
                progress?.Report(92.0d);

                var settingsFilePath = Path.Combine(workingDirectory.Path, previousMetadata.SettingsFile);
                if (File.Exists(settingsFilePath))
                {
                    File.Copy(settingsFilePath, Path.Combine(destFolder.Path, newMetadata.SettingsFile), true);
                }

                progress?.Report(98.0d);
            }

            _mounterSettings.WorkingDirectory = destFolder.Name;
        }

        progress?.Report(100.0d);

        await TryCleanupMounterRootFolderAsync();
    }

    public async Task<bool> VerifyContentIntegrityAsync()
    {
        var workingDirectory = await TryGetMounterWorkingDirectoryAsync();
        if (workingDirectory is null)
        {
            return false;
        }

        var metadataFolder = await workingDirectory.TryGetItemAsync("_metadata");
        if (metadataFolder is null || metadataFolder.IsOfType(StorageItemTypes.Folder) is false)
        {
            return false;
        }

        var metadataFile = await ((StorageFolder)metadataFolder).TryGetItemAsync("blockmap.json");
        if (metadataFile is null || metadataFile.IsOfType(StorageItemTypes.File) is false)
        {
            return false;
        }

        var blockMapContent = await FileIO.ReadTextAsync((IStorageFile)metadataFile);
        if (JsonObject.TryParse(blockMapContent, out var blockMapJson) is false ||
            blockMapJson.TryGetValue("Blocks", out var blocksArray) is false ||
            blocksArray.ValueType is not JsonValueType.Array)
        {
            return false;
        }

        var localBlockMap = blocksArray.GetArray().ToFrozenDictionary(
            a => ulong.Parse(a.GetObject()["Hash"].GetString(), CultureInfo.InvariantCulture),
            b => ulong.Parse(b.GetObject()["Checksum"].GetString(), CultureInfo.InvariantCulture));

        var parentFolderPathLength = workingDirectory.Path.Length + 1;
        var hashToPath = Directory.EnumerateFiles(workingDirectory.Path, "*", SearchOption.AllDirectories).ToFrozenDictionary(
            a => XxHash64.HashToUInt64(Encoding.UTF8.GetBytes(a[parentFolderPathLength..].Replace('\\', '/'))), b => b);


        foreach (var item in localBlockMap)
        {
            if (hashToPath.TryGetValue(item.Key, out var path) is false)
            {
                return false;
            }

            try
            {
                if (XxHash3.HashToUInt64(File.ReadAllBytes(path)) != item.Value)
                {
                    return false;
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogWarning($"MounterService::VerifyLocalMTPackageIntegrityAsync -> Failed to calculate file checksum: {excp.Message}");
                return false;
            }
        }

        return true;
    }

    private async Task<StorageFolder> CreateAnEmptyFolderAsync(StorageFolder relativeTo, string folderName)
    {
        var targetFolder = await relativeTo.TryGetItemAsync(folderName);
        if (targetFolder is not null && targetFolder.IsOfType(StorageItemTypes.Folder))
        {
            await targetFolder.DeleteAsync();
        }

        return await relativeTo.CreateFolderAsync(folderName);
    }

    private async Task<MTPackageManifest> DownloadPackageManifestAsync(HttpClient httpClient)
    {
        foreach (var uri in OnlineManifests)
        {
            using var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonObject.Parse(result);

            return new MTPackageManifest(
                jsonObject[nameof(MTPackageManifest.Path)].GetString(),
                jsonObject[nameof(MTPackageManifest.Version)].GetString());
        }

        throw new IndexOutOfRangeException();
    }

    private async Task<StorageFolder> DownloadMTPackageAsync(HttpClient httpClient, Uri requestUri)
    {
        using var responseStream = await httpClient.GetInputStreamAsync(requestUri);
        using var zipArchive = new ZipArchive(responseStream.AsStreamForRead());

        var rootFolder = await GetMounterRootFolderAsync();
        var destFolder = await CreateAnEmptyFolderAsync(rootFolder, Guid.NewGuid().ToString());

        zipArchive.ExtractToDirectory(destFolder.Path);
        return destFolder;
    }

    private async Task<MTPackageMetadata> GetLocalMTPackageMetadataAsync(StorageFolder workingDirectory, params string[] filter)
    {
        var metadataFolder = await workingDirectory.GetFolderAsync("_metadata");
        var metadataFile = await metadataFolder.GetFileAsync("package.json");

        var content = await FileIO.ReadTextAsync(metadataFile, default);
        return ParseMetadataFromString(content, filter);
    }

    private async Task<StorageFolder> GetMounterRootFolderAsync()
    {
        return await GetOrCreateFolderAsync(ApplicationData.Current.RoamingFolder, "Mounter");
    }

    private async Task<StorageFolder> GetMounterWorkingDirectoryAsync()
    {
        var rootFolder = await GetMounterRootFolderAsync();
        return await rootFolder.GetFolderAsync(_mounterSettings.WorkingDirectory);
    }

    private async Task<StorageFolder> GetOrCreateFolderAsync(StorageFolder relativeTo, string folderName)
    {
        var targetFolder = await relativeTo.TryGetItemAsync(folderName);
        if (targetFolder is not null && targetFolder.IsOfType(StorageItemTypes.Folder))
        {
            return (StorageFolder)targetFolder;
        }
        else
        {
            return await relativeTo.CreateFolderAsync(folderName);
        }
    }

    private MTPackageMetadata ParseMetadataFromString(string input, params string[] filter)
    {
        static IEnumerable<MTPackageMetadataStartUpArgument> GetStartUpArgs(IJsonValue jsonArray)
        {
            if (jsonArray.ValueType is JsonValueType.Array)
            {
                foreach (var item in jsonArray.GetArray())
                {
                    var startUpArgData = item.GetObject();
                    if (startUpArgData.TryGetValue("Name", out var name) &&
                        startUpArgData.TryGetValue("Value", out var value))
                    {
                        yield return new(name.GetString(), value.GetString());
                    }
                }

                yield break;
            }
        }

        if (JsonObject.TryParse(input, out var jsonObject) is false)
        {
            return MTPackageMetadata.Empty;
        }

        IJsonValue? tempValue;

        string author = string.Empty, version = string.Empty;
        string settingsFile = string.Empty, executableFile = string.Empty;
        string procStartupArgs = string.Empty, procTerminateArgs = string.Empty;
        var startUpArgs = Array.Empty<MTPackageMetadataStartUpArgument>();

        if (filter.Length is 0 || filter.Contains(nameof(MTPackageMetadata.Author), StringComparer.OrdinalIgnoreCase))
        {
            if (jsonObject.TryGetValue(nameof(MTPackageMetadata.Author), out tempValue))
            {
                author = tempValue.GetString();
            }
        }

        if (filter.Length is 0 || filter.Contains(nameof(MTPackageMetadata.Version), StringComparer.OrdinalIgnoreCase))
        {
            if (jsonObject.TryGetValue(nameof(MTPackageMetadata.Version), out tempValue))
            {
                version = tempValue.GetString();
            }
        }

        if (filter.Length is 0 || filter.Contains(nameof(MTPackageMetadata.SettingsFile), StringComparer.OrdinalIgnoreCase))
        {
            if (jsonObject.TryGetValue(nameof(MTPackageMetadata.SettingsFile), out tempValue))
            {
                settingsFile = tempValue.GetString();
            }
        }

        if (filter.Length is 0 || filter.Contains(nameof(MTPackageMetadata.ExecutableFile), StringComparer.OrdinalIgnoreCase))
        {
            if (jsonObject.TryGetValue(nameof(MTPackageMetadata.ExecutableFile), out tempValue))
            {
                executableFile = tempValue.GetString();
            }
        }

        if (filter.Length is 0 || filter.Contains(nameof(MTPackageMetadata.ProcStartupArgs), StringComparer.OrdinalIgnoreCase))
        {
            if (jsonObject.TryGetValue(nameof(MTPackageMetadata.ProcStartupArgs), out tempValue))
            {
                procStartupArgs = tempValue.GetString();
            }
        }

        if (filter.Length is 0 || filter.Contains(nameof(MTPackageMetadata.ProcTerminateArgs), StringComparer.OrdinalIgnoreCase))
        {
            if (jsonObject.TryGetValue(nameof(MTPackageMetadata.ProcTerminateArgs), out tempValue))
            {
                procTerminateArgs = tempValue.GetString();
            }
        }

        if (filter.Length is 0 || filter.Contains(nameof(MTPackageMetadata.OtherStartupArgs), StringComparer.OrdinalIgnoreCase))
        {
            if (jsonObject.TryGetValue(nameof(MTPackageMetadata.OtherStartupArgs), out tempValue))
            {
                startUpArgs = GetStartUpArgs(tempValue).ToArray();
            }
        }

        return new MTPackageMetadata(author, version, settingsFile, executableFile, procStartupArgs, procTerminateArgs, startUpArgs);
    }

    private async Task TryCleanupMounterRootFolderAsync()
    {
        var targetFolderName = _mounterSettings.WorkingDirectory;

        var rootFolder = await GetMounterRootFolderAsync();
        foreach (var item in await rootFolder.GetItemsAsync())
        {
            /// 当匹配到与工作目录相同的文件夹时，跳过删除操作
            /// 除此之外，其余的所有文件/文件夹都将被删除。
            if (StringComparer.Ordinal.Equals(item.Name, targetFolderName) && item.IsOfType(StorageItemTypes.Folder))
            {
                continue;
            }

            try
            {
                await item.DeleteAsync();
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogWarning($"MounterService::TryClearMounterRootFolderAsync -> Failed to delete storage item ({item.Name}): \"{excp.Message}\".");
            }
        }
    }

    private async Task<MTPackageMetadata> TryGetLocalMTPackageMetadataAsync(StorageFolder workingDirectory, params string[] filter)
    {
        try
        {
            return await GetLocalMTPackageMetadataAsync(workingDirectory, filter);
        }
        catch
        {
            return MTPackageMetadata.Empty;
        }
    }

    private async Task<StorageFolder?> TryGetMounterWorkingDirectoryAsync()
    {
        try
        {
            return await GetMounterWorkingDirectoryAsync();
        }
        catch
        {
            return null;
        }
    }

    private readonly struct MTPackageManifest
    {
        public static readonly MTPackageManifest Empty = new();

        public readonly string Path;

        public readonly string Version;

        public readonly bool IsEmpty;

        public MTPackageManifest()
        {
            Path = string.Empty;
            Version = string.Empty;
            IsEmpty = true;
        }

        public MTPackageManifest(string path, string version)
        {
            Path = path;
            Version = version;
            IsEmpty = false;
        }
    }
}
