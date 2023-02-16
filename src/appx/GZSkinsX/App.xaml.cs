// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Reflection;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Composition;
using GZSkinsX.Extension;

using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.Composition;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <inheritdoc/>
    public App()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var catalog = new AssemblyCatalogV2()
            .AddParts(GetAssemblies())
            .AddParts(GetExtensionAssemblies());

        var container = new CompositionContainerV2(catalog);
        var exportProvider = await container.CreateExportProviderAsync(true);

        InitializeServices(exportProvider);

        var appWindow = exportProvider.GetExportedValue<IAppxWindow>();
        appWindow.Show();
    }

    /// <summary>
    /// 初始化应用程序核心服务
    /// </summary>
    /// <param name="exportProvider"><see cref="ExportProvider"/> 对象的实例</param>
    private void InitializeServices(ExportProvider exportProvider)
    {
        var serviceLocator = exportProvider.GetExportedValue<ServiceLocator>();
        serviceLocator.SetExportProvider(exportProvider);

        InitializeExtension(exportProvider.GetExportedValue<ExtensionService>());
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

    /// <summary>
    /// 获取当前 Appx 引用程序集
    /// </summary>
    private static IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(App).Assembly;
        yield return typeof(IAppxWindow).Assembly;
    }

    /// <summary>
    /// 获取当前 Appx 的扩展程序集
    /// </summary>
    private static IEnumerable<Assembly> GetExtensionAssemblies()
    {
        var root = new DirectoryInfo(AppxContext.AppxDirectory);
        foreach (var appx in root.GetFiles("GZSkinsX.Appx.*.dll", SearchOption.TopDirectoryOnly))
        {
            yield return Assembly.LoadFrom(appx.FullName);
        }

        foreach (var exAsm in root.GetFiles("*.x.dll", SearchOption.AllDirectories))
        {
            yield return Assembly.LoadFrom(exAsm.FullName);
        }
    }
}
