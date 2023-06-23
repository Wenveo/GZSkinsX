// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Extension;
using GZSkinsX.Logging;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace GZSkinsX.MainApp;

#if DISABLE_XAML_GENERATED_MAIN
/// <summary>
/// 自定义的应用程序启动类
/// </summary>
internal static partial class StartUpClass
{
    /// <summary>
    /// 当前组件容器的宿主的懒加载对象
    /// </summary>
    private static readonly Lazy<CompositionHost> s_lazy_compositionHost;

    /// <summary>
    /// 获取内部的 <see cref="System.Composition.Hosting.CompositionHost"/> 公开实现
    /// </summary>
    public static CompositionHost CompositionHost => s_lazy_compositionHost.Value;

    /// <summary>
    /// 当前扩展服务的懒加载对象
    /// </summary>
    private static Lazy<ExtensionService> s_lazy_extensionService;

    /// <summary>
    /// 获取内部的 <see cref="Extension.ExtensionService"/> 静态成员实例
    /// </summary>
    public static ExtensionService ExtensionService => s_lazy_extensionService.Value;

    /// <summary>
    /// 初始化 <see cref="StartUpClass"/> 的静态成员
    /// </summary>
    static StartUpClass()
    {
        s_lazy_compositionHost = new(() =>
        {
            var configuration = new ContainerConfiguration();
            configuration.WithAssemblies(GetAssemblies());
            return configuration.CreateContainer();
        });

        s_lazy_extensionService = new(s_lazy_compositionHost.Value.GetExport<ExtensionService>);
    }

    [STAThread]
    private static async Task<int> Main(string[] args)
    {
        XamlCheckProcessRequirements();
        WinRT.ComWrappersSupport.InitializeComWrappers();

        if (EnsureWindowsApp() && !await DecideRedirection())
        {
            /// 在应用程序启动之前进行日志器的初始化操作
            /// 这样能够大幅减少日志器的初始化所需的时间
            await LoggerImpl.Shared.InitializeAsync();

            Application.Start((p) =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread()));

                AppxContext.InitializeLifetimeService(p, CompositionHost);

                ExtensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.BeforeUniversalExtensions);
                ExtensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AfterUniversalExtensions);

                new App();

                ExtensionService.NotifyUniversalExtensions(UniversalExtensionEvent.Loaded);
                ExtensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AfterUniversalExtensionsLoaded);
            });

            LoggerImpl.Shared.CloseOutputStream();
        }

        return 0;
    }

    private static IEnumerable<Assembly> GetAssemblies()
    {
        // Main Appx
        {
            // Self Assembly
            yield return typeof(App).Assembly;
            // GZSkinsX.Api
            yield return typeof(IAppxWindow).Assembly;
        }

        Assembly asm;
        foreach (var filePath in Directory.EnumerateFiles(AppxContext.AppxDirectory, "GZSkinsX.Appx.*.dll"))
        {
            try
            {
                asm = Assembly.LoadFile(filePath);
            }
            catch (Exception e)
            {
                AppxContext.LoggingService.LogError($"Failed to load extension assembly: '{filePath}'. Message = '{e.Message}'");
                continue;
            }

            yield return asm;
        }
    }

    private static async Task<bool> DecideRedirection()
    {
        var mainInstance = AppInstance.FindOrRegisterForKey("main");

        // If the instance that's executing the OnLaunched handler right now
        // isn't the "main" instance.
        if (!mainInstance.IsCurrent)
        {
            // Redirect the activation (and args) to the "main" instance, and exit.
            var activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            await mainInstance.RedirectActivationToAsync(activatedEventArgs);
        }

        return !mainInstance.IsCurrent;
    }

    private static unsafe bool EnsureWindowsApp()
    {
        var length = 0;
        return GetCurrentPackageFullName(ref length, null) != 15700L;
    }

    [LibraryImport("Microsoft.ui.xaml.dll")]
    private static partial void XamlCheckProcessRequirements();

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static unsafe partial int GetCurrentPackageFullName(
        ref int packageFullNameLength, char* packageFullName);
}
#endif
