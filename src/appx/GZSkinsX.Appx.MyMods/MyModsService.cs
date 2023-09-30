// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.MyMods;

namespace GZSkinsX.Appx.MyMods;

/// <inheritdoc cref="IMyModsService"/>
[Shared, Export(typeof(IMyModsService))]
internal sealed partial class MyModsService : IMyModsService
{
    private const string MT_SETTINGS_KEY = "4D418624-AB90-4A4E-9264-ACBC7BCB9716";
    private const string MT_SETTINGS_ENABLE_NAME = "Enable";
    private const string MT_SETTINGS_BLOOD_NAME = "Blood";
    private const string MT_SETTINGS_CUSTOMINSTALL_NAME = "CustomInstall";
    private const string MT_SETTINGS_RGZINSTALL_NAME = "RGZInstall";
    private const string MT_SETTINGS_FILETABLE_NAME = "RGZFileTable";
    private const string LOLGEZI_EXTENSION_NAME = ".lolgezi";

    /// <summary>
    /// 获取本地的图片缓存文件夹路径。
    /// </summary>
    private string MyImageCacheFolder { get; } = Path.Combine(AppxContext.LocalCacheFolder, "Local", "MyMods", "Images");

    /// <summary>
    /// 获取默认的模组图片路径。
    /// </summary>
    private Uri DefaultImageUri { get; } = new("ms-appx:///Assets/Images/Mod_Preview_Empty.png");

    /// <summary>
    /// 用于暂存模组文件的安装列表，用于快速访问和写出。
    /// </summary>
    private HashSet<string> InstalledMods { get; } = new();

    /// <inheritdoc/>
    public async Task ClearAllInstalledAsync()
    {
        InstalledMods.Clear();

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.Appx.MyMods.MyModsService.ClearAllInstalled",
            "Successfully cleaned all installed mods.");

