// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;
using GZSkinsX.Kernel;

using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class PreloadPage : Page
{
    public PreloadPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        DispatcherQueue.GetForCurrentThread().EnqueueAsync(
            InitializeAsync, DispatcherQueuePriority.Low).FireAndForget();
    }

    private async Task InitializeAsync()
    {
        var b = await Package.Current.VerifyContentIntegrityAsync();
        var r = await Task.Run(KernelInterop.InitializeGZXKernelModule);

        if (b && r is 0)
        {
            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Main_Guid);
        }
        else
        {
            ShowCrashMessage("Preload_Crash_Failed_To_Verify_Content_Integrity".GetLocalized()!);
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
