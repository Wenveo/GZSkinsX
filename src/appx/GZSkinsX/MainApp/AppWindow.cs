﻿// Copyright 2022 - 2023 GZSkins, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Composition;

using GZSkinsX.Contracts.App;
using GZSkinsX.Contracts.Extension;
using GZSkinsX.Extension;

using Microsoft.UI.Xaml;

namespace GZSkinsX.MainApp;

/// <summary>
/// 主程序应用窗口类，用于管理、创建、以及注册主窗口
/// </summary>
[Shared]
[Export, Export(typeof(IAppWindow))]
internal sealed class AppWindow : IAppWindow
{
    /// <summary>
    /// 当前应用程序的扩展服务，主要用于在 OnAppLoaded 事件中对已加载的扩展进行通知 AppLoaded 事件
    /// </summary>
    private readonly ExtensionService _extensionService;

    /// <summary>
    /// 当前应用程序主窗口实例
    /// </summary>
    private readonly ShellWindow _shellWindow;

    /// <inheritdoc/>
    public event EventHandler<WindowActivatedEventArgs>? Activated;

    /// <inheritdoc/>
    public event EventHandler<WindowActivatedEventArgs>? Deactivated;

    /// <inheritdoc/>
    public event EventHandler<WindowEventArgs>? Closing;

    /// <inheritdoc/>
    public event EventHandler? Closed;

    /// <inheritdoc/>
    public Window MainWindow => _shellWindow;

    /// <summary>
    /// 初始化 <see cref="AppWindow"/> 的新实例
    /// </summary>
    /// <param name="extensionService">应用程序扩展服务</param>
    [ImportingConstructor]
    public AppWindow(ExtensionService extensionService)
    {
        _extensionService = extensionService;
        _shellWindow = new ShellWindow();

        _shellWindow.Activated += OnActivated;
        _shellWindow.Closed += OnClosed;

        Activated += OnAppLoaded;
    }

    event EventHandler? IAppWindow.Closed
    {
        add => throw new NotImplementedException();

        remove => throw new NotImplementedException();
    }

    /// <summary>
    /// 负责对已加载的应用程序扩展通知 AppLoaded 事件
    /// </summary>
    private void OnAppLoaded(object? sender, WindowActivatedEventArgs e)
    {
        Activated -= OnAppLoaded;
        _extensionService.NotifyExtensions(ExtensionEvent.AppLoaded);
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

    /// <summary>
    /// 当前应用程序主窗口的关闭事件
    /// </summary>
    private void OnClosed(object sender, WindowEventArgs args)
    {
        Closing?.Invoke(this, args);

        if (!args.Handled)
        {
            _extensionService.NotifyExtensions(ExtensionEvent.AppExit);
            Closed?.Invoke(this, new EventArgs());
        }
    }

    /// <summary>
    /// 显示当前应用程序主窗口
    /// </summary>
    public void ShowWindow()
    {
        _shellWindow.Activate();
    }
}
