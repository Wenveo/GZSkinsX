// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
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

            AppxContext.InitializeLifetimeService(exportProvider);

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
    /// 获取当前 Appx 的扩展程序集
    /// </summary>
    private static IEnumerable<Assembly> GetAssemblies()
    {
        // Main Appx
        {
            // Self Assembly
            yield return typeof(App).Assembly;

            // GZSkinsX.Appx.AccessCache
            yield return typeof(Appx.AccessCache.AppxContract).Assembly;

            // GZSkinsX.Appx.Activation
            yield return typeof(Appx.Activation.AppxContract).Assembly;

            // GZSkinsX.Appx.Command
            yield return typeof(Appx.Command.AppxContract).Assembly;

            // GZSkinsX.Appx.ContextMenu
            yield return typeof(Appx.ContextMenu.AppxContract).Assembly;

            // GZSkinsX.Appx.Contracts
            yield return typeof(Contracts.AppxContract).Assembly;

            // GZSkinsX.Appx.Game
            yield return typeof(Appx.Game.AppxContract).Assembly;

            // GZSkinsX.Appx.Kernel
            yield return typeof(Appx.Kernel.AppxContract).Assembly;

            // GZSkinsX.Appx.Logging
            yield return typeof(Appx.Logging.AppxContract).Assembly;

            // GZSkinsX.Appx.MainApp
            yield return typeof(Appx.MainApp.AppxContract).Assembly;

            // GZSkinsX.Appx.MotClient
            yield return typeof(Appx.MotClient.AppxContract).Assembly;

            // GZSkinsX.Appx.MRTCore
            yield return typeof(Appx.MRTCore.AppxContract).Assembly;

            // GZSkinsX.Appx.MyMods
            yield return typeof(Appx.MyMods.AppxContract).Assembly;

            // GZSkinsX.Appx.Navigation
            yield return typeof(Appx.Navigation.AppxContract).Assembly;

            // GZSkinsX.Appx.Settings
            yield return typeof(Appx.Settings.AppxContract).Assembly;

            // GZSkinsX.Appx.Themes
            yield return typeof(Appx.Themes.AppxContract).Assembly;

            // GZSkinsX.Appx.WindowManager
            yield return typeof(Appx.WindowManager.AppxContract).Assembly;
        }
    }

    /// <summary>
    /// 获取当前应用程序的外部扩展程序集
    /// </summary>
    private static IEnumerable<Assembly> GetExtensionAssemblies()
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var pluginsFolder = Path.Combine(userProfile, ".gzskinsx");
        if (Directory.Exists(pluginsFolder) is false)
        {
            yield break;
        }

        Assembly asm;
        foreach (var filePath in Directory.EnumerateFiles(pluginsFolder, "*.x.dll", SearchOption.AllDirectories))
        {
            try
            {
                asm = Assembly.LoadFile(filePath);
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Program.GetExtensionAssemblies",
                    $"Failed to load extern extension assembly: \"{filePath}\". Message = \"{excp.Message}\".");

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
