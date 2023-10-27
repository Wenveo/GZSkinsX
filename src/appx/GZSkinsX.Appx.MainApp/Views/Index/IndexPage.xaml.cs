// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.MainApp.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class IndexPage : Page
{
    private Exception? _innerException;

    public IndexPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (AppxContext.KernelService.VerifyModuleIntegrity())
            {
                AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.StartUp_Guid);
                Task.Run(AppxContext.KernelService.UpdateManifestAsync).FireAndForget();
                return;
            }
        }
        catch
        {
        }

        ContentGrid.Visibility = Visibility.Visible;
        if ((await AppxUpdater.IsRequiredUpdates()) is true)
        {
            await AppxUpdater.ShowDialogIfNeedUpdatesAsync();
        }
        else
        {
            await StartDownloadAsync();
        }
    }

    private async Task StartDownloadAsync()
    {
        Index_ShowError_Button.Visibility = Visibility.Collapsed;
        Index_Downloading_Title.Visibility = Visibility.Visible;
        Index_Downloading_ProgressBar.Visibility = Visibility.Visible;
        Index_Downloading_ProgressBar.IsIndeterminate = true;
        Index_Downloading_ProgressBar.ShowError = false;

        try
        {
            await AppxContext.KernelService.UpdateModuleAsync(new Progress<double>(async (d) =>
            {
                await DispatcherQueue.EnqueueAsync(() =>
                {
                    Index_Downloading_ProgressBar.IsIndeterminate = false;
                    Index_Downloading_ProgressBar.Value = d * 100;
                });
            }));

            await Task.Delay(200);

            Index_Downloading_Title.Visibility = Visibility.Collapsed;
            Index_PendingRestart_Title.Visibility = Visibility.Visible;
        }
        catch (Exception excp)
        {
            _innerException = excp;

            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MainApp.Views.IndexPage.StartDownloadAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

            Index_ShowError_Button.Visibility = Visibility.Visible;
            Index_Downloading_Title.Visibility = Visibility.Collapsed;
            Index_DownloadFailed_Title.Visibility = Visibility.Visible;
            Index_Downloading_ProgressBar.ShowError = true;
        }
    }

    private async void ShowError_Click(object sender, RoutedEventArgs e)
    {
        var innerException = _innerException;
        if (innerException is null)
        {
            return;
        }

        await new ContentDialog()
        {
            XamlRoot = XamlRoot,
            Content = innerException.StackTrace,
            Title = $"{innerException}: {innerException.Message}",
            DefaultButton = ContentDialogButton.Close,
            CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Index_ShowError_Dialog_CloseText")
        }.ShowAsync();
    }
}
