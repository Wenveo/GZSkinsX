// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.WindowManager;

/// <summary>
/// 表示位于应用程序主窗口中的视图元素，通常被用于在主窗口中对根元素进行页面切换
/// </summary>
public interface IWindowFrame
{
    /// <summary>
    /// 在页面初始化时触发，可对目标页面属性进行更改及调整
    /// </summary>
    /// <param name="viewElement">目标视图对象</param>
    Task OnInitializeAsync(Page viewElement);

    /// <summary>
    /// 在进入导航操作时触发，可在导航到目标页面前进行相关操作
    /// </summary>
    /// <param name="args">导航的事件参数</param>
    Task OnNavigatingAsync(WindowFrameNavigateEventArgs args);
}
