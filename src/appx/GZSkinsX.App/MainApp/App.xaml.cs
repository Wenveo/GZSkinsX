// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.WindowManager;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.MainApp;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static Window MainWindow { get; } = new Window { SystemBackdrop = new MicaBackdrop() };

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppxContext.AppxWindow.Activate();

        if (_commonResourceDictionary is null)
        {
            _commonResourceDictionary = new ResourceDictionary();

            var mergedResourceDictionaries = _commonResourceDictionary.MergedDictionaries;
            foreach (var rsrc in StartUpClass.ExtensionService.GetMergedResourceDictionaries())
            {
                mergedResourceDictionaries.Add(rsrc);
            }

            Resources.MergedDictionaries.Add(_commonResourceDictionary);
        }

        if (MainWindow.Content is not Frame frame || frame.Content is null)
        {
            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);
        }

        if (MainWindow.Content is FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = AppxContext.ThemeService.CurrentTheme;
        }
    }

    private ResourceDictionary? _commonResourceDictionary;
}
