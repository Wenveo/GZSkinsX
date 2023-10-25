// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Mounter;

namespace GZSkinsX.Appx.MyMods;

partial class MyModsService
{
    /// <summary>
    /// 自定义的 Json 序列化配置选项。
    /// </summary>
    private JsonSerializerOptions MyJsonSerializerOptions { get; } = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

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

        BuildMTSettingsData(settingsData);

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
    private void BuildMTSettingsData(MTSettingsData settingsData)
    {
        settingsData.GZSkins ??= new();
        settingsData.GZSkins[MT_SETTINGS_ENABLE_NAME] = true;

        // 设置默认的红色血液选项
        if (settingsData.GZSkins.TryGetValue(MT_SETTINGS_BLOOD_NAME, out var value) is false ||
            (value is JsonElement elem && elem.ValueKind is not JsonValueKind.True or JsonValueKind.False))
        {
            settingsData.GZSkins[MT_SETTINGS_BLOOD_NAME] = false;
        }

        // 设置默认的 Wad 目录路径
        string? wadsFolder;
        if (settingsData.GZSkins.TryGetValue(MT_SETTINGS_CUSTOMINSTALL_NAME, out value))
        {
            wadsFolder = GetStringFromJson(value);
            if (string.IsNullOrWhiteSpace(wadsFolder))
            {
                wadsFolder = GetWGZSubFolderPath("Wads");
            }
            else if (wadsFolder[^1] != Path.DirectorySeparatorChar)
            {
                wadsFolder = Path.GetFullPath(wadsFolder) + Path.DirectorySeparatorChar;
            }
        }
        else
        {
            wadsFolder = GetWGZSubFolderPath("Wads");
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
                // 如果无法创建目录或者给定的路径无效而引发了异常，
                // 那么将此变量的内容设为空，避免写出无效的路径。
                wadsFolder = string.Empty;
            }
        }

        // 将 Wad 目录的路径写出到配置。
        settingsData.GZSkins[MT_SETTINGS_CUSTOMINSTALL_NAME] = wadsFolder;

        // 设置默认的 Mod 目录路径
        string? modFolder;
        if (settingsData.GZSkins.TryGetValue(MT_SETTINGS_RGZINSTALL_NAME, out value))
        {
            modFolder = GetStringFromJson(value);
            if (string.IsNullOrWhiteSpace(modFolder))
            {
                modFolder = GetWGZSubFolderPath("Mods");
            }
            else if (modFolder[^1] != Path.DirectorySeparatorChar)
            {
                modFolder = Path.GetFullPath(modFolder) + Path.DirectorySeparatorChar;
            }
        }
        else
        {
            modFolder = GetWGZSubFolderPath("Mods");
        }

        // 当目标文件夹不存在时将其创建
        if (Directory.Exists(modFolder) is false)
        {
            try
            {
                Directory.CreateDirectory(modFolder);
            }
            catch
            {
                // 同理，如果给定的路径无效或无法创建文件夹时清空配置，并跳过下方的文件表生成。
                settingsData.GZSkins[MT_SETTINGS_RGZINSTALL_NAME] = string.Empty;
                settingsData.GZSkins[MT_SETTINGS_FILETABLE_NAME] = string.Empty;
                return;
            }
        }

        // 将 Mod 目录的路径写出到配置。
        settingsData.GZSkins[MT_SETTINGS_RGZINSTALL_NAME] = modFolder;

        // 在以下代码中需要手动生成文件的完整路径名，
        // 并对已安装的模组文件列表进行一个文件检查。

        // 由于生成文件的完整路径时需要手动追加文件扩展名，
        // 因此使用 StringBuilder 进行 Append 会比使用 Path.Combine 高效。
        // <!-- Path.Combine(modsFolder, item + ".lolgezi") -->

        var pathBuilder = new StringBuilder();
        pathBuilder.Append(modFolder);
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

        settingsData.GZSkins[MT_SETTINGS_FILETABLE_NAME] = AppxContext.KernelService.EncryptConfigText(tableBuilder.ToString());
    }

    private sealed class MTSettingsData
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
