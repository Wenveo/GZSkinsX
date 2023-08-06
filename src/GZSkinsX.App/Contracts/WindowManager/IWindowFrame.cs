// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.WindowManager;

/// <summary>
/// 表示位于应用程序主窗口中的页面元素，通常被用于在主窗口进行页面切换
/// </summary>
public interface IWindowFrame
{
    /// <summary>
    /// 获取当前 <see cref="IWindowFrame"/> 是否可以进行导航
    /// </summary>
    /// <param name="args">进入导航时的事件参数</param>
    /// <returns>如果可以导航至目标 <see cref="IWindowFrame"/> 则返回 true，否则返回 false</returns>
    bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args);
}
