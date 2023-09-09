// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Media.Animation;

namespace GZSkinsX.Contracts.WindowManager;

/// <summary>
/// 表示在 <see cref="IWindowManagerService"/> 中进行导航时所用到的事件参数。
/// </summary>
public sealed class WindowFrameNavigatingEvnetArgs(
    IWindowFrameContext context, object? parameter,
    NavigationTransitionInfo? navigationTransitionInfo)
{
    /// <summary>
    /// 获取当前导航对象的上下文信息。
    /// </summary>
    public IWindowFrameContext Context { get; } = context;

    /// <summary>
    /// 获取和设置导航至目标页面所传递的参数。
    /// </summary>
    public object? Parameter { get; set; } = parameter;

    /// <summary>
    /// 获取和设置在导航时用于页面过渡的切换动画参数。
    /// </summary>
    public NavigationTransitionInfo? NavigationTransitionInfo { get; set; } = navigationTransitionInfo;
}
