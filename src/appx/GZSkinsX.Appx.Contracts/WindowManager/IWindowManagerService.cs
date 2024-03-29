// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace GZSkinsX.Contracts.WindowManager;

/// <summary>
/// 提供对当前应用程序主窗口中的页面的切换/导航的能力。
/// </summary>
public interface IWindowManagerService
{
    /// <summary>
    /// 获取当前窗口页面的上下文。
    /// </summary>
    IWindowFrameContext? FrameContext { get; }

    /// <summary>
    /// 导航到与标识符匹配的指定页面。
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值。</param>
    void NavigateTo(string guidString);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数。
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值。</param>
    /// <param name="parameter">传递给目标页面的参数。</param>
    void NavigateTo(string guidString, object parameter);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数和指定导航页面切换效果。
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值。</param>
    /// <param name="parameter">传递给目标页面的参数。</param>
    /// <param name="infoOverride">用于导航时的页面过渡效果。</param>
    void NavigateTo(string guidString, object parameter, Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo infoOverride);

    /// <summary>
    /// 导航到与标识符匹配的指定页面。
    /// </summary>
    /// <param name="frameGuid">视图对象的 <see cref="Guid"/> 值。</param>
    void NavigateTo(Guid frameGuid);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数。
    /// </summary>
    /// <param name="frameGuid">视图对象的 <see cref="Guid"/> 值。</param>
    /// <param name="parameter">传递给目标页面的参数。</param>
    void NavigateTo(Guid frameGuid, object parameter);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数和指定导航页面切换效果。
    /// </summary>
    /// <param name="frameGuid">视图对象的 <see cref="Guid"/> 值。</param>
    /// <param name="parameter">传递给目标页面的参数。</param>
    /// <param name="infoOverride">用于导航时的页面过渡效果。</param>
    void NavigateTo(Guid frameGuid, object parameter, Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo infoOverride);
}
