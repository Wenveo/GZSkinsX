// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.WindowManager;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Views.WindowFrames.Preload;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PreloadPage : Page
{
    public PreloadPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var b = await Package.Current.VerifyContentIntegrityAsync();
        if (b)
        {
            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.StartUp_Guid);
        }
        else
        {
            ShowCrashMessage(ResourceHelper.GetLocalized("Resources/Preload_Crash_Failed_To_Verify_Content_Integrity"));
            AppxContext.LoggingService.LogError($"AppxPreload: Failed to verify content integrity.");
        }
    }

    private void ShowCrashMessage(string message)
    {
        DefaultContent.Visibility = Visibility.Collapsed;
        CrashContent.Visibility = Visibility.Visible;

        CrashTextHost.Text = message;
    }
}
