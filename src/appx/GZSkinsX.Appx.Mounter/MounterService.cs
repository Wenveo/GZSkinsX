// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Mounter;

using Windows.Data.Json;
using Windows.Foundation;
using Windows.Storage;

namespace GZSkinsX.Appx.Mounter;

/// <inheritdoc cref="IMounterService"/>
[Shared, Export(typeof(IMounterService))]
internal sealed class MounterService : IMounterService
{
    private static Uri[] OnlineManifests { get; } = new Uri[]
    {
        new Uri("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/PackageManifest.json"),
        new Uri("http://x1.gzskins.com/MounterV3/PackageManifest.json")
    };

    private readonly MounterSettings _mounterSettings;

    private readonly HttpClient _httpClient;

    /// <inheritdoc/>
    public bool IsMTRunning
    {
        get
        {
            using var handle =
                Windows.Win32.PInvoke.OpenFileMapping(
                0xF001F, false, "Gz_services:execute");

            return handle.IsInvalid is false;
        }
    }

    /// <inheritdoc/>
    public event TypedEventHandler<IMounterService, bool>? IsRunningChanged;

    [ImportingConstructor]
    public MounterService(MounterSettings mounterSettings)
    {
        _mounterSettings = mounterSettings;
        _httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
        });

        var worker = new BackgroundWorker();
        worker.DoWork += DoSomething;
        worker.RunWorkerAsync();
    }

    // ヾ(•ω•`)o
    private async void DoSomething(object? sender, DoWorkEventArgs e)
    {
        async Task CheckExitAsync()
        {
            while (true)
            {
                await Task.Delay(1000);

                try
                {
                    if (IsMTRunning is false)
                    {
                        IsRunningChanged?.Invoke(this, false);
                        AppxContext.LoggingService.LogAlways("GZSkinsX.Services.MounterService.CheckExitAsync", "MotClientAgent has exited.");
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
                await Task.Delay(1000);

                try
                {
                    if (IsMTRunning)
                    {
                        IsRunningChanged?.Invoke(this, true);
                        AppxContext.LoggingService.LogAlways("GZSkinsX.Services.MounterService.CheckRunAsync", "MotClientAgent is running.");
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
                if (IsMTRunning)
                {
                    IsRunningChanged?.Invoke(this, true);
                    AppxContext.LoggingService.LogAlways("GZSkinsX.Services.MounterService.DoSomething", "MotClientAgent is running.");
                    await CheckExitAsync();
                }
                else
                {
                    IsRunningChanged?.Invoke(this, false);
                    AppxContext.LoggingService.LogAlways("GZSkinsX.Services.MounterService.DoSomething", "MotClientAgent is not running.");
                    await CheckRunAsync();
                }
            }
            catch
            {
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CheckForUpdatesAsync()
    {
        var metadata = await TryGetCurrentPackageMetadataAsync();
        if (metadata.IsEmpty is false)
        {
            try
            {
                var onlineManifest = await DownloadPackageManifestAsync();
                if (StringComparer.Ordinal.Equals(metadata.Version, onlineManifest.Version))
                {
                    AppxContext.LoggingService.LogOkay("GZSkinsX.Services.MounterService.CheckForUpdatesAsync", $"Now is uptodate \"{metadata.Version}\".");
                    return false;
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Services.MounterService.CheckForUpdatesAsync",
                    $"""
                    Failed to check updates.
                    {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}
                    """);

                throw;
            }
        }

        AppxContext.LoggingService.LogAlways("GZSkinsX.Services.MounterService.CheckForUpdatesAsync", $"Attention needed.");
        return true;
    }

    /// <inheritdoc/>
    public async Task<MTPackageMetadata> GetCurrentPackageMetadataAsync(params string[] filter)
    {
        return await GetLocalMTPackageMetadataAsync(await GetMounterWorkingDirectoryAsync(), filter);
    }

    /// <inheritdoc/>
    public async Task<StorageFolder> GetMounterWorkingDirectoryAsync()
    {
        var rootFolder = await GetMounterRootFolderAsync();
        return await rootFolder.GetFolderAsync(_mounterSettings.WorkingDirectory);
    }

    /// <inheritdoc/>
    public async Task LaunchAsync()
    {
        var workingDirectory = await GetMounterWorkingDirectoryAsync();

        var localPackageMetadata =
            await GetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile),
                nameof(MTPackageMetadata.ProcStartupArgs));

        var executableFile = Path.Combine(workingDirectory.Path, localPackageMetadata.ExecutableFile);
        try
        {
            ProcessLaunch(executableFile, localPackageMetadata.ProcStartupArgs);
            AppxContext.LoggingService.LogOkay("GZSkinsX.Services.MounterService.LaunchAsync", "Launch successfully.");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Services.MounterService.LaunchAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task LaunchAsync(string args)
    {
        var workingDirectory = await GetMounterWorkingDirectoryAsync();

        var localPackageMetadata =
            await GetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile));

        var executableFile = Path.Combine(workingDirectory.Path, localPackageMetadata.ExecutableFile);

        try
        {
            ProcessLaunch(executableFile, args);
            AppxContext.LoggingService.LogOkay("GZSkinsX.Services.MounterService.LaunchAsync2", "Launch successfully.");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Services.MounterService.LaunchAsync2",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task TerminateAsync()
    {
        var workingDirectory = await GetMounterWorkingDirectoryAsync();

        var localPackageMetadata =
            await GetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile),
                nameof(MTPackageMetadata.ProcTerminateArgs));

        var executableFile = Path.Combine(workingDirectory.Path, localPackageMetadata.ExecutableFile);
        try
        {
            ProcessLaunch(executableFile, localPackageMetadata.ProcTerminateArgs);
            AppxContext.LoggingService.LogOkay("GZSkinsX.Services.MounterService.TerminateAsync", "Terminate successfully.");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Services.MounterService.TerminateAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

            throw;
        }
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<StorageFolder?> TryGetMounterWorkingDirectoryAsync()
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

    /// <inheritdoc/>
    public async Task UpdateAsync(IProgress<double>? progress = null)
    {
        var workingDirectory = await TryGetMounterWorkingDirectoryAsync();
        var previousMetadata = workingDirectory is not null ? await TryGetLocalMTPackageMetadataAsync(workingDirectory,
            nameof(MTPackageMetadata.Version), nameof(MTPackageMetadata.SettingsFile)) : MTPackageMetadata.Empty;

        var onlineManifest = await DownloadPackageManifestAsync(new((value) => progress?.Report(value * 6)));
        if (StringComparer.Ordinal.Equals(previousMetadata.Version, onlineManifest.Version))
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Services.MounterService.UpdateAsync",
                "This component is already up to date.");
        }
        else
        {
            var destFolder = await DownloadMTPackageAsync(new(onlineManifest.Path), new((value) =>
            {
                progress?.Report(6 + value * 94);
            }));

            // New Metadata
            var newMetadata = await GetLocalMTPackageMetadataAsync(destFolder, nameof(MTPackageMetadata.SettingsFile));

            // Copy settings file
            if (workingDirectory is not null && previousMetadata.IsEmpty is false)
            {
                var settingsFilePath = Path.Combine(workingDirectory.Path, previousMetadata.SettingsFile);
                if (File.Exists(settingsFilePath))
                {
                    File.Copy(settingsFilePath, Path.Combine(destFolder.Path, newMetadata.SettingsFile), true);
                }
            }

            _mounterSettings.WorkingDirectory = destFolder.Name;

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.Services.MounterService.UpdateAsync",
                $"The component has been updated to \"{onlineManifest.Version}\".");
        }

        await TryCleanupMounterRootFolderAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> VerifyContentIntegrityAsync()
    {
        var workingDirectory = await TryGetMounterWorkingDirectoryAsync();
        if (workingDirectory is null)
        {
            return false;
        }

        if (await workingDirectory.TryGetItemAsync("_metadata") is not StorageFolder metadataFolder)
        {
            return false;
        }

        if (await metadataFolder.TryGetItemAsync("blockmap.json") is not StorageFile metadataFile)
        {
            return false;
        }

        if (JsonObject.TryParse(await FileIO.ReadTextAsync(metadataFile), out var blockMapJson) is false ||
            blockMapJson.TryGetValue("Blocks", out var blocksArray) is false || blocksArray.ValueType is not JsonValueType.Array)
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
                AppxContext.LoggingService.LogError(
                    "MounterService::VerifyLocalMTPackageIntegrityAsync",
                    $"""
                    Failed to calculate file checksum "{path}".
                    {excp}: "{excp.Message}" {Environment.NewLine}{excp.StackTrace}".
                    """);

                return false;
            }
        }

        return true;
    }

    private static async Task<StorageFolder> CreateAnEmptyFolderAsync(StorageFolder relativeTo, string folderName)
    {
        var targetFolder = await relativeTo.TryGetItemAsync(folderName);
        if (targetFolder is not null && targetFolder.IsOfType(StorageItemTypes.Folder))
        {
            await targetFolder.DeleteAsync();
        }

        return await relativeTo.CreateFolderAsync(folderName);
    }

    private async Task<MTPackageManifest> DownloadPackageManifestAsync(Progress<double>? progress = null)
    {
        var temp = await GetTemporaryFileAsync();

        foreach (var uri in OnlineManifests)
        {
            try
            {
                using (var outputStream = await temp.OpenStreamForWriteAsync())
                {
                    await _httpClient.DownloadAsync(uri, outputStream, progress);
                }

                var content = await FileIO.ReadTextAsync(temp);
                var jsonObject = JsonObject.Parse(content);

                await temp.DeleteAsync();

                return new MTPackageManifest(
                    jsonObject[nameof(MTPackageManifest.Path)].GetString(),
                    jsonObject[nameof(MTPackageManifest.Version)].GetString());
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Services.MounterService.DownloadPackageManifestAsync",
                    $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

                continue;
            }
        }

        throw new IndexOutOfRangeException();
    }

    private async Task<StorageFolder> DownloadMTPackageAsync(Uri requestUri, Progress<double>? progress = null)
    {
        var temp = await GetTemporaryFileAsync();

        using (var outputStream = await temp.OpenStreamForWriteAsync())
        {
            await _httpClient.DownloadAsync(requestUri, outputStream, progress);
        }

        var rootFolder = await GetMounterRootFolderAsync();
        var destFolder = await CreateAnEmptyFolderAsync(rootFolder, Guid.NewGuid().ToString());

        try
        {
            ZipFile.ExtractToDirectory(temp.Path, destFolder.Path);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Services.MounterService.DownloadMTPackageAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

            throw;
        }
        finally
        {
            await temp.DeleteAsync();
        }

        return destFolder;
    }

    private static async Task<MTPackageMetadata> GetLocalMTPackageMetadataAsync(StorageFolder workingDirectory, params string[] filter)
    {
        var metadataFolder = await workingDirectory.GetFolderAsync("_metadata");
        var metadataFile = await metadataFolder.GetFileAsync("package.json");

        var content = await FileIO.ReadTextAsync(metadataFile, default);
        return ParseMetadataFromString(content, filter);
    }

    private static async Task<StorageFolder> GetMounterRootFolderAsync()
    {
        return await ApplicationData.Current.RoamingFolder.CreateFolderAsync("Mounter", CreationCollisionOption.OpenIfExists);
    }

    private static async Task<StorageFile> GetTemporaryFileAsync()
    {
        return await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Guid.NewGuid().ToString());
    }

    private static MTPackageMetadata ParseMetadataFromString(string input, params string[] filter)
    {
        IEnumerable<MTPackageMetadataStartUpArgument> GetStartUpArgs(JsonObject json, string propName)
        {
            if (filter.Length is 0 || filter.Contains(propName, StringComparer.OrdinalIgnoreCase))
            {
                if (json.TryGetValue(propName, out var tempValue) is false || tempValue.ValueType is not JsonValueType.Array)
                {
                    yield break;
                }

                foreach (var item in tempValue.GetArray())
                {
                    if (item.ValueType is not JsonValueType.Object)
                    {
                        continue;
                    }

                    if (item.GetObject().TryGetValue("Name", out var name) is false || name.ValueType is not JsonValueType.String)
                    {
                        continue;
                    }

                    if (item.GetObject().TryGetValue("Value", out var value) is false || name.ValueType is not JsonValueType.String)
                    {
                        continue;
                    }

                    yield return new(name.GetString(), value.GetString());
                }
            }

            yield break;
        }

        string GetValueOrDefault(JsonObject json, string propName)
        {
            if (filter.Length is 0 || filter.Contains(propName, StringComparer.OrdinalIgnoreCase))
            {
                if (json.TryGetValue(propName, out var tempValue) && tempValue.ValueType is JsonValueType.String)
                {
                    return tempValue.GetString();
                }
            }

            return string.Empty;
        }

        if (JsonObject.TryParse(input, out var jsonObject) is false)
        {
            return MTPackageMetadata.Empty;
        }

        return new MTPackageMetadata(
            GetValueOrDefault(jsonObject, nameof(MTPackageMetadata.Author)),
            GetValueOrDefault(jsonObject, nameof(MTPackageMetadata.Version)),
            GetValueOrDefault(jsonObject, nameof(MTPackageMetadata.Description)),
            GetValueOrDefault(jsonObject, nameof(MTPackageMetadata.AboutTheAuthor)),
            GetValueOrDefault(jsonObject, nameof(MTPackageMetadata.SettingsFile)),
            GetValueOrDefault(jsonObject, nameof(MTPackageMetadata.ExecutableFile)),
            GetValueOrDefault(jsonObject, nameof(MTPackageMetadata.ProcStartupArgs)),
            GetValueOrDefault(jsonObject, nameof(MTPackageMetadata.ProcTerminateArgs)),
            GetStartUpArgs(jsonObject, nameof(MTPackageMetadata.ProcStartupArgs)).ToArray());
    }

    private static void ProcessLaunch(string filePath, string args)
    {
        Process.Start(new ProcessStartInfo()
        {
            Arguments = args,
            FileName = filePath,
            UseShellExecute = true,
            Verb = "RunAs",
            WorkingDirectory = Path.GetDirectoryName(filePath)
        });
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
                AppxContext.LoggingService.LogError(
                    "GZSkinsX::Services::MounterService::TryClearMounterRootFolderAsync",
                    $"Failed to delete the storage item ({item.Name}): \"{excp.Message}\".");
            }
        }
    }

    private static async Task<MTPackageMetadata> TryGetLocalMTPackageMetadataAsync(StorageFolder workingDirectory, params string[] filter)
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
