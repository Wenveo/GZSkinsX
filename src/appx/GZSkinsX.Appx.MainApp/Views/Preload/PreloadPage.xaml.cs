// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.MainApp.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class PreloadPage : Page
{
    public PreloadPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Try Initialize Module
        try
        {
            await Task.Run(AppxContext.KernelService.InitializeModule);
        }
        catch
        {
        }

        if (await Package.Current.VerifyContentIntegrityAsync())
        {
            AppxUpdater.ShowDialogIfNeedUpdatesAsync().FireAndForget();
            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Main_Guid);
        }
        else
        {
            ShowCrashMessage(ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Crash_Failed_To_Verify_Content_Integrity"));
            AppxContext.LoggingService.LogError("GZSkinsX.Appx.MainApp.Views.PreloadPage.OnLoaded", $"Failed to verify content integrity.");
        }
    }

    private void ShowCrashMessage(string message)
    {
        DefaultContent.Visibility = Visibility.Collapsed;
        CrashContent.Visibility = Visibility.Visible;

        CrashTextHost.Text = message;
    }
}
