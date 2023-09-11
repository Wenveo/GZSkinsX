// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Extension;
using GZSkinsX.Contracts.Game;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;
using GZSkinsX.Extension;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppLifecycle;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Use the <see cref="Lazy{T}"/> to initialize the services for the current application.
    /// </summary>
    private static Lazy<Func<App, Task>> InitializeServiceAsync { get; } = new(() => async (app) =>
    {
        var gameService = AppxContext.Resolve<IGameService>();
        var rootFolder = await gameService.TryGetRootFolderAsync();
        await gameService.TryUpdateAsync(rootFolder, gameService.CurrentRegion);

        var extensionService = AppxContext.Resolve<ExtensionService>();
        extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.BeforeUniversalExtensions);
        foreach (var rsrc in extensionService.GetMergedResourceDictionaries())
        {
            app.Resources.MergedDictionaries.Add(rsrc);
        }

        extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AfterUniversalExtensions);
        extensionService.NotifyUniversalExtensions(UniversalExtensionEvent.Loaded);
        extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AfterUniversalExtensionsLoaded);
    });

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        CompositionTarget.Rendering += OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        CompositionTarget.Rendering -= OnRendering;

        var extensionService = AppxContext.Resolve<ExtensionService>();
        extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AppLoaded);
        extensionService.NotifyUniversalExtensions(UniversalExtensionEvent.AppLoaded);

        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Index_Guid);
    }

    internal void OnActivated(object? sender, AppActivationArguments e)
    {
        OnAppLaunch(e);
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        if (InitializeServiceAsync.IsValueCreated is false)
        {
            await InitializeServiceAsync.Value(this);
        }

        OnAppLaunch(AppInstance.GetCurrent().GetActivatedEventArgs());
    }

    private async void OnAppLaunch(AppActivationArguments activationArgs)
    {
        // Avoid marked as static
        Debug.Assert(_contentLoaded);

        // Handles activation args
        await AppxContext.ActivationService.ActivateAsync(activationArgs);

        // Ensure the current window is active
        AppxContext.AppxWindow.MainWindow.DispatcherQueue.TryEnqueue(AppxContext.AppxWindow.Activate);
    }
}
