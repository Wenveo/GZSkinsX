// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Services.Logging;

using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace GZSkinsX.Services.MainApp;

/// <summary>
/// 主程序应用窗口类，用于管理、创建、以及注册主窗口
/// </summary>
[Shared, Export(typeof(IAppxWindow))]
internal sealed class AppxWindow : IAppxWindow
{
    /// <summary>
    /// 当前应用主视图实例
    /// </summary>
    private readonly ApplicationView _currentAppView;

    /// <summary>
    /// 当前应用程序主窗口实例
    /// </summary>
    private readonly Window _shellWindow;

    /// <inheritdoc/>
    public ApplicationView ApplicationView => _currentAppView;

    /// <inheritdoc/>
    public Window MainWindow => _shellWindow;

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
        _shellWindow = Window.Current;
        _shellWindow.Activated += OnActivated;
        _shellWindow.VisibilityChanged += OnVisibilityChanged;
        _currentAppView = ApplicationView.GetForCurrentView();
    }

    /// <summary>
    /// 用于在窗口显示完成时，注册窗口的关闭事件
    /// </summary>
    private void OnVisibilityChanged(object sender, VisibilityChangedEventArgs e)
    {
        _shellWindow.VisibilityChanged -= OnVisibilityChanged;
        SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += OnCloseRequested;
    }

    /// <summary>
    /// 当前应用程序主窗口的激活事件
    /// </summary>
    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState == CoreWindowActivationState.Deactivated)
        {
            Deactivated?.Invoke(this, args);
        }
        else
        {
            Activated?.Invoke(this, args);
        }
    }

    /// <summary>
    /// 当前应用程序主窗口的关闭事件
    /// </summary>
    private void OnCloseRequested(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
    {
        if (!e.Handled)
        {
            // 优先通知事件，因为有可能会在关闭事件中输出日志
            Closed?.Invoke(this, EventArgs.Empty);

            // 关闭日志输出流
            LoggerImpl.Shared.CloseOutputStream();
        }
    }

    /// <inheritdoc/>
    public void Activate()
    {
        _shellWindow.Activate();
    }

    /// <inheritdoc/>
    public void Close()
    {
        _shellWindow.Close();
    }
}