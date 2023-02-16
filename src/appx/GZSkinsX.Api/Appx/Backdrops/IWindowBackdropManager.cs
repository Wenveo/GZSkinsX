// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Appx.Backdrops;

/// <summary>
/// 提供设置应用程序主窗口的背景的能力
/// </summary>
public interface IWindowBackdropManager
{
    /// <summary>
    /// 当前应用程序主窗口的背景类型
    /// </summary>
    BackdropType CurrentBackdrop { get; }

    /// <summary>
    /// 设置当前应用程序主窗口的背景
    /// </summary>
    /// <param name="type">背景类型</param>
    /// <returns>如果设置成功则返回 true，反之为 false</returns>
    bool SetBackdrop(BackdropType type);
}
