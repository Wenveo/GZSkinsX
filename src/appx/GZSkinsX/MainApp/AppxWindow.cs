// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Extension;

using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace GZSkinsX.MainApp;

/// <summary>
/// 主程序应用窗口类，用于管理、创建、以及注册主窗口
/// </summary>
[Shared, Export(typeof(IAppxWindow))]
internal sealed class AppxWindow : IAppxWindow
{
    /// <summary>
    /// 当前应用程序的扩展服务，主要用于在 OnAppLoaded 事件中对已加载的扩展进行通知 AppLoaded 事件
    /// </summary>
    private readonly ExtensionService _extensionService;

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
    /// <param name="extensionService">应用程序扩展服务</param>
    [ImportingConstructor]
    public AppxWindow(ExtensionService extensionService)
    {
        _extensionService = extensionService;
        _currentAppView = ApplicationView.GetForCurrentView();

        _shellWindow = Window.Current;
        _shellWindow.Activated += OnActivated;
        _shellWindow.Closed += OnClosed;

        Activated += OnAppLoaded;
    }

    private void InitializeUIElement()
    {
#if DEBUG
        _currentAppView.Title = "DEBUG";
#endif
    }

    /// <summary>
    /// 负责对已加载的应用程序扩展通知 AppLoaded 事件
    /// </summary>
    private void OnAppLoaded(object? sender, WindowActivatedEventArgs e)
    {
        Activated -= OnAppLoaded;
        InitializeUIElement();

        _extensionService.NotifyExtensions(ExtensionEvent.AppLoaded);
        _extensionService.LoadAutoLoaded(AutoLoadedType.AppLoaded);
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
    private void OnClosed(object sender, CoreWindowEventArgs args)
    {
        _extensionService.NotifyExtensions(ExtensionEvent.AppExit);
        Closed?.Invoke(this, new EventArgs());
    }

    /// <inheritdoc/>
    public void Activate()
    => MainWindow.Activate();

    /// <inheritdoc/>
    public void Close()
    => MainWindow.Close();
}
