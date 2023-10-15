// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Net.Http;
using System.Net.Http.Json;
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
            TryCheckUpdatesAsync().FireAndForget();
            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Main_Guid, true);
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

    private async Task ShowUnsupportedAppVersionDialogAsync(string uriString)
    {
        var contentDialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            DefaultButton = ContentDialogButton.Primary,
            Title = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Unsupported_AppVersion_Dialog_Title"),
            Content = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Unsupported_AppVersion_Dialog_Content"),
            CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Unsupported_AppVersion_Dialog_CloseButtonText"),
            PrimaryButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Unsupported_AppVersion_Dialog_PrimaryButtonText")
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
            Title = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Available_Update_Dialog_Title"),
            Content = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Available_Update_Dialog_Content"),
            CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Available_Update_Dialog_CloseButtonText"),
            PrimaryButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/Preload_Available_Update_Dialog_PrimaryButtonText")
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
        var updateInfo = await ApplicationManifest.TryGetManifestAsync();
        if (updateInfo is null)
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

    private sealed record class ApplicationManifest(bool IsSupported, bool NeedUpdates, string UriString)
    {
        private static Uri[] OnlineApplicationManifests { get; } =
        [
            new("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/ApplicationData.json"),
            new("http://x1.gzskins.com/MounterV3/ApplicationData.json")
        ];

        public static async Task<ApplicationManifest?> TryGetManifestAsync()
        {
            using var httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            });

            try
            {
                var applicationData = await DownloadApplicationInfoAsync(httpClient);
                var needUpdate = Version.Parse(applicationData.AppVersion) != AppxContext.AppxVersion;
                var isSupported = Version.Parse(applicationData.SupportedMinVersion) <= AppxContext.AppxVersion;

                return new(isSupported, needUpdate, applicationData.MainApp);
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MainApp.Views.PreloadPage.ApplicationManifest.TryGetManifestAsync",
                    $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

                return null;
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        private static async Task<ApplicationInfo> DownloadApplicationInfoAsync(HttpClient httpClient)
        {
            foreach (var uri in OnlineApplicationManifests)
            {
                var result = await httpClient.GetFromJsonAsync<ApplicationInfo>(uri);
                if (result is null || string.IsNullOrEmpty(result.MainApp))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(result.AppVersion))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(result.SupportedMinVersion))
                {
                    continue;
                }

                return result;
            }

            throw new IndexOutOfRangeException();
        }

        private sealed record class ApplicationInfo(string MainApp, string AppVersion, string SupportedMinVersion) { }
    }
}
