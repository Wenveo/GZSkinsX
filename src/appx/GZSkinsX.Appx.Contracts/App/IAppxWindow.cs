// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using Microsoft.UI.Xaml;

namespace GZSkinsX.Appx.Contracts.App;

/// <summary>
/// 提供应用程序主窗口管理的相关功能集合。
/// </summary>
public interface IAppxWindow
{
    /// <summary>
    /// 当前应用程序主窗口。
    /// </summary>
    Window MainWindow { get; }

    /// <summary>
    /// 当前应用程序主窗口的句柄。
    /// </summary>
    nint MainWindowHandle { get; }

    /// <summary>
    /// 激活当前应用程序主窗口。
    /// </summary>
    void Activate();

    /// <summary>
    /// 关闭当前应用程序主窗口。
    /// </summary>
    void Close();

    /// <summary>
    /// 当应用程序主窗口被激活时触发。
    /// </summary>
    event EventHandler<WindowActivatedEventArgs>? Activated;

    /// <summary>
    /// 当应用程序主窗口被置为后台窗口时触发。
    /// </summary>
    event EventHandler<WindowActivatedEventArgs>? Deactivated;

    /// <summary>
    /// 在应用程序主窗口关闭时触发。
    /// </summary>
    event EventHandler? Closed;
}
