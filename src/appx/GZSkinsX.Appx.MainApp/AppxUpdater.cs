// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Settings;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.MainApp;

internal static class AppxUpdater
{
    private static ApplicationManifest? s_appManifest;

    public static async Task<bool?> IsRequiredUpdates()
    {
        var appManifest = s_appManifest ??= await ApplicationManifest.TryGetManifestAsync();
        if (appManifest is null)
        {
            return null;
        }

        return appManifest.IsSupported is false;
    }

    public static async Task ShowDialogIfNeedUpdatesAsync()
    {
        var appManifest = s_appManifest ??= await ApplicationManifest.TryGetManifestAsync();
        if (appManifest is null)
        {
            return;
        }

        var settings = AppxContext.Resolve<AppxUpdaterSettings>();
        if (appManifest.NeedUpdates is false)
        {
            // Reset
            if (settings.DontNeedShowTheAvailableUpdateDialog)
            {
                settings.DontNeedShowTheAvailableUpdateDialog = false;
            }

            return;
        }

        if (appManifest.IsSupported is false)
        {
            await ShowUnsupportedAppVersionDialogAsync(appManifest.UriString);
        }
        else
        {
            if (settings.DontNeedShowTheAvailableUpdateDialog is false)
            {
                await ShowAvailableUpdateDialogAsync(appManifest.UriString);
            }
        }
    }

    private static async Task ShowUnsupportedAppVersionDialogAsync(string uriString)
    {
        var contentDialog = new ContentDialog
        {
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = AppxContext.AppxWindow.MainWindow.Content.XamlRoot,
            Title = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/AppxUpdater_Unsupported_AppVersion_Dialog_Title"),
            Content = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/AppxUpdater_Unsupported_AppVersion_Dialog_Content"),
            CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/AppxUpdater_Unsupported_AppVersion_Dialog_CloseButtonText"),
            PrimaryButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/AppxUpdater_Unsupported_AppVersion_Dialog_PrimaryButtonText")
        };

        var result = await contentDialog.ShowAsync();
        if (result is ContentDialogResult.Primary)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(uriString));
        }

        Application.Current.Exit();
    }

    private static async Task ShowAvailableUpdateDialogAsync(string uriString)
    {
        var contentDialog = new ContentDialog
        {
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = AppxContext.AppxWindow.MainWindow.Content.XamlRoot,
            Title = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/AppxUpdater_Available_Update_Dialog_Title"),
            Content = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/AppxUpdater_Available_Update_Dialog_Content"),
            CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/AppxUpdater_Available_Update_Dialog_CloseButtonText"),
            PrimaryButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/AppxUpdater_Available_Update_Dialog_PrimaryButtonText")
        };

        var result = await contentDialog.ShowAsync();
        if (result is ContentDialogResult.Primary)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(uriString));
        }

        AppxContext.Resolve<AppxUpdaterSettings>().DontNeedShowTheAvailableUpdateDialog = true;
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
                    "GZSkinsX.Appx.MainApp.ApplicationManifest.TryGetManifestAsync",
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

    [Shared, Export]
    private sealed class AppxUpdaterSettings
    {
        private const string THE_GUID = "99092BD3-A425-403C-9EFA-A5C126B77F88";
        private const string DONT_NEED_SHOW_THE_AVAILABLE_UPDATE_DIALOG_NAME = "DontNeedShowTheAvailableUpdateDialog";

        private readonly ISettingsSection _settingsSection;

        public bool DontNeedShowTheAvailableUpdateDialog
        {
            get => _settingsSection.Attribute<bool>(DONT_NEED_SHOW_THE_AVAILABLE_UPDATE_DIALOG_NAME);
            set => _settingsSection.Attribute(DONT_NEED_SHOW_THE_AVAILABLE_UPDATE_DIALOG_NAME, value);
        }

        public AppxUpdaterSettings()
        {
            _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
        }
    }

}
