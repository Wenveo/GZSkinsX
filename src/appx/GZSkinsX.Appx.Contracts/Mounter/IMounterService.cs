// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Windows.Foundation;

namespace GZSkinsX.Contracts.Mounter;

/// <summary>
/// 提供对组件服务的启动、停止、更新等一系列的功能集成。
/// </summary>
public interface IMounterService
{
    /// <summary>
    /// 获取当前组件服务的运行状态。
    /// </summary>
    /// <returns>如果组件服务正在运行则为 true，否则为 false。</returns>
    bool IsMTRunning { get; }

    /// <summary>
    /// 用于判断组件服务是否正在运行中的事件，该事件支持实时监听运行状态更改。
    /// </summary>
    event TypedEventHandler<IMounterService, bool>? IsRunningChanged;

    /// <summary>
    /// 检查服务组件是否有更新的内容。
    /// </summary>
    /// <returns>当有最新的更新内容时返回 true，否则返回 false。</returns>
    Task<bool> CheckForUpdatesAsync();

    /// <summary>
    /// 启动组件服务。
    /// </summary>
    Task LaunchAsync();

    /// <summary>
    /// 通过指定的参数运行组件服务。
    /// </summary>
    /// <param name="args">用于启动的参数。</param>
    Task LaunchAsync(string args);

    /// <summary>
    /// 终止组件服务。
    /// </summary>
    Task TerminateAsync();

    /// <summary>
    /// 尝试获取当前组件的包元数据信息。
    /// </summary>
    /// <param name="filter">筛选并获取指定成员的值，默认将获取所有成员的值。</param>
    /// <returns>如果成功获取到当前组件的包元数据便会将其返回，否则将返回 null。</returns>
    Task<MTPackageMetadata?> TryGetCurrentPackageMetadataAsync(params string[] filter);

    /// <summary>
    /// 尝试获取当前服务组件的工作目录。
    /// </summary>
    /// <param name="result">当此方法返回时，如果成功找到该工作目录便会将其返回，否则将返回 null。</param>
    /// <returns>如果成功获取到该工作目录将返回 true，否则返回 false。</returns>
    bool TryGetMounterWorkingDirectory([NotNullWhen(true)] out string? result);

    /// <summary>
    /// 从服务器中下载和更新组件。
    /// </summary>
    /// <param name="progress">用于报告进度更新。</param>
    Task UpdateAsync(IProgress<double>? progress = null);

    /// <summary>
    /// 验证本地已下载的组件的完整性。
    /// </summary>
    /// <returns>当对本地的组件完成验证后返回 true，否则返回 false。</returns>
    Task<bool> VerifyContentIntegrityAsync();
}
