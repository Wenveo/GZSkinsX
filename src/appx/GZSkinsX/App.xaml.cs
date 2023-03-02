﻿// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition.Hosting;

using GZSkinsX.Api.Appx;

using GZSkinsX.Api.Extension;
using GZSkinsX.Api.Shell;
using GZSkinsX.Composition;
using GZSkinsX.Diagnostics;
using GZSkinsX.Extension;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    /// <summary>
    /// 当前组件容器的宿主实例
    /// </summary>
    private readonly CompositionHost _compositionHost;

    /// <summary>
    /// 当前应用程序主窗口
    /// </summary>
    private IAppxWindow? _appxWindow;

    /// <summary>
    /// 当前窗口视图核心管理服务
    /// </summary>
    private IViewManagerService? _viewManagerService;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();

        var provider = CompositionContainerProvider.Instance;
        _compositionHost = provider.CompositionHost;
        InitializeServices(_compositionHost);
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        _appxWindow ??= _compositionHost.GetExport<IAppxWindow>();
        if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
        {
            //TODO: Load state from previously suspended application
        }

        if (e.PrelaunchActivated == false)
        {
            var frame = _appxWindow.MainWindow.Content as Frame;
            Debug2.Assert(frame is not null);
            if (frame.Content is null)
            {
                _viewManagerService ??= _compositionHost.GetExport<IViewManagerService>();
                _viewManagerService.NavigateTo(ViewElementConstants.StartUpPage_Guid);
            }

            _appxWindow.Activate();
        }
    }

    /// <summary>
    /// 初始化应用程序核心服务
    /// </summary>
    /// <param name="compositionHost"><see cref="CompositionHost"/> 对象的实例</param>
    private void InitializeServices(CompositionHost compositionHost)
    {
        InitializeExtension(compositionHost.GetExport<ExtensionService>());
    }

    /// <summary>
    /// 初始化应用程序扩展组件
    /// </summary>
    /// <param name="extensionService">扩展服务实例</param>
    private void InitializeExtension(ExtensionService extensionService)
    {
        extensionService.LoadAutoLoaded(AutoLoadedType.BeforeExtensions);
        foreach (var rsrc in extensionService.GetMergedResourceDictionaries())
        {
            Resources.MergedDictionaries.Add(rsrc);
        }

        extensionService.LoadAutoLoaded(AutoLoadedType.AfterExtensions);
        extensionService.NotifyExtensions(ExtensionEvent.Loaded);
        extensionService.LoadAutoLoaded(AutoLoadedType.AfterExtensionsLoaded);
    }
}
