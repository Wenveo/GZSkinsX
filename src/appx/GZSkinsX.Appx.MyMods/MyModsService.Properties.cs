// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;

namespace GZSkinsX.Appx.MyMods;

partial class MyModsService
{
    /// <inheritdoc/>
    public async Task<bool> GetIsEnableBloodAsync()
    {
        return GetBooleanFromJson(await TryGetMTSettingsValueAsync(MT_SETTINGS_BLOOD_NAME));
    }

    /// <inheritdoc/>
    public async Task<string?> GetModFolderAsync()
    {
        return GetStringFromJson(await TryGetMTSettingsValueAsync(MT_SETTINGS_RGZINSTALL_NAME))
               ?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    /// <inheritdoc/>
    public async Task<string?> GetWadFolderAsync()
    {
        return GetStringFromJson(await TryGetMTSettingsValueAsync(MT_SETTINGS_CUSTOMINSTALL_NAME))
               ?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
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
    public async Task SetModFolderAsync(string newFolder)
    {
        if (await TrySetMTSettingsValueAsync(MT_SETTINGS_RGZINSTALL_NAME, newFolder))
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.MyMods.MyModsService.SetModFolderAsync",
                $"The mods folder have been changed, new folder: \"{newFolder}\".");
        }
    }

    /// <inheritdoc/>
    public async Task SetWadFolderAsync(string newFolder)
    {
        if (await TrySetMTSettingsValueAsync(MT_SETTINGS_CUSTOMINSTALL_NAME, newFolder))
        {
            AppxContext.LoggingService.LogAlways(
                "GZSkinsX.Appx.MyMods.MyModsService.SetWadsFolderAsync",
                $"The wads folder have been changed, new folder: \"{newFolder}\".");
        }
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
    /// 从指定 Json 对象中获取字符串值。
    /// </summary>
    private static string? GetStringFromJson(object? obj)
    {
        if (obj is JsonElement elem && elem.ValueKind is JsonValueKind.String)
        {
            return elem.GetString();
        }
        else if (obj is string s)
        {
            return s;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 从指定 Json 对象中获取 Boolean 值。
    /// </summary>
    private static bool GetBooleanFromJson(object? obj)
    {
        if (obj is JsonElement elem && elem.ValueKind is JsonValueKind.True)
        {
            return true;
        }
        else if (obj is bool b)
        {
            return b;
        }
        else
        {
            return false;
        }
    }
}
