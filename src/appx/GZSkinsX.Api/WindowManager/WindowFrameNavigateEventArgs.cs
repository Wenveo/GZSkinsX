// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.WindowManager;

/// <summary>
/// 导航的事件参数，当触发导航操作时被使用
/// </summary>
public sealed class WindowFrameNavigateEventArgs : System.EventArgs
{
    /// <summary>
    /// 表示当前事件是否已经被处理；当为 true 时表明当前事件已被处理且不再继续往下执行
    /// </summary>
    public bool Handled { get; set; }
}
