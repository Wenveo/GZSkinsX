// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

using GZSkinsX.Composition;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Extension;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.Composition;
using Microsoft.Windows.AppLifecycle;

namespace GZSkinsX;

/// <summary>
/// 自定义的应用程序启动类。
/// </summary>
internal static partial class Program
{
    // Causes incorrect output in the message window.
    [LibraryImport("User32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int MessageBoxW(nint hWnd, string lpText, string lpCaption, uint uType);

    [STAThread]
    private static void Main(string[] args)
    {
        if (EnsureWindowsApp() is false)
        {
            Environment.Exit(MessageBoxW(0, $"请确保该应用已被正常安装。{Environment.NewLine}(Make sure the application is installed correctly.)", string.Empty, 0));
        }

        XamlCheckProcessRequirements();
        WinRT.ComWrappersSupport.InitializeComWrappers();

        if (!DecideRedirection().GetAwaiter().GetResult())
        {
            try
            {
                ProfileOptimization.SetProfileRoot(AppxContext.LocalCacheFolder);
                ProfileOptimization.StartProfile("startup.profile");
            }
            catch
            {
            }

            App? mainApp = null;

            var appInstance = AppInstance.GetCurrent();
            var exportProvider = InitializeMEFV2().GetAwaiter().GetResult();
            AppxContext.InitializeLifetimeService(new ServiceResolver(exportProvider));

            /// 在此获取和加载扩展服务的实例，而不是在 Application.Start 内。
            var extensionService = exportProvider.GetExportedValue<ExtensionService>();
            Application.Start((p) =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherQueueSynchronizationContextX(
                        DispatcherQueue.GetForCurrentThread()));

                mainApp = new();
                appInstance.Activated += mainApp.OnActivated;
            });

            if (mainApp is not null)
            {
                appInstance.Activated -= mainApp.OnActivated;
            }

            /// 通知应用程序退出的事件。
            extensionService.OnAppExit();
        }
    }

    /// <summary>
    /// 初始化 MEF 组件。
    /// </summary>
    private static async Task<ExportProvider> InitializeMEFV2()
    {
        var catalogV2 = new AssemblyCatalogV2()
            .AddParts(GetAssemblies())
            .AddParts(GetExtensionAssemblies());

        var containerV2 = new CompositionContainerV2(catalogV2);
        return await containerV2.CreateExportProviderAsync(true);
    }

    /// <summary>
    /// 获取当前 Appx 的扩展程序集。
    /// </summary>
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
        foreach (var filePath in Directory.EnumerateFiles(AppxContext.AppxDirectory, "GZSkinsX.Appx.*.dll")
                .Where(a => a.IndexOf("GZSkinsX.Appx.Contracts.dll", StringComparison.OrdinalIgnoreCase) is -1))
        {
            try
            {
                var asmName = AssemblyLoadContext.GetAssemblyName(filePath);
                asm = AssemblyLoadContext.Default.LoadFromAssemblyName(asmName);
            }
            catch (BadImageFormatException)
            {
                continue;
            }
            catch (FileNotFoundException)
            {
                continue;
            }
            catch (FileLoadException)
            {
                continue;
            }

            yield return asm;
        }
    }

    /// <summary>
    /// 获取当前应用目录中的扩展程序集。
    /// </summary>
    private static IEnumerable<Assembly> GetExtensionAssemblies()
    {
        Assembly asm;
        foreach (var filePath in Directory.EnumerateFiles(AppxContext.AppxDirectory, "*.x.dll"))
        {
            try
            {
                var asmName = AssemblyLoadContext.GetAssemblyName(filePath);
                asm = AssemblyLoadContext.Default.LoadFromAssemblyName(asmName);
            }
            catch (BadImageFormatException)
            {
                continue;
            }
            catch (FileNotFoundException)
            {
                continue;
            }
            catch (FileLoadException)
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

    /// <summary>
    /// 默认的服务解析器实现。
    /// </summary>
    private sealed class ServiceResolver(ExportProvider exportProvider) : IServiceResolver
    {
        /// <inheritdoc/>
        public T Resolve<T>() where T : class
        {
            var type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                Debug.Assert(type.GenericTypeArguments.Length == 1);
                var itemType = type.GenericTypeArguments.Single();
                if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(Lazy<,>))
                {
                    var genericTypes = itemType.GenericTypeArguments;
                    Debug.Assert(genericTypes != null && genericTypes.Length == 2);
                    return Unsafe.As<T>(exportProvider.GetExports(genericTypes[0], genericTypes[1], null));
                }

                return Unsafe.As<T>(exportProvider.GetExportedValues(itemType, null));
            }

            return exportProvider.GetExportedValue<T>();
        }

        /// <inheritdoc/>
        public bool TryResolve<T>([NotNullWhen(true)] out T? value) where T : class
        {
            try
            {
                value = Resolve<T>();
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }
}
