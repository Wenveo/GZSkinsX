// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

using GZSkinsX.Api.Appx;

using Microsoft.UI.Xaml;

using WinRT.Interop;

namespace GZSkinsX.MainApp;

/// <summary>
/// 主程序应用窗口类，用于管理、创建、以及注册主窗口
/// </summary>
[Shared, Export(typeof(IAppxWindow))]
internal sealed class AppxWindow : IAppxWindow
{
    /// <summary>
    /// 当前应用程序主窗口实例
    /// </summary>
    private readonly Window _shellWindow;

    /// <inheritdoc/>
    public Window MainWindow => _shellWindow;

    /// <inheritdoc/>
    public nint MainWindowHandle { get; }

    /// <inheritdoc/>
    public event EventHandler<WindowActivatedEventArgs>? Activated;

    /// <inheritdoc/>
    public event EventHandler<WindowActivatedEventArgs>? Deactivated;

    /// <inheritdoc/>
    public event EventHandler? Closed;

    /// <summary>
    /// 初始化 <see cref="AppxWindow"/> 的新实例
    /// </summary>
    public AppxWindow()
    {
        _shellWindow = App.MainWindow;
        _shellWindow.Activated += OnActivated;
        _shellWindow.Closed += OnClosed;

        MainWindowHandle = WindowNative.GetWindowHandle(_shellWindow);
    }

    /// <summary>
    /// 当前应用程序主窗口的激活事件
    /// </summary>
    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            Deactivated?.Invoke(this, args);
        }
        else
        {
            Activated?.Invoke(this, args);
        }
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        Closed?.Invoke(this, new EventArgs());
    }

    /// <inheritdoc/>
    public void Activate() => _shellWindow.Activate();

    /// <inheritdoc/>
    public void Close() => _shellWindow.Close();
}
