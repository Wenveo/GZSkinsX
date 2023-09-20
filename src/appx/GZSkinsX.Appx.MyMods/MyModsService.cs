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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

using GZSkinsX.Appx.MyMods;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Mounter;
using GZSkinsX.Contracts.MyMods;

using Windows.Storage;

namespace GZSkinsX.Services.MyMods;

/// <inheritdoc cref="IMyModsService"/>
[Shared, Export(typeof(IMyModsService))]
internal sealed class MyModsService : IMyModsService
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
    private string MyImageCacheFolder { get; } = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Local", "MyMods", "Images");

    /// <summary>
    /// 自定义的 Json 序列化配置选项。
    /// </summary>
    private JsonSerializerOptions MyJsonSerializerOptions { get; } = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
    };

    /// <summary>
    /// 用于暂存模组文件的安装列表，用于快速访问和写出。
    /// </summary>
    private HashSet<string> InstalledMods { get; } = new();

    /// <inheritdoc/>
    public async Task<bool> GetIsEnableBloodAsync()
    {
        var val = await TryGetMTSettingsValueAsync(MT_SETTINGS_BLOOD_NAME);
        if (val is JsonElement elem && elem.ValueKind is JsonValueKind.True)
        {
            return true;
        }
        else if (val is bool b)
        {
            return b;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<string?> GetModsFolderAsync()
    {
        return GetStringFromJson(await TryGetMTSettingsValueAsync(MT_SETTINGS_RGZINSTALL_NAME));
    }

    /// <inheritdoc/>
    public async Task<string?> GetWadsFolderAsync()
    {
        return GetStringFromJson(await TryGetMTSettingsValueAsync(MT_SETTINGS_CUSTOMINSTALL_NAME));
    }

    /// <inheritdoc/>
    public async Task SetIsEnableBloodAsync(bool isEnable)
    {
        if (await TrySetMTSettingsValueAsync(MT_SETTINGS_BLOOD_NAME, isEnable))
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.MyMods.MyModsService.SetIsEnableBloodAsync",
                $"The blood mode is {(isEnable ? "enabled" : "disabled")}.");
        }
    }

    /// <inheritdoc/>
    public async Task SetModsFolderAsync(string newFolder)
    {
        if (await TrySetMTSettingsValueAsync(MT_SETTINGS_RGZINSTALL_NAME, newFolder))
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.MyMods.MyModsService.SetModsFolderAsync",
                $"The mods folder have been changed, new folder: \"{newFolder}\".");
        }
    }

    /// <inheritdoc/>
    public async Task SetWadsFolderAsync(string newFolder)
    {
        if (await TrySetMTSettingsValueAsync(MT_SETTINGS_CUSTOMINSTALL_NAME, newFolder))
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.MyMods.MyModsService.SetWadsFolderAsync",
                $"The wads folder have been changed, new folder: \"{newFolder}\".");
        }
    }

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
                await ReadModInfoAsync(item);
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
    public async Task<Uri> GetModImageAsync(string filePath)
    {
        try
        {
            var imageCacheFolder = MyImageCacheFolder;
            if (Directory.Exists(imageCacheFolder) is false)
            {
                Directory.CreateDirectory(imageCacheFolder);
            }

            var imageFilePath = Path.Combine(imageCacheFolder, Path.GetFileNameWithoutExtension(filePath));
            await MyModsHelper.ExtractModImageAsync(filePath, imageFilePath);
            return new Uri(imageFilePath, UriKind.Absolute);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MyMods.MyModsService.GetModImage",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}");

            return new Uri("ms-appx:///Assets/Images/Mod_Preview_Empty.png");
        }
    }

    /// <inheritdoc/>
    public async Task<MyModInfo> ReadModInfoAsync(string filePath)
    {
        return await MyModsHelper.ReadModInfoAsync(filePath);
    }

    /// <inheritdoc/>
    public async Task<MyModInfo?> TryReadModInfoAsync(string filePath)
    {
        try
        {
            return await MyModsHelper.ReadModInfoAsync(filePath);
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
    /// 尝试获取模组配置的文件路径。
    /// </summary>
    /// <returns>如果服务组件未安装或不存在有效的包元数据将返回 null，否则将根据服务组件中的包元数据返回所指定的配置文件的完整路径。</returns>
    private static async Task<string?> TryGetMTSettingsFilePathAsync()
    {
        if (AppxContext.MounterService.TryGetMounterWorkingDirectory(out var workingDirectory) is false)
        {
            return null;
        }

        var metadata = await AppxContext.MounterService.TryGetCurrentPackageMetadataAsync(nameof(MTPackageMetadata.SettingsFile));
        if (metadata is null)
        {
            return null;
        }

        return Path.Combine(workingDirectory, metadata.SettingsFile);
    }

    /// <summary>
    /// 尝试从指定的模组配置文件中获取模组配置的 <see cref="MTSettingsRoot"/> 根节点内容。
    /// </summary>
    /// <param name="settingsFile">模组配置文件的路径。</param>
    /// <returns>如果成功在指定的模组配置文件中解析到目标节点便会将其返回，否则将返回 null。</returns>
    [return: NotNullIfNotNull(nameof(settingsFile))]
    private async Task<MTSettingsRoot?> TryGetMTSettingsRootAsync(string? settingsFile)
    {
        if (string.IsNullOrEmpty(settingsFile))
        {
            return null;
        }

        try
        {
            using var fileStream = new FileStream(settingsFile, FileMode.Open, FileAccess.Read);
            var settingsRoot = await JsonSerializer.DeserializeAsync<MTSettingsRoot>(fileStream, MyJsonSerializerOptions);
            if (settingsRoot is not null)
            {
                return settingsRoot;
            }
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    /// 从目标模组配置 <see cref="MTSettingsRoot"/> 的根节点中获取或创建指定的子数据配置节点。
    /// </summary>
    /// <param name="settingsRoot">模组配置的根对象。</param>
    private static MTSettingsData GetOrCreateSettingsData(MTSettingsRoot settingsRoot)
    {
        settingsRoot.Data ??= new List<MTSettingsData>();
        if (settingsRoot.Data.FirstOrDefault(a => StringComparer.OrdinalIgnoreCase.Equals(a.Key, MT_SETTINGS_KEY)) is not { } settingsData)
        {
            settingsRoot.Data.Add(settingsData = new MTSettingsData() { Key = MT_SETTINGS_KEY });
        }

        return settingsData;
    }

    /// <summary>
    /// 尝试从模组配置文件中特定的数据节点获取属性值。
    /// </summary>
    /// <param name="propName">与值所关联的属性名称。</param>
    /// <returns>如果成功从模组配置文件中找到特定的数据节点并获取到该成员的值便会将其返回，否则将返回 null。</returns>
    private async Task<object?> TryGetMTSettingsValueAsync(string propName)
    {
        var settingsFile = await TryGetMTSettingsFilePathAsync();
        var settingsRoot = await TryGetMTSettingsRootAsync(settingsFile);
        if (settingsRoot is not null)
        {
            var settingsData = GetOrCreateSettingsData(settingsRoot);
            if (settingsData.GZSkins?.TryGetValue(propName, out var obj) is true)
            {
                return obj;
            }
        }

        return null;
    }

    /// <summary>
    /// 尝试为模组配置文件中特定的数据节点设置属性的值。
    /// </summary>
    /// <param name="propName">与值所关联的属性名称。</param>
    /// <param name="value">目标属性的值。</param>
    /// <returns>如果成功为模组配置文件中的特定的数据节点中设置属性值将返回 true，否则返回 false。</returns>
    private async Task<bool> TrySetMTSettingsValueAsync(string propName, object value)
    {
        var settingsFile = await TryGetMTSettingsFilePathAsync();
        var settingsRoot = await TryGetMTSettingsRootAsync(settingsFile);

        if (settingsRoot is not null)
        {
            Debug.Assert(settingsFile is not null);
            var settingsData = GetOrCreateSettingsData(settingsRoot);

            settingsData.GZSkins ??= new();
            settingsData.GZSkins[propName] = value ?? string.Empty;
            await UpdateSettingsCoreAsync(settingsFile, settingsRoot, settingsData);
        }

        return false;
    }

    /// <summary>
    /// 对传入的已加密的字符串进行解密并返回模组文件列表。
    /// </summary>
    /// <param name="encryptedStr">加密后的模组文件列表字符串。</param>
    private static async IAsyncEnumerable<string> ParseRGZFileTableAsync(string? encryptedStr)
    {
        if (string.IsNullOrWhiteSpace(encryptedStr))
        {
            yield break;
        }

        var result = await MyModsHelper.DecryptConfigTextAsync(encryptedStr);
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
                await BuildMTSettingsDataAsync(settingsData);
                Debug.Assert(settingsData.GZSkins is not null);
                await UpdateSettingsCoreAsync(settingsFile, settingsRoot, settingsData);
            }

            if (settingsData.GZSkins.TryGetValue(MT_SETTINGS_FILETABLE_NAME, out var fileTable))
            {
                await foreach (var item in ParseRGZFileTableAsync(GetStringFromJson(fileTable)))
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

    /// <summary>
    /// 更新模组配置方法的核心实现。
    /// </summary>
    /// <param name="settingsFile">模组配置文件的路径。</param>
    /// <param name="settingsRoot">需要写出的模组配置的根对象。</param>
    /// <param name="settingsData">需要写出的模组配置中的子数据节点。</param>
    private async Task UpdateSettingsCoreAsync(string settingsFile, MTSettingsRoot settingsRoot, MTSettingsData settingsData)
    {
        var parentFolder = Path.GetDirectoryName(settingsFile);
        if (string.IsNullOrWhiteSpace(parentFolder) is false)
        {
            if (Directory.Exists(parentFolder) is false)
            {
                Directory.CreateDirectory(parentFolder);
            }
        }

        await BuildMTSettingsDataAsync(settingsData);

        try
        {
            using var outputStream = new FileStream(settingsFile, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(outputStream, settingsRoot, MyJsonSerializerOptions);

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.Appx.MyMods.MyModsService.UpdateSettingsCoreAsync",
                "Successfully update the mods configuration file.");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MyMods.MyModsService.UpdateSettingsCoreAsync",
                $"""
                Failed to update the mods configuration file.
                {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}.
                """);

            throw;
        }
    }

    /// <summary>
    /// 从特定的目录中获取子文件夹的完整路径。
    /// </summary>
    /// <param name="subFolderName">子文件夹的名称。</param>
    private static string GetWGZSubFolderPath(string subFolderName)
    {
        var targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (string.IsNullOrWhiteSpace(targetFolder))
        {
            return string.Empty;
        }
        else
        {
            // Try Create ?
            if (Directory.Exists(targetFolder) is false)
            {
                try
                {
                    Directory.CreateDirectory(targetFolder);
                }
                catch
                {
                    return string.Empty;
                }
            }

            return Path.Combine(targetFolder, "GeziSkin", subFolderName) + Path.DirectorySeparatorChar;
        }
    }

    /// <summary>
    /// 当前的上下文数据保存至指定的配置数据节点中。
    /// </summary>
    /// <param name="settingsData">需要写入的配置数据节点。</param>
    private async Task BuildMTSettingsDataAsync(MTSettingsData settingsData)
    {
        settingsData.GZSkins ??= new();
        settingsData.GZSkins[MT_SETTINGS_ENABLE_NAME] = true;

        // 设置默认的红色血液选项
        if (settingsData.GZSkins.TryGetValue(MT_SETTINGS_BLOOD_NAME, out var value) is false ||
            value is JsonElement elem && elem.ValueKind is not JsonValueKind.True or JsonValueKind.False)
        {
            settingsData.GZSkins[MT_SETTINGS_BLOOD_NAME] = false;
        }

        // 设置默认的 Wad 目录路径
        string? wadsFolder;
        if (settingsData.GZSkins.TryGetValue(MT_SETTINGS_CUSTOMINSTALL_NAME, out value) is false || string.IsNullOrWhiteSpace(wadsFolder = GetStringFromJson(value)))
        {
            settingsData.GZSkins[MT_SETTINGS_CUSTOMINSTALL_NAME] = wadsFolder = GetWGZSubFolderPath("Wads");
        }

        // 当目标文件夹不存在时将其创建
        if (Directory.Exists(wadsFolder) is false)
        {
            try
            {
                Directory.CreateDirectory(wadsFolder);
            }
            catch
            {
            }
        }

        // 设置默认的 Mod 目录路径
        string? modsFolder;
        if (settingsData.GZSkins.TryGetValue(MT_SETTINGS_RGZINSTALL_NAME, out value) is false || string.IsNullOrWhiteSpace((modsFolder = GetStringFromJson(value))))
        {
            settingsData.GZSkins[MT_SETTINGS_RGZINSTALL_NAME] = modsFolder = GetWGZSubFolderPath("Mods");
        }

        // 当目标文件夹不存在时将其创建
        if (Directory.Exists(modsFolder) is false)
        {
            try
            {
                Directory.CreateDirectory(modsFolder);
            }
            catch
            {
            }

            // 进入此处时表示目标文件夹不存在，因此将下方配置选项设为空并返回。
            settingsData.GZSkins[MT_SETTINGS_FILETABLE_NAME] = string.Empty;
            return;
        }

        // 在以下代码中需要手动生成文件的完整路径名，
        // 并对已安装的模组文件列表进行一个文件检查。

        // 由于生成文件的完整路径时需要手动追加文件扩展名，
        // 因此使用 StringBuilder 进行 Append 会比使用 Path.Combine 高效。
        // <!-- Path.Combine(modsFolder, item + ".lolgezi") -->

        var pathBuilder = new StringBuilder();
        pathBuilder.Append(modsFolder);
        pathBuilder.Append(Path.DirectorySeparatorChar);
        var folderPathOffset = pathBuilder.Length;

        var tableBuilder = new StringBuilder();
        foreach (var item in InstalledMods)
        {
            pathBuilder.Append(item);
            pathBuilder.Append(LOLGEZI_EXTENSION_NAME);

            var filePath = pathBuilder.ToString();
            if (File.Exists(filePath))
            {
                tableBuilder.Append(item);
                tableBuilder.Append(';');
            }

            pathBuilder.Length = folderPathOffset;
        }

        settingsData.GZSkins[MT_SETTINGS_FILETABLE_NAME] = await MyModsHelper.EncryptConfigTextAsync(tableBuilder.ToString());
    }

    /// <summary>
    /// 从指定 Json 元素对象中获取字符串值。
    /// </summary>
    private static string? GetStringFromJson(object? obj)
    {
        if (obj is null)
        {
            return null;
        }

        if (obj is string s)
        {
            return s;
        }

        if (obj is JsonElement elem && elem.ValueKind is JsonValueKind.String)
        {
            return elem.GetString();
        }

        return null;
    }

    private sealed record class MTSettingsData
    {
        public string? Key { get; set; }

        public Dictionary<string, object>? GZSkins { get; set; }

        public Dictionary<string, object>? SkinsData { get; set; }

        public Dictionary<string, object>? Extra { get; set; }
    }

    private sealed class MTSettingsRoot
    {
        public IList<MTSettingsData>? Data { get; set; }
    }
}
