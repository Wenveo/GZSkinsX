// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml.Media.Animation;

namespace GZSkinsX.Api.WindowManager;

/// <summary>
/// 表示在 <see cref="IWindowManagerService"/> 中进行导航时所用到的事件参数
/// </summary>
public sealed class WindowFrameNavigatingEvnetArgs
{
    /// <summary>
    /// 获取当前导航对象的上下文信息
    /// </summary>
    public IWindowFrameContext Context { get; }

    /// <summary>
    /// 获取和设置导航至目标页面所传递的参数
    /// </summary>
    public object? Parameter { get; set; }

    /// <summary>
    /// 获取和设置在导航时用于页面过渡的切换动画参数
    /// </summary>
    public NavigationTransitionInfo? NavigationTransitionInfo { get; set; }

    /// <summary>
    /// 初始化 <see cref="WindowFrameNavigatingEvnetArgs"/> 的新实例
    /// </summary>
    public WindowFrameNavigatingEvnetArgs(
        IWindowFrameContext context, object? parameter,
        NavigationTransitionInfo? navigationTransitionInfo)
    {
        Context = context;
        Parameter = parameter;
        NavigationTransitionInfo = navigationTransitionInfo;
    }
}
