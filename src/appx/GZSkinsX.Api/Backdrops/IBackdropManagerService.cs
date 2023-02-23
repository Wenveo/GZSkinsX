// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Backdrops;

/// <summary>
/// 提供管理应用程序主窗口背景的能力
/// </summary>
public interface IBackdropManagerService
{
    /// <summary>
    /// 当前应用程序主窗口的背景类型
    /// </summary>
    BackdropType CurrentBackdrop { get; }

    /// <summary>
    /// 设置当前应用程序主窗口的背景
    /// </summary>
    /// <param name="type">需要设置的窗口背景类型</param>
    /// <returns>如果设置成功则返回 true，否则返回 false</returns>
    bool SetBackdrop(BackdropType type);

    /// <summary>
    /// 判断目标背景类型是否在此系统上支持
    /// </summary>
    /// <param name="type">用于判断的 <see cref="BackdropType"/> 枚举值</param>
    /// <returns>如果该系统支持则返回 true，否则返回 false</returns>
    bool IsSupported(BackdropType type);
}
