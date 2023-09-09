// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using GZSkinsX.Appx.Logging;
using GZSkinsX.Contracts.Appx;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace GZSkinsX;

/// <summary>
/// 自定义的应用程序启动类。
/// </summary>
internal static partial class Program
{
    /// <summary>
    /// 当前组件容器的宿主的懒加载对象。
    /// </summary>
    private static readonly Lazy<CompositionHost> s_lazy_compositionHost;

    /// <summary>
    /// 获取内部的 <see cref="System.Composition.Hosting.CompositionHost"/> 公开实现。
    /// </summary>
    public static CompositionHost CompositionHost => s_lazy_compositionHost.Value;

    /// <summary>
    /// 初始化 <see cref="Program"/> 的静态成员。
    /// </summary>
    static Program()
    {
        s_lazy_compositionHost = new(() =>
        {
            var configuration = new ContainerConfiguration();
            configuration.WithAssemblies(GetAssemblies());
            return configuration.CreateContainer();
        });
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

                new App();
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

            // GZSkinsX.Appx.Contracts
            yield return typeof(AppxContext).Assembly;
        }

        Assembly asm;
        foreach (var filePath in Directory.EnumerateFiles(AppxContext.AppxDirectory.Path, "GZSkinsX.Appx.*.dll")
                .Where(a => a.IndexOf("GZSkinsX.Appx.Contracts.dll", StringComparison.OrdinalIgnoreCase) is -1))
        {
            try
            {
                asm = Assembly.LoadFile(filePath);
            }
            catch (Exception)
            {
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
         ref int packageFullNameLength, [Optional] char* packageFullName);
}
