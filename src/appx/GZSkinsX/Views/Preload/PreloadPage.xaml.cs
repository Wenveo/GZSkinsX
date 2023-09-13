// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel;


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
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += async (s, e) =>
        {
            var dispatcherQueue = (e.Argument as DispatcherQueue);
            Debug.Assert(dispatcherQueue is not null);

            // Try Initialize Module
            try
            {
                await AppxContext.MyModsService.UpdateSettingsAsync();
            }
            catch
            {
            }

            await dispatcherQueue.EnqueueAsync(async () =>
            {
                if (await Package.Current.VerifyContentIntegrityAsync())
                {
                    TryCheckUpdatesAsync().FireAndForget();
                    AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Main_Guid, true);
                }
                else
                {
                    ShowCrashMessage(ResourceHelper.GetLocalized("Resources/Preload_Crash_Failed_To_Verify_Content_Integrity"));
                    AppxContext.LoggingService.LogError("GZSkinsX.App.Views.PreloadPage.OnLoaded", $"Failed to verify content integrity.");
                }
            });
        };

        backgroundWorker.RunWorkerAsync(DispatcherQueue);
    }

    private void ShowCrashMessage(string message)
    {
        DefaultContent.Visibility = Visibility.Collapsed;
        CrashContent.Visibility = Visibility.Visible;

        CrashTextHost.Text = message;
    }

    private async Task ShowUnsupportedAppVersionDialogAsync(string uriString)
    {
        var contentDialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            DefaultButton = ContentDialogButton.Primary,
            Title = ResourceHelper.GetLocalized("Resources/Preload_Unsupported_AppVersion_Dialog_Title"),
            Content = ResourceHelper.GetLocalized("Resources/Preload_Unsupported_AppVersion_Dialog_Content"),
            CloseButtonText = ResourceHelper.GetLocalized("Resources/Preload_Unsupported_AppVersion_Dialog_Quit"),
            PrimaryButtonText = ResourceHelper.GetLocalized("Resources/Preload_Unsupported_AppVersion_Dialog_GetUpdates")
        };

        var result = await contentDialog.ShowAsync();
        if (result is ContentDialogResult.Primary)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(uriString));
        }

        Application.Current.Exit();
    }

    private async Task ShowAvailableUpdateDialogAsync(string uriString)
    {
        var contentDialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            DefaultButton = ContentDialogButton.Primary,
            Title = ResourceHelper.GetLocalized("Resources/Preload_Available_Update_Dialog_Title"),
            Content = ResourceHelper.GetLocalized("Resources/Preload_Available_Update_Dialog_Content"),
            CloseButtonText = ResourceHelper.GetLocalized("Resources/Common_Dialog_Cancel"),
            PrimaryButtonText = ResourceHelper.GetLocalized("Resources/Common_Dialog_OK")
        };

        var result = await contentDialog.ShowAsync();
        if (result is ContentDialogResult.Primary)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(uriString));
        }

        AppxContext.Resolve<PreloadSettings>().DontNeedShowTheAvailableUpdateDialog = true;
    }

    private async Task TryCheckUpdatesAsync()
    {
        var updateInfo = await AppUpdater.TryGetAppInfoAsync();
        if (updateInfo.IsEmpty)
        {
            return;
        }

        var settings = AppxContext.Resolve<PreloadSettings>();
        if (updateInfo.NeedUpdates is false)
        {
            // Reset
            if (settings.DontNeedShowTheAvailableUpdateDialog)
            {
                settings.DontNeedShowTheAvailableUpdateDialog = false;
            }

            return;
        }

        if (updateInfo.IsSupported is false)
        {
            await ShowUnsupportedAppVersionDialogAsync(updateInfo.UriString);
        }
        else
        {
            if (settings.DontNeedShowTheAvailableUpdateDialog is false)
            {
                await ShowAvailableUpdateDialogAsync(updateInfo.UriString);
            }
        }
    }
}
