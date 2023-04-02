// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.CodeDom.Compiler;
using System.Composition.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Api.Scripting;
using GZSkinsX.Extension;
using GZSkinsX.Logging;

using Windows.UI.Xaml;

namespace GZSkinsX;

#if DISABLE_XAML_GENERATED_MAIN
/// <summary>
/// �Զ���ĳ���������
/// </summary>
internal static partial class Program
{
    /// <summary>
    /// ��ǰ�������������ʵ��
    /// </summary>
    private static readonly CompositionHost s_compositionHost;

    /// <summary>
    /// ��ȡ�ڲ��� <see cref="global::System.Composition.Hosting.CompositionHost"/> ����ʵ��
    /// </summary>
    public static CompositionHost CompositionHost => s_compositionHost;

    /// <summary>
    /// ��ʼ�� <see cref="Program"/> �ľ�̬��Ա
    /// </summary>
    static Program()
    {
        var configuration = new ContainerConfiguration();
        configuration.WithAssemblies(GetAssemblies());
        s_compositionHost = configuration.CreateContainer();
    }

    [GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", " 0.0.0.0")]
    [DebuggerNonUserCodeAttribute]
    public static void Main(string[] args)
    {
        Application.Start(async (p) =>
        {
            await InitializeServicesAsync(p, new App());
        });
    }

    /// <summary>
    /// ���غͳ�ʼ��Ӧ�ó���Ļ�������
    /// </summary>
    /// <param name="parms">Ӧ�ó����ʼ��ʱ�Ĳ���</param>
    /// <param name="mainApp">���ڳ�ʼ����Ӧ�ó���</param>
    private static async Task InitializeServicesAsync(ApplicationInitializationCallbackParams parms, App mainApp)
    {
        await LoggerImpl.Shared.InitializeAsync();

        var extensionService = s_compositionHost.GetExport<ExtensionService>();
        var serviceLocator = s_compositionHost.GetExport<IServiceLocator>();
        AppxContext.InitializeLifetimeService(parms, serviceLocator);

        extensionService.LoadAutoLoaded(AutoLoadedType.BeforeExtensions);
        foreach (var rsrc in extensionService.GetMergedResourceDictionaries())
        {
            mainApp.Resources.MergedDictionaries.Add(rsrc);
        }

        extensionService.LoadAutoLoaded(AutoLoadedType.AfterExtensions);
        extensionService.NotifyExtensions(ExtensionEvent.Loaded);
        extensionService.LoadAutoLoaded(AutoLoadedType.AfterExtensionsLoaded);
    }
}
#endif