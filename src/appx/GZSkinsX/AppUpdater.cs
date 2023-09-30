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

namespace GZSkinsX;

internal sealed record class AppInfo(bool IsSupported, bool NeedUpdates, string UriString);

internal sealed class AppUpdater
{
    private static Uri[] OnlineApplicationInfos { get; } =
    [
        new("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/ApplicationData.json"),
        new("http://x1.gzskins.com/MounterV3/ApplicationData.json")
    ];

    public static async Task<AppInfo?> TryGetAppInfoAsync()
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

            return new AppInfo(isSupported, needUpdate, applicationData.MainApp);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.App.AppUpdater.TryGetAppInfoAsync",
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
        foreach (var uri in OnlineApplicationInfos)
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
