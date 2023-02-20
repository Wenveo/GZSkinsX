// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml;

namespace GZSkinsX.Api.Appx;

/// <summary>
/// 提供应用程序主窗口的事件，以及窗口管理相关的 Api
/// </summary>
public interface IAppxWindow
{
    /// <summary>
    /// 当应用程序主窗口被激活时触发
    /// </summary>
    event EventHandler<WindowActivatedEventArgs>? Activated;

    /// <summary>
    /// 当应用程序主窗口被置为后台窗口时触发
    /// </summary>
    event EventHandler<WindowActivatedEventArgs>? Deactivated;

    /// <summary>
    /// 在应用程序主窗口关闭时触发
    /// </summary>
    event EventHandler<WindowEventArgs>? Closing;

    /// <summary>
    /// 在应用程序主窗口关闭之后触发
    /// </summary>
    event EventHandler? Closed;

    /// <summary>
    /// 当前应用程序主窗口
    /// </summary>
    Window MainWindow { get; }

    /// <summary>
    /// 激活当前应用程序主窗口
    /// </summary>
    void Activate();

    /// <summary>
    /// 最小化当前应用程序主窗口
    /// </summary>
    void Minimize();

    /// <summary>
    /// 最大化当前应用程序主窗口
    /// </summary>
    void Maximize();

    /// <summary>
    /// 向下还原当前应用程序主窗口
    /// </summary>
    void Restore();

    /// <summary>
    /// 关闭当前应用程序主窗口
    /// </summary>
    void Close();

    /// <summary>
    /// 隐藏当前应用程序主窗口
    /// </summary>
    void Hide();

    /// <summary>
    /// 显示当前应用程序主窗口
    /// </summary>
    void Show();
}
