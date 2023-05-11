// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.WindowManager;

using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.MainApp;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        var appxWindow = AppxContext.AppxWindow;
        if (appxWindow.MainWindow.Content is not Frame frame || frame.Content is null)
        {
            // 合并扩展组件的资源字典至主程序内
            var xamlControlsResources = Resources = new XamlControlsResources();
            var mergedResourceDictionaries = xamlControlsResources.MergedDictionaries;
            foreach (var rsrc in StartUpClass.s_extensionService.GetMergedResourceDictionaries())
            {
                mergedResourceDictionaries.Add(rsrc);
            }

            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);
        }

        if (appxWindow.MainWindow.Content is FrameworkElement frameworkElement)
        {
            var themeService = AppxContext.ThemeService;
            frameworkElement.RequestedTheme = themeService.CurrentTheme;
        }

        if (e.PrelaunchActivated == false)
        {
            // 如果启用了 Prelaunch 就可能会跳过 StartUpClass 里的 InitializeServices，
            // 从而导致上面的 "var appxWindow = AppxContext.AppxWindow;" 这段代码触发异常。
            // 目前暂时不做这个处理，并注释掉以下代码。

            //if (ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch"))
            //{
            //    CoreApplication.EnablePrelaunch(true);
            //}

            appxWindow.Activate();
        }
    }
}
