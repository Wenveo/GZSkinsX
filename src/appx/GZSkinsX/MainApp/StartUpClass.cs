// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition.Hosting;
using System.Diagnostics;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Api.Scripting;
using GZSkinsX.Extension;
using GZSkinsX.Logging;

using Microsoft.UI.Xaml.Controls;

using Windows.System;
using Windows.UI.Xaml;

namespace GZSkinsX.MainApp;

#if DISABLE_XAML_GENERATED_MAIN
/// <summary>
/// 自定义的程序启动类
/// </summary>
public sealed partial class StartUpClass
{
    /// <summary>
    /// 当前组件容器的宿主实例
    /// </summary>
    private static readonly CompositionHost s_compositionHost;

    /// <summary>
    /// 获取内部的 <see cref="global::System.Composition.Hosting.CompositionHost"/> 公开实现
    /// </summary>
    public static CompositionHost CompositionHost => s_compositionHost;

    /// <summary>
    /// 初始化 <see cref="StartUpClass"/> 的静态成员
    /// </summary>
    static StartUpClass()
    {
        var configuration = new ContainerConfiguration();
        configuration.WithAssemblies(GetAssemblies());
        s_compositionHost = configuration.CreateContainer();
    }

    [DebuggerNonUserCode]
    public static void Main(string[] args)
    {
        Application.Start((p) => InitializeServices(p, new App()));
    }

    /// <summary>
    /// 加载和初始化应用程序的基本服务
    /// </summary>
    /// <param name="parms">应用程序初始化时的参数</param>
    /// <param name="mainApp">用于初始化的应用程序</param>
    [DebuggerNonUserCode]
    private static async void InitializeServices(ApplicationInitializationCallbackParams parms, App mainApp)
    {
        await LoggerImpl.Shared.InitializeAsync();

        // 这部分可能看着有些别扭，但这里必须先获取导出
        // 的 ExtensionService，然后开始输出日志消息
        var extensionService = s_compositionHost.GetExport<ExtensionService>();
        var serviceLocator = s_compositionHost.GetExport<IServiceLocator>();
        AppxContext.InitializeLifetimeService(parms, serviceLocator);
        extensionService.LoadAutoLoaded(AutoLoadedType.BeforeExtensions);

        // 合并扩展组件的资源字典至主程序内
        var xamlControlsResources = new XamlControlsResources();
        var mergedResourceDictionaries = xamlControlsResources.MergedDictionaries;
        foreach (var rsrc in extensionService.GetMergedResourceDictionaries())
        {
            mergedResourceDictionaries.Add(rsrc);
        }

        // 修复 XamlControlsResources 访问冲突的异常
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        if (dispatcherQueue.HasThreadAccess)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                mainApp.Resources = xamlControlsResources;
            });
        }

        extensionService.LoadAutoLoaded(AutoLoadedType.AfterExtensions);
        extensionService.NotifyExtensions(ExtensionEvent.Loaded);
        extensionService.LoadAutoLoaded(AutoLoadedType.AfterExtensionsLoaded);
    }
}
#endif
