// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;

using Windows.Data.Json;
using Windows.Web.Http;

namespace GZSkinsX;

internal readonly struct UpdateInfo
{
    public static readonly UpdateInfo Empty = new();

    public readonly bool NeedUpdates;

    public readonly bool IsSupported;

    public readonly string UriString;

    public readonly bool IsEmpty;

    public UpdateInfo()
    {
        NeedUpdates = false;
        IsSupported = false;
        UriString = string.Empty;
        IsEmpty = true;
    }

    public UpdateInfo(bool needUpdates, bool isSupported, string uriString)
    {
        NeedUpdates = needUpdates;
        IsSupported = isSupported;
        UriString = uriString;
        IsEmpty = false;
    }
}

[Shared, Export]
internal sealed class Updater
{
    private static Uri[] OnlineApplicationDatas { get; } = new Uri[]
    {
        new Uri("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/ApplicationData.json")
    };

    public async ValueTask<UpdateInfo> GetUpdateInfoAsync()
    {
        var httpClient = new HttpClient();

        try
        {
            var applicationData = await DownloadApplicationAsync(httpClient);

            var needUpdate = Version.Parse(applicationData.AppVersion) != AppxContext.AppxVersion;
            var isSupported = Version.Parse(applicationData.SupportedMinVersion) <= AppxContext.AppxVersion;

            return new UpdateInfo(needUpdate, isSupported, applicationData.MainApp);
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    private async Task<ApplicationData> DownloadApplicationAsync(HttpClient httpClient)
    {
        foreach (var uri in OnlineApplicationDatas)
        {
            using var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonObject.Parse(result);

            return new ApplicationData(
                jsonObject[nameof(ApplicationData.MainApp)].GetString(),
                jsonObject[nameof(ApplicationData.AppVersion)].GetString(),
                jsonObject[nameof(ApplicationData.SupportedMinVersion)].GetString());
        }

        throw new IndexOutOfRangeException();
    }


    private readonly struct ApplicationData
    {
        public static readonly ApplicationData Empty = new();

        public readonly string MainApp;

        public readonly string AppVersion;

        public readonly string SupportedMinVersion;

        public readonly bool IsEmpty;

        public ApplicationData()
        {
            MainApp = string.Empty;
            AppVersion = string.Empty;
            SupportedMinVersion = string.Empty;
            IsEmpty = true;
        }

        public ApplicationData(string mainApp, string appVersion, string supportedMinVersion)
        {
            MainApp = mainApp;
            AppVersion = appVersion;
            SupportedMinVersion = supportedMinVersion;
            IsEmpty = false;
        }
    }
}
