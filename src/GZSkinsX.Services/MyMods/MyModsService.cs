// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Mounter;
using GZSkinsX.Contracts.MyMods;

using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace GZSkinsX.Services.MyMods;

[Shared, Export(typeof(IMyModsService))]
[method: ImportingConstructor]
internal sealed class MyModsService(MyModsSettings myModSettings) : IMyModsService
{
    private const string MT_SETTINGS_KEY = "4D418624-AB90-4A4E-9264-ACBC7BCB9716";
    private const string FA_TOKEN_MODSFOLDER = "MyModService_ModsFolder";
    private const string FA_TOKEN_WADSFOLDER = "MyModService_WadsFolder";

    private readonly MyModsSettings _myModSettings = myModSettings;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    private readonly UTF8Encoding _utf8EncodingWithOutBOM = new(false);

    private HashSet<string> InstalledMods { get; } = new();

    public async Task ClearAllInstalledAsync()
    {
        InstalledMods.Clear();

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.Services.MyModsService.ClearAllInstalledAsync",
            "Successfully cleaned all installed mods.");

        await UpdateSettingsAsync();
    }

    public bool IsInstalled(StorageFile storageFile)
    {
        var result = InstalledMods.Contains(storageFile.DisplayName);

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Services.MyModsService.IsInstalled",
            $"The mod \"{storageFile.DisplayName}\" is {(result ? "installed" : "uninstalled")}.");

        return result;
    }

    public int IndexOfTable(StorageFile storageFile)
    {
        var result = 0;

        using var enumerator = InstalledMods.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (StringComparer.Ordinal.Equals(enumerator.Current, storageFile.DisplayName))
            {
                break;
            }

            result++;
        }

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Services.MyModsService.IndexOfTable",
            $"The index of the mod \"{storageFile.DisplayName}\" is {result}.");

        return result;
    }

    public async Task ImportModsAsync(params StorageFile[] storageFiles)
    {
        await ImportModsCoreAsync(storageFiles);
    }

    public async Task ImportModsAsync(IEnumerable<StorageFile> storageFiles)
    {
        await ImportModsCoreAsync(storageFiles);
    }

    private async Task ImportModsCoreAsync(IEnumerable<StorageFile> storageFiles)
    {
        var modsFolder = await GetModsFolderAsync();
        foreach (var file in storageFiles)
        {
            if (await modsFolder.TryGetItemAsync(file.Name) is StorageFile existsFile)
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.Services.MyModsService.ImportModsCoreAsync",
                    $"A mod file with the same name already exists \"{existsFile.Name}\", skip import.");
            }

            try
            {
                await ReadModInfoAsync(file);

                var newFile = await file.CopyAsync(modsFolder);
                if (Path.GetExtension(newFile.Name) != ".lolgezi")
                {
                    // Fix extension name
                    await newFile.RenameAsync(Path.GetFileName(newFile.Name) + ".lolgezi");
                }

                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.Services.MyModsService.ImportModsCoreAsync",
                    $"The mod file \"{newFile.Name}\" have been imported.");
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Services.MyModsService.ImportModsCoreAsync",
                    $"""
                    Failed to import the file "{file.Name}".
                    {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}.
                    """);
            }
        }
    }

    public async Task InstallModsAsync(params StorageFile[] storageFiles)
    {
        await InstallModsCoreAsync(storageFiles);
    }

    public async Task InstallModsAsync(IEnumerable<StorageFile> storageFiles)
    {
        await InstallModsCoreAsync(storageFiles);
    }

    private async Task InstallModsCoreAsync(IEnumerable<StorageFile> storageFiles)
    {
        var modsFolder = await GetModsFolderAsync();
        foreach (var file in storageFiles)
        {
            InstalledMods.Remove(file.DisplayName);

            if (await modsFolder.TryGetItemAsync(file.Name) is not StorageFile)
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.Services.MyModsService.InstallModsCoreAsync",
                    $"The mod file does not exist in the mods folder {file.Name}.");

                continue;
            }

            InstalledMods.Add(file.DisplayName);

            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Services.MyModsService.InstallModsCoreAsync",
                $"The mod file \"{file.Name}\" have been installed.");
        }

        await UpdateSettingsAsync();
    }

    public async Task UninstallModsAsync(params StorageFile[] storageFiles)
    {
        await UninstallModsCoreAsync(storageFiles);
    }

    public async Task UninstallModsAsync(IEnumerable<StorageFile> storageFiles)
    {
        await UninstallModsCoreAsync(storageFiles);
    }

    private async Task UninstallModsCoreAsync(IEnumerable<StorageFile> storageFiles)
    {
        foreach (var file in storageFiles)
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Services.MyModsService.UninstallModsCoreAsync",
                $"The mod \"{file.DisplayName}\" have been uninstalled.");

            InstalledMods.Remove(file.DisplayName);
        }

        await UpdateSettingsAsync();
    }

    private async Task<StorageFolder> GetWGZSubFolderAsync(string name, string faToken)
    {
        var targetFolder = await AppxContext.FutureAccessService.TryGetFolderAsync(faToken);
        if (targetFolder is null)
        {
            var gameFolder = AppxContext.GameService.GameData.GameFolder ?? throw new InvalidOperationException();
            var rootFolder = await gameFolder.CreateFolderAsync("GeziSkin", CreationCollisionOption.OpenIfExists);

            targetFolder = await rootFolder.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
            AppxContext.FutureAccessService.Add(targetFolder, faToken);
        }

        return targetFolder;
    }

    public Task<bool> GetIsEnableBloodAsync()
    {
        return Task.FromResult(_myModSettings.EnableBlood);
    }

    public async Task<StorageFolder> GetModsFolderAsync()
    {
        return await GetWGZSubFolderAsync("Mods", FA_TOKEN_MODSFOLDER);
    }

    public async Task<StorageFolder> GetWadsFolderAsync()
    {
        return await GetWGZSubFolderAsync("Wads", FA_TOKEN_WADSFOLDER);
    }

    public async Task SetIsEnableBloodAsync(bool isEnable)
    {
        if (_myModSettings.EnableBlood == isEnable)
        {
            return;
        }

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Services.MyModsService.SetIsEnableBloodAsync",
            $"The blood is {(isEnable ? "enabled" : "disabled")}.");

        _myModSettings.EnableBlood = isEnable;
        await UpdateSettingsAsync();
    }

    public async Task SetModsFolderAsync(StorageFolder storageFolder)
    {
        AppxContext.FutureAccessService.Add(storageFolder, FA_TOKEN_MODSFOLDER);

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Services.MyModsService.SetModsFolderAsync",
            $"The mods folder have been changed, new folder: \"{storageFolder.Path}\".");

        await UpdateSettingsAsync();
    }

    public async Task SetWadsFolderAsync(StorageFolder storageFolder)
    {
        AppxContext.FutureAccessService.Add(storageFolder, FA_TOKEN_WADSFOLDER);

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Services.MyModsService.SetWadsFolderAsync",
            $"The wads folder have been changed, new folder: \"{storageFolder.Path}\".");

        await UpdateSettingsAsync();
    }

    public async Task<BitmapImage> GetModImageAsync(StorageFile storageFile)
    {
        static async Task<StorageFolder> GetImageCacheFolderAsync()
        {
            return await (await (await ApplicationData.Current.LocalCacheFolder
                .CreateFolderAsync("Local", CreationCollisionOption.OpenIfExists))
                .CreateFolderAsync("MyMods", CreationCollisionOption.OpenIfExists))
                .CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
        }

        try
        {
            var localCacheFolder = await GetImageCacheFolderAsync();

            var imageFilePath = Path.Combine(localCacheFolder.Path, storageFile.DisplayName);
            await AppxContext.DesktopExtensionService.ExtractModImageAsync(storageFile.Path, imageFilePath);

            return new BitmapImage(new Uri(imageFilePath, UriKind.Absolute));
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Services.MyModsService.GetModImageAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}");

            return new BitmapImage(new Uri("ms-appx:///Assets/Images/Mod_Preview_Empty.png"));
        }
    }

    public async Task<MyModInfo> ReadModInfoAsync(StorageFile storageFile)
    {
        return await AppxContext.DesktopExtensionService.ReadModInfoAsync(storageFile.Path);
    }

    public async Task<MyModInfo?> TryReadModInfoAsync(StorageFile storageFile)
    {
        MyModInfo? result;
        try
        {
            result = await AppxContext.DesktopExtensionService.ReadModInfoAsync(storageFile.Path);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Services.MyModsService.TryReadModInfoAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}");

            result = null;
        }

        return result;
    }

    private async Task<string> TryGetSettingsFilePathAsync()
    {
        var workingDirectory = await AppxContext.MounterService.TryGetMounterWorkingDirectoryAsync();
        if (workingDirectory is null)
        {
            return string.Empty;
        }

        var metadata = await AppxContext.MounterService.TryGetCurrentPackageMetadataAsync(nameof(MTPackageMetadata.SettingsFile));
        if (metadata.IsEmpty)
        {
            return string.Empty;
        }

        return Path.Combine(workingDirectory.Path, metadata.SettingsFile);
    }

    private async Task<MTSettingsRoot?> TryGetSettingsRootAsync()
    {
        return await TryGetSettingsRootAsync(await TryGetSettingsFilePathAsync());
    }

    private async Task<MTSettingsRoot?> TryGetSettingsRootAsync(string settingsFilePath)
    {
        if (File.Exists(settingsFilePath) is false)
        {
            return null;
        }

        using var fileStream = new FileStream(settingsFilePath, FileMode.Open, FileAccess.Read);
        if (await JsonSerializer.DeserializeAsync<MTSettingsRoot>(fileStream) is { } settingsRoot)
        {
            return settingsRoot;
        }

        return null;
    }

    private async Task<MTSettingsData?> TryGetSettingsDataAsync()
    {
        var settingsRoot = await TryGetSettingsRootAsync();
        if (settingsRoot is null)
        {
            return null;
        }

        if (settingsRoot.Data.FirstOrDefault(a => StringComparer.OrdinalIgnoreCase.Equals(a.Key, MT_SETTINGS_KEY)) is not { } settingsData)
        {
            return null;
        }

        return settingsData;
    }

    private async IAsyncEnumerable<string> TryParseRGZFileTableAsync(object fileTable)
    {
        string str;
        if (fileTable is JsonElement elem && elem.ValueKind is JsonValueKind.String)
        {
            str = elem.GetString() ?? string.Empty;
        }
        else
        {
            str = fileTable is string s ? s : fileTable.ToString();
        }

        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            yield break;
        }

        var result = await AppxContext.DesktopExtensionService.DecryptConfigTextAsync(str);
        if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
        {
            yield break;
        }

        foreach (var item in result.Split(';', StringSplitOptions.None))
        {
            if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
            {
                continue;
            }

            yield return item;
        }
    }

    private async Task RefreshInstalledModsAsync()
    {
        InstalledMods.Clear();

        var settinsData = await TryGetSettingsDataAsync();
        if (settinsData is null || settinsData.GZSkins is null)
        {
            return;
        }

        if (settinsData.GZSkins.TryGetValue("RGZFileTable", out var fileTable))
        {
            await foreach (var item in TryParseRGZFileTableAsync(fileTable))
            {
                InstalledMods.Add(item);
            }
        }
    }

    public async Task RefreshAsync()
    {
        await RefreshInstalledModsAsync();
    }

    public async Task UpdateSettingsAsync()
    {
        var modsFolder = await GetModsFolderAsync();
        var wadsFolder = await GetWadsFolderAsync();

        var settingsFilePath = await TryGetSettingsFilePathAsync();
        var settingsRoot = await TryGetSettingsRootAsync(settingsFilePath) ?? new MTSettingsRoot();

        settingsRoot.Data ??= new List<MTSettingsData>();
        if (settingsRoot.Data.FirstOrDefault(a => StringComparer.OrdinalIgnoreCase.Equals(a.Key, MT_SETTINGS_KEY)) is not { } settingsData)
        {
            settingsRoot.Data.Add(settingsData = new MTSettingsData() { Key = MT_SETTINGS_KEY });
        }

        var stringBuilder = new StringBuilder();
        foreach (var item in InstalledMods)
        {
            if (await modsFolder.TryGetItemAsync(item + ".lolgezi") is not StorageFile)
            {
                continue;
            }

            stringBuilder.Append(item);
            stringBuilder.Append(';');
        }

#pragma warning disable format
        settingsData.GZSkins ??= new();
        settingsData.GZSkins["Enable"]          = true;
        settingsData.GZSkins["Blood"]           = _myModSettings.EnableBlood;
        settingsData.GZSkins["CustomInstall"]   = wadsFolder.Path + Path.DirectorySeparatorChar;
        settingsData.GZSkins["RGZInstall"]      = modsFolder.Path + Path.DirectorySeparatorChar;
        settingsData.GZSkins["RGZFileTable"]    = await AppxContext.DesktopExtensionService.EncryptConfigTextAsync(stringBuilder.ToString());
#pragma warning restore format

        var parentFolder = Path.GetDirectoryName(settingsFilePath);
        if (Directory.Exists(parentFolder) is false)
        {
            Directory.CreateDirectory(parentFolder);
        }

        try
        {
            File.WriteAllText(settingsFilePath, JsonSerializer.Serialize(settingsRoot, _jsonSerializerOptions), _utf8EncodingWithOutBOM);

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.Services.MyModsService.UpdateSettingsAsync",
                "Successfully update the mods configuration file.");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Services.MyModsService.UpdateSettingsAsync",
                $"""
                Failed to update the mods configuration file.
                {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}.
                """);

            throw;
        }
    }

    private sealed class MTSettingsData
    {
        public string? Key
        {
            get; set;
        }

        public Dictionary<string, object>? GZSkins
        {
            get; set;
        }

        public Dictionary<string, object>? SkinsData
        {
            get; set;
        }

        public Dictionary<string, object>? Extra
        {
            get; set;
        }
    }

    private sealed class MTSettingsRoot
    {
        public IList<MTSettingsData>? Data
        {
            get; set;
        }
    }
}
