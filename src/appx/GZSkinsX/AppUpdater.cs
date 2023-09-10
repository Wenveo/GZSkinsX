// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;

using Windows.Data.Json;
using Windows.Web.Http;

namespace GZSkinsX;

internal readonly struct AppInfo
{
    public static readonly AppInfo Empty = new();

    public readonly bool IsSupported;

    public readonly bool NeedUpdates;

    public readonly string UriString;

    public readonly bool IsEmpty;

    public AppInfo()
    {
        UriString = string.Empty;
        IsEmpty = true;
    }

    public AppInfo(bool isSupported, bool needUpdates, string uriString)
    {
        IsSupported = isSupported;
        NeedUpdates = needUpdates;
        UriString = uriString;
        IsEmpty = false;
    }
}

internal sealed class AppUpdater
{
    private static Uri[] OnlineApplicationInfos { get; } = new Uri[]
    {
        new Uri("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/ApplicationData.json"),
        new Uri("http://x1.gzskins.com/MounterV3/ApplicationData.json")
    };

    public static async ValueTask<AppInfo> TryGetAppInfoAsync()
    {
        var httpClient = new HttpClient();

        try
        {
            var applicationData = await DownloadApplicationInfoAsync(httpClient);

            var needUpdate = Version.Parse(applicationData.AppVersion) != AppxContext.AppxVersion;
            var isSupported = Version.Parse(applicationData.SupportedMinVersion) <= AppxContext.AppxVersion;

            return new AppInfo(isSupported, needUpdate, applicationData.MainApp);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.App.AppUpdater.TryGetAppInfoAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

            return AppInfo.Empty;
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    private static async Task<ApplicationInfo> DownloadApplicationInfoAsync(HttpClient httpClient)
    {
        foreach (var uri in OnlineApplicationInfos)
        {
            using var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonObject.Parse(result);

            return new ApplicationInfo(
                jsonObject[nameof(ApplicationInfo.MainApp)].GetString(),
                jsonObject[nameof(ApplicationInfo.AppVersion)].GetString(),
                jsonObject[nameof(ApplicationInfo.SupportedMinVersion)].GetString());
        }

        throw new IndexOutOfRangeException();
    }


    private readonly struct ApplicationInfo
    {
        public static readonly ApplicationInfo Empty = new();

        public readonly string MainApp;

        public readonly string AppVersion;

        public readonly string SupportedMinVersion;

        public readonly bool IsEmpty;

        public ApplicationInfo()
        {
            MainApp = string.Empty;
            AppVersion = string.Empty;
            SupportedMinVersion = string.Empty;
            IsEmpty = true;
        }

        public ApplicationInfo(string mainApp, string appVersion, string supportedMinVersion)
        {
            MainApp = mainApp;
            AppVersion = appVersion;
            SupportedMinVersion = supportedMinVersion;
            IsEmpty = false;
        }
    }
}
