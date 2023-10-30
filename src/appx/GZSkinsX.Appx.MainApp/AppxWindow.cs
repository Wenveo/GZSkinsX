// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

using GZSkinsX.Contracts.Appx;

using Microsoft.UI.Xaml;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace GZSkinsX.Appx.MainApp;

/// <inheritdoc cref="IAppxWindow"/>
[Shared, Export(typeof(IAppxWindow))]
internal sealed class AppxWindow : IAppxWindow
{
    /// <summary>
    /// 当前应用程序主窗口实例。
    /// </summary>
    private readonly ShellWindow _shellWindow;

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
    /// 初始化 <see cref="AppxWindow"/> 的新实例。
    /// </summary>
    public AppxWindow()
    {
        _shellWindow = new ShellWindow(default, true);
        _shellWindow.Activated += OnActivated;
        _shellWindow.Closed += OnClosed;

        MainWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(_shellWindow);
    }

    /// <summary>
    /// 当前应用程序主窗口的激活事件。
    /// </summary>
    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState is WindowActivationState.Deactivated)
        {
            Deactivated?.Invoke(this, args);
        }
        else
        {
            if (_shellWindow.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = AppxContext.ThemeService.CurrentTheme;
            }

            Activated?.Invoke(this, args);
        }
    }

    /// <summary>
    /// 当前应用程序主窗口的关闭事件。
    /// </summary>
    private void OnClosed(object sender, WindowEventArgs args)
    {
        Closed?.Invoke(this, new EventArgs());
    }

    /// <inheritdoc/>
    public void Activate()
    {
        var hWnd = (HWND)MainWindowHandle;
        if (PInvoke.IsIconic(hWnd))
        {
            PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_RESTORE);
        }
        else
        {
            PInvoke.SetForegroundWindow(hWnd);
        }
    }

    /// <inheritdoc/>
    public void Close()
    {
        _shellWindow.Close();
    }
}
