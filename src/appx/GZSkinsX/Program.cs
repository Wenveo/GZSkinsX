// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using GZSkinsX.Composition;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Extension;
using GZSkinsX.Extension;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.Composition;
using Microsoft.Windows.AppLifecycle;

using Windows.Storage;

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
            Environment.Exit(MessageBoxW(0, $"请确保该应用已被正常安装。{Environment.NewLine}(Please ensure the application has been properly installed.)", string.Empty, 0));
        }

        ValidationProcessName();
        XamlCheckProcessRequirements();
        WinRT.ComWrappersSupport.InitializeComWrappers();

        if (!DecideRedirection().GetAwaiter().GetResult())
        {
            try
            {
                ProfileOptimization.SetProfileRoot(ApplicationData.Current.LocalCacheFolder.Path);
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
                    new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread()));

                mainApp = new();
                appInstance.Activated += mainApp.OnActivated;
            });

            if (mainApp is not null)
            {
                appInstance.Activated -= mainApp.OnActivated;
            }

            /// 通知应用程序退出的事件。
            extensionService.NotifyUniversalExtensions(UniversalExtensionEvent.AppExit);
        }
    }

    private static void ValidationProcessName()
    {
#if DEBUG
        if (Debugger.IsAttached)
        {
            return;
        }
#endif

        if (File.Exists(Environment.ProcessPath) is false)
        {
            return;
        }

        if (Path.GetFileName(Environment.ProcessPath) is not { } processName)
        {
            return;
        }

        if (StringComparer.OrdinalIgnoreCase.Equals(processName, "GZSkinsX.exe") is false)
        {
            return;
        }

        var workingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var lockFile = Path.Combine(workingDirectory, "lockfile.lock");
        if (File.Exists(lockFile))
        {
            File.SetAttributes(lockFile, System.IO.FileAttributes.Normal);
            var oldProcessName = File.ReadAllText(lockFile) + ".exe";
            try
            {
                File.Delete(Path.Combine(workingDirectory, oldProcessName));
            }
            catch
            {
            }
        }

        var newProcessName = Convert.ToHexString(Guid.NewGuid().ToByteArray());
        File.WriteAllText(lockFile, newProcessName, System.Text.Encoding.UTF8);
        File.SetAttributes(lockFile, System.IO.FileAttributes.Hidden);

        var newProcessPath = Path.Combine(workingDirectory, newProcessName + ".exe");
        File.Copy(Environment.ProcessPath, newProcessPath, overwrite: true);
        File.SetAttributes(newProcessPath, System.IO.FileAttributes.Hidden);
        Process.Start(new ProcessStartInfo() { FileName = newProcessPath });

        Environment.Exit(0);
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
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Program.GetAssemblies",
                    $"Failed to load appx extension assembly: \"{filePath}\". Message = \"{excp.Message}\".");

                continue;
            }

            yield return asm;
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
