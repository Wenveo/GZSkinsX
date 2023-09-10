// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics;

using GZSkinsX.Contracts.Appx;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
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
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    internal void OnActivated(object? sender, AppActivationArguments e)
    {
        OnAppLaunch(e);
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        OnAppLaunch(AppInstance.GetCurrent().GetActivatedEventArgs());
    }

    private void OnAppLaunch(AppActivationArguments activationArgs)
    {
        // Avoid marked as static
        Debug.Assert(_contentLoaded);

        AppxContext.AppxWindow.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            // Handles activation args
            await AppxContext.ActivationService.ActivateAsync(activationArgs);

            // Ensure the current window is active
            AppxContext.AppxWindow.Activate();
        });
    }
}
