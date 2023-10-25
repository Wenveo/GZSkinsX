// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GZSkinsX.Contracts.MyMods;

/// <summary>
/// 提供对模组管理的一系列功能接口的集成。
/// </summary>
public interface IMyModsService
{
    /// <summary>
    /// 获取当前模组配置中是否启用了红色血液选项。
    /// </summary>
    Task<bool> GetIsEnableBloodAsync();

    /// <summary>
    /// 获取当前模组配置中设置的存放 ".lolgezi" 类型的文件的目录。
    /// </summary>
    Task<string?> GetModFolderAsync();

    /// <summary>
    /// 获取当前模组配置中设置的存放 ".wad.client" 类型的文件的目录。
    /// </summary>
    Task<string?> GetWadFolderAsync();

    /// <summary>
    /// 对当前的模组配置设置是否启用红色血液选项。
    /// </summary>
    /// <param name="isEnable">true 表示为启用，false 则表示为禁用。</param>
    Task SetIsEnableBloodAsync(bool isEnable);

    /// <summary>
    /// 为当前模组配置中的存放 ".lolgezi" 类型的文件的目录设置一个新的路径。
    /// </summary>
    /// <param name="newFolder">需要设置的新的目录路径。</param>
    Task SetModFolderAsync(string newFolder);

    /// <summary>
    /// 为当前模组配置中的存放 ".wad.client" 类型的文件的目录设置一个新的路径。
    /// </summary>
    /// <param name="newFolder">需要设置的新的目录路径。</param>
    Task SetWadFolderAsync(string newFolder);

    /// <summary>
    /// 清楚当前模组配置中的所有已安装项。
    /// </summary>
    Task ClearAllInstalledAsync();

    /// <summary>
    /// 判断目标模组 (仅支持 ".lolgezi") 是否为已安装的状态。
    /// </summary>
    /// <param name="filename">模组的文件名，可以是完整的路径名称，亦或是文件的显示名称。</param>
    /// <returns>当目标模组存在于模组文件夹和模组的安装表中将返回 true，否则返回 false。</returns>
    bool IsInstalled(string filename);

    /// <summary>
    /// 获取目标模组位于安装表中的索引位置。
    /// </summary>
    /// <param name="filename">模组的文件名，可以是完整的路径名称，亦或是文件的显示名称。</param>
    /// <returns>如果安装表为空，或者该模组不存在与安装表中将返回 -1，否则返回非负值。</returns>
    int IndexOfTable(string filename);

    /// <summary>
    /// 导入一个或多个模组文件。
    /// </summary>
    /// <param name="files">需要导入的文件列表。</param>
    Task ImportModsAsync(params string[] files);

    /// <summary>
    /// 导入一个或多个模组文件，并决定是否覆写已有文件。
    /// </summary>
    /// <param name="overwrite">当传入 true 时表示可覆写已有文件，否则将会在找到存在相同名称的文件时跳过导入操作。</param>
    /// <param name="files">需要导入的文件列表。</param>
    Task ImportModsAsync(bool overwrite, params string[] files);

    /// <summary>
    /// 导入一个或多个模组文件。
    /// </summary>
    /// <param name="files">需要导入的文件列表。</param>
    Task ImportModsAsync(IEnumerable<string> files);

    /// <summary>
    /// 导入一个或多个模组文件，并决定是否覆写已有文件。
    /// </summary>
    /// <param name="files">需要导入的文件列表。</param>
    /// <param name="overwrite">当传入 true 时表示可覆写已有文件，否则将会在找到存在相同名称的文件时跳过导入操作。</param>
    Task ImportModsAsync(IEnumerable<string> files, bool overwrite);

    /// <summary>
    /// 安装一个或多个模组文件。
    /// </summary>
    /// <param name="files">需要安装的模组文件列表。</param>
    Task InstallModsAsync(params string[] files);

    /// <summary>
    /// 安装一个或多个模组文件。
    /// </summary>
    /// <param name="files">需要安装的模组文件列表。</param>
    Task InstallModsAsync(IEnumerable<string> files);

    /// <summary>
    /// 卸载一个或多个模组文件。
    /// </summary>
    /// <param name="files">需要卸载的模组文件列表。</param>
    Task UninstallModsAsync(params string[] files);

    /// <summary>
    /// 卸载一个或多个模组文件。
    /// </summary>
    /// <param name="files">需要卸载的模组文件列表。</param>
    Task UninstallModsAsync(IEnumerable<string> files);

    /// <summary>
    /// 获取目标模组文件的本地缓存图片地址。
    /// </summary>
    /// <param name="filePath">模组文件的路径。</param>
    /// <returns>当此操作完成时，将返回该模组文件的本地缓存图片地址。</returns>
    Uri GetModImage(string filePath);

    /// <summary>
    /// 读取目标模组文件的模组信息。
    /// </summary>
    /// <param name="filePath">模组文件的路径。</param>
    /// <returns>当此操作完成时，将返回该模组文件的模组信息。</returns>
    MyModInfo ReadModInfo(string filePath);

    /// <summary>
    /// 尝试读取目标模组文件的模组信息。
    /// </summary>
    /// <param name="filePath">模组文件的路径。</param>
    /// <returns>如果成功读取该模组文件的模组信息便会将其返回，否则将返回 null。</returns>
    MyModInfo? TryReadModInfo(string filePath);

    /// <summary>
    /// 刷新当前模组服务的上下文。
    /// </summary>
    Task RefreshAsync();

    /// <summary>
    /// 更新模组配置。
    /// </summary>
    Task UpdateSettingsAsync();
}
