// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

using GZSkinsX.Api.AccessCache;
using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.Game;

using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

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
    /// Invoked when the application creates a window.
    /// </summary>
    /// <param name="args">Event data for the event.</param>
    protected override async void OnWindowCreated(WindowCreatedEventArgs args)
    {
        async static Task InitializeServicesAsync()
        {
            var gameService = (GameService)AppxContext.GameService;
            gameService.RootFolder = await AppxContext.FutureAccessService.TryGetFolderAsync(FutureAccessItemConstants.Game_RootFolder_Name);
        }

        var appxWindow = AppxContext.AppxWindow;
        if (appxWindow.MainWindow == args.Window)
        {
            await InitializeServicesAsync();

            var extensionService = StartUpClass.s_extensionService;
            extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.BeforeUniversalExtensions);

            // 合并扩展组件的资源字典至主程序内
            var xamlControlsResources = Resources = new XamlControlsResources();
            foreach (var rsrc in StartUpClass.s_extensionService.GetMergedResourceDictionaries())
            {
                xamlControlsResources.MergedDictionaries.Add(rsrc);
            }

            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Index_Guid);

            if (appxWindow.MainWindow.Content is FrameworkElement frameworkElement)
            {
                var themeService = AppxContext.ThemeService;
                frameworkElement.RequestedTheme = themeService.CurrentTheme;
            }

            extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AfterUniversalExtensions);
            extensionService.NotifyUniversalExtensions(UniversalExtensionEvent.Loaded);
            extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AfterUniversalExtensionsLoaded);
        }

        base.OnWindowCreated(args);
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (args.PrelaunchActivated == false)
        {
            if (ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch"))
            {
                CoreApplication.EnablePrelaunch(true);
            }

            AppxContext.AppxWindow.Activate();
        }

        base.OnLaunched(args);
    }
}
