// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage;

namespace GZSkinsX.Contracts.Mounter;

/// <summary>
/// 提供对挂载服务的启动、停止、更新等一系列的功能集成
/// </summary>
public interface IMounterService
{
    /// <summary>
    /// 用于判断挂载服务是否正在运行中的事件，该事件支持实时监听运行状态更改
    /// </summary>
    event TypedEventHandler<IMounterService, bool>? IsRunningChanged;

    /// <summary>
    /// 检查服务组件是否有更新的内容
    /// </summary>
    /// <returns>当有最新的更新内容时返回 true，否则返回 false</returns>
    Task<bool> CheckForUpdatesAsync();

    /// <summary>
    /// 获取当前挂载服务的运行状态
    /// </summary>
    /// <returns>如果挂载服务正在运行则为 true，否则为 false</returns>
    Task<bool> GetIsRunningAsync();

    /// <summary>
    /// 获取当前组件的包元数据信息
    /// </summary>
    /// <param name="filter">筛选并获取指定成员的值，默认为获取所有成员的值</param>
    /// <returns>如果未找到有效的文件/目录则会抛出异常，否则将返回具体的包元数据信息</returns>
    Task<MTPackageMetadata> GetCurrentPackageMetadataAsync(params string[] filter);

    /// <summary>
    /// 获取当前挂载服务的工作目录
    /// </summary>
    /// <returns>如果未下载/安装服务组件或文件夹不存在时将抛出异常，否则将返回具体的工作目录</returns>
    Task<StorageFolder> GetMounterWorkingDirectoryAsync();

    /// <summary>
    /// 启动挂载服务
    /// </summary>
    Task LaunchAsync();

    /// <summary>
    /// 通过指定的参数运行挂载服务
    /// </summary>
    /// <param name="args">启动参数</param>
    Task LaunchAsync(string args);

    /// <summary>
    /// 终止挂载服务
    /// </summary>
    Task TerminateAsync();

    /// <summary>
    /// 尝试获取当前组件的包元数据信息
    /// </summary>
    /// <param name="filter">筛选并获取指定成员的值，默认为获取所有成员的值</param>
    /// <returns>如果未找到有效的文件/目录则会返回 <seealso cref="MTPackageMetadata.Empty"/>，否则将返回非空的包元数据信息</returns>
    Task<MTPackageMetadata> TryGetCurrentPackageMetadataAsync(params string[] filter);

    /// <summary>
    /// 尝试获取当前挂载服务的工作目录
    /// </summary>
    /// <returns>如果未下载/安装服务组件或文件夹不存在时将返回空文件夹，否则将返回非空的工作目录</returns>
    Task<StorageFolder?> TryGetMounterWorkingDirectoryAsync();

    /// <summary>
    /// 从服务器中下载和更新组件。
    /// <para>
    /// 注意，此方法为强制性更新，在进行下载和更新前请务必使用 <seealso cref="CheckForUpdatesAsync"/> 检查是否有必要更新。
    /// </para>
    /// </summary>
    /// <param name="progress">进度更新</param>
    Task UpdateAsync(IProgress<double>? progress = null);

    /// <summary>
    /// 验证本地已下载的组件的完整性
    /// </summary>
    /// <returns>当</returns>
    Task<bool> VerifyContentIntegrityAsync();
}