        await UpdateSettingsAsync();
    }

    /// <inheritdoc/>
    public bool IsInstalled(string filename)
    {
        filename = Path.GetFileNameWithoutExtension(filename);
        var result = InstalledMods.Contains(filename);

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Appx.MyMods.MyModsService.IsInstalled",
            $"The mod \"{filename}\" is {(result ? "installed" : "uninstalled")}.");

        return result;
    }

    /// <inheritdoc/>
    public int IndexOfTable(string filename)
    {
        if (InstalledMods.Count is 0)
        {
            return -1;
        }

        var result = 0;
        filename = Path.GetFileNameWithoutExtension(filename);
        using var enumerator = InstalledMods.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (StringComparer.Ordinal.Equals(enumerator.Current, filename))
            {
                AppxContext.LoggingService.LogAlways(
                    "GZSkinsX.Appx.MyMods.MyModsService.IndexOfTable",
                    $"The index of the mod \"{filename}\" is \"{result}\".");

                return result;
            }

            result++;
        }

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Appx.MyMods.MyModsService.IndexOfTable",
            $"The mod \"{filename}\" does not exist in the table (-1).");

        return -1;
    }

    /// <inheritdoc/>
    public async Task ImportModsAsync(params string[] files)
    {
        await ImportModsCoreAsync(files, false);
    }

    /// <inheritdoc/>
    public async Task ImportModsAsync(bool overwrite, params string[] files)
    {
        await ImportModsCoreAsync(files, overwrite);
    }

    /// <inheritdoc/>
    public async Task ImportModsAsync(IEnumerable<string> files)
    {
        await ImportModsCoreAsync(files, false);
    }

    /// <inheritdoc/>
    public async Task ImportModsAsync(IEnumerable<string> files, bool overwrite)
    {
        await ImportModsCoreAsync(files, overwrite);
    }

    /// <inheritdoc cref="ImportModsAsync(IEnumerable{string}, bool)"/>
    private async Task ImportModsCoreAsync(IEnumerable<string> files, bool overwrite)
    {
        var modsFolder = await GetModsFolderAsync();
        if (string.IsNullOrWhiteSpace(modsFolder))
        {
            return;
        }

        if (Directory.Exists(modsFolder) is false)
        {
            Directory.CreateDirectory(modsFolder);
        }

        foreach (var item in files)
        {
            var fileInfo = new FileInfo(item);
            if (fileInfo.Exists is false)
            {
                continue;
            }

            var destFilename = StringComparer.OrdinalIgnoreCase.Equals(fileInfo.Extension, LOLGEZI_EXTENSION_NAME)
                ? fileInfo.Name : Path.GetFileNameWithoutExtension(fileInfo.FullName) + LOLGEZI_EXTENSION_NAME;

            var destFilePath = Path.Combine(modsFolder, destFilename);
            if (File.Exists(destFilePath) && overwrite is false)
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.Appx.MyMods.MyModsService.ImportModsCoreAsync",
                    $"A mod file with the same name already exists \"{destFilename}\", skip overwrite.");

                continue;
            }

            try
            {
                ReadModInfo(item);
                File.Copy(item, destFilename, true);
                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.Appx.MyMods.MyModsService.ImportModsCore",
                    $"The mod file \"{destFilename}\" have been imported.");
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MyMods.MyModsService.ImportModsCore",
                    $"""
                    Failed to import the file "{fileInfo.Name}".
                    {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}.
                    """);
            }
        }
    }

    /// <inheritdoc/>
    public async Task InstallModsAsync(params string[] files)
    {
        await InstallModsCoreAsync(files);
    }

    /// <inheritdoc/>
    public async Task InstallModsAsync(IEnumerable<string> files)
    {
        await InstallModsCoreAsync(files);
    }

    /// <inheritdoc cref="InstallModsAsync(IEnumerable{string})"/>
    private async Task InstallModsCoreAsync(IEnumerable<string> files)
    {
        var modsFolder = await GetModsFolderAsync();
        if (Directory.Exists(modsFolder) is false)
        {
            return;
        }

        foreach (var item in files)
        {
            var displayName = Path.GetFileNameWithoutExtension(item);
            InstalledMods.Remove(displayName);

            if (File.Exists(item) is false)
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.Appx.MyMods.MyModsService.InstallModsCoreAsync",
                    $"The mod file does not exist in the mods folder {displayName}.");

                continue;
            }

            InstalledMods.Add(displayName);

            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.MyMods.MyModsService.InstallModsCoreAsync",
                $"The mod file \"{displayName}\" have been installed.");
        }

        await UpdateSettingsAsync();
    }

    /// <inheritdoc/>
    public async Task UninstallModsAsync(params string[] files)
    {
        await UninstallModsCoreAsync(files);
    }

    /// <inheritdoc/>
    public async Task UninstallModsAsync(IEnumerable<string> files)
    {
        await UninstallModsCoreAsync(files);
    }

    /// <inheritdoc cref="UninstallModsAsync(IEnumerable{string})"/>
    private async Task UninstallModsCoreAsync(IEnumerable<string> files)
    {
        foreach (var item in files)
        {
            var displayName = Path.GetFileNameWithoutExtension(item);
            InstalledMods.Remove(displayName);

            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.MyMods.MyModsService.UninstallModsCoreAsync",
                $"The mod \"{displayName}\" have been uninstalled.");
        }

        await UpdateSettingsAsync();
    }

    /// <inheritdoc/>
    public Uri GetModImage(string filePath)
    {
        try
        {
            var imageCacheFolder = MyImageCacheFolder;
            if (Directory.Exists(imageCacheFolder) is false)
            {
                Directory.CreateDirectory(imageCacheFolder);
            }

            var imageFilePath = Path.Combine(imageCacheFolder, Path.GetFileNameWithoutExtension(filePath));
            AppxContext.KernelService.ExtractWGZModImage(filePath, imageFilePath);
            return new(imageFilePath, UriKind.Absolute);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MyMods.MyModsService.GetModImage",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}");

            return DefaultImageUri;
        }
    }

    /// <inheritdoc/>
    public MyModInfo ReadModInfo(string filePath)
    {
        var wgzModInfo = AppxContext.KernelService.ReadWGZModInfo(filePath);
        return new(wgzModInfo.Name, wgzModInfo.Author, wgzModInfo.Description, wgzModInfo.DateTime, new(filePath));
    }

    /// <inheritdoc/>
    public MyModInfo? TryReadModInfo(string filePath)
    {
        try
        {
            var wgzModInfo = AppxContext.KernelService.ReadWGZModInfo(filePath);
            return new(wgzModInfo.Name, wgzModInfo.Author, wgzModInfo.Description, wgzModInfo.DateTime, new(filePath));
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MyMods.MyModsService.TryReadModInfo",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}");

            return null;
        }
    }

    /// <summary>
    /// 对传入的已加密的字符串进行解密并返回模组文件列表。
    /// </summary>
    /// <param name="encryptedStr">加密后的模组文件列表字符串。</param>
    private static IEnumerable<string> ParseRGZFileTable(string? encryptedStr)
    {
        if (string.IsNullOrWhiteSpace(encryptedStr))
        {
            yield break;
        }

        var result = AppxContext.KernelService.DecryptConfigText(encryptedStr);
        if (string.IsNullOrWhiteSpace(result))
        {
            yield break;
        }

        foreach (var item in result.Split(';', StringSplitOptions.None))
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                continue;
            }

            yield return item;
        }
    }

    /// <inheritdoc/>
    public async Task RefreshAsync()
    {
        InstalledMods.Clear();

        var settingsFile = await TryGetMTSettingsFilePathAsync();
        if (string.IsNullOrWhiteSpace(settingsFile))
        {
            return;
        }

        var settingsRoot = await TryGetMTSettingsRootAsync(settingsFile);
        if (settingsRoot is not null)
        {
            var settingsData = GetOrCreateSettingsData(settingsRoot);
            if (settingsData.GZSkins is null)
            {
                BuildMTSettingsData(settingsData);
                Debug.Assert(settingsData.GZSkins is not null);
                await UpdateSettingsCoreAsync(settingsFile, settingsRoot, settingsData);
            }

            if (settingsData.GZSkins.TryGetValue(MT_SETTINGS_FILETABLE_NAME, out var fileTable))
            {
                foreach (var item in ParseRGZFileTable(GetStringFromJson(fileTable)))
                {
                    InstalledMods.Add(item);
                }
            }
        }
        else
        {
            // 如果该配置根节点为空，则创建并写入新的配置文件。
            // 并避免当配置文件不存在时，无法从配置文件中检索配置属性内容。
            var settingsData = GetOrCreateSettingsData(settingsRoot = new());
            await UpdateSettingsCoreAsync(settingsFile, settingsRoot, settingsData);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateSettingsAsync()
    {
        var settingsFile = await TryGetMTSettingsFilePathAsync();
        if (settingsFile is null)
        {
            return;
        }

        var settingsRoot = await TryGetMTSettingsRootAsync(settingsFile) ?? new();
        var settingsData = GetOrCreateSettingsData(settingsRoot);
        await UpdateSettingsCoreAsync(settingsFile, settingsRoot, settingsData);
    }
}
