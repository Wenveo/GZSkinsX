// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GZSkinsX.Api.WindowManager;

/// <summary>
/// 表示位于应用程序主窗口中的页面元素，通常被用于在主窗口进行页面切换
/// </summary>
public interface IWindowFrame
{
    /// <summary>
    /// 在离开当前页面时触发，可在此进行取消事件注册等相关操作
    /// </summary>
    Task OnNavigateFromAsync();

    /// <summary>
    /// 在导航至目标页面时触发，可对目标页面属性进行更改及调整
    /// </summary>
    /// <param name="args"><seealso cref="Frame"/> 的导航事件参数</param>
    Task OnNavigateToAsync(NavigationEventArgs args);

    /// <summary>
    /// 在进入导航操作时触发，可在导航到目标页面前进行相关操作
    /// </summary>
    /// <param name="args">导航的事件参数</param>
    Task OnNavigatingAsync(WindowFrameNavigateEventArgs args);
}
