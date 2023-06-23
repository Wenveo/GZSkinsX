// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;

using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.MainApp;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static Window MainWindow { get; } = new Window();

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        MainWindow.VisibilityChanged += OnVisibilityChanged;
        InitializeComponent();
    }

    private void OnVisibilityChanged(object sender, WindowVisibilityChangedEventArgs args)
    {
        if (args.Visible)
        {
            MainWindow.VisibilityChanged -= OnVisibilityChanged;

            var extensionService = StartUpClass.ExtensionService;
            extensionService.NotifyUniversalExtensions(UniversalExtensionEvent.AppLoaded);
            extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AppLoaded);
        }
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppxContext.AppxWindow.Activate();
    }
}
