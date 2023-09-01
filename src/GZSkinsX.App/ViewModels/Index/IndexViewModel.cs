// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using GZSkinsX.Contracts.Appx;

using Windows.ApplicationModel.Core;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace GZSkinsX.ViewModels;

internal sealed partial class IndexViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InProcess))]
    private double _processValue;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isPendingRestart;

    [ObservableProperty]
    private bool _isDownloading;

    public bool InProcess => ProcessValue != 0;

    private static Uri[] ModuleUris { get; } = new Uri[]
    {
        new("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/Kernel/3.0.0.0/GZSkinsX.Kernel.dll"),
        new("http://x1.gzskins.com/MounterV3/Kernel/3.0.0.0/GZSkinsX.Kernel.dll")
    };

    public async Task DownloadAsync()
    {
        IsDownloading = true;

        var destFile = await (await ApplicationData.Current.RoamingFolder
            .CreateFolderAsync("Kernel", CreationCollisionOption.OpenIfExists))
            .CreateFileAsync("GZSkinsX.Kernel.dll", CreationCollisionOption.OpenIfExists);

        var downloader = new BackgroundDownloader();
        foreach (var uri in ModuleUris)
        {
            try
            {
                var operation = downloader.CreateDownload(uri, destFile);
                await operation.StartAsync().AsTask(new Progress<DownloadOperation>(UpdateDownloadProgress));

                await Task.Delay(200);

                // Should complete in there
                var result = await CoreApplication.RequestRestartAsync(string.Empty);
                if (result is AppRestartFailureReason.NotInForeground or AppRestartFailureReason.Other)
                {
                    IsPendingRestart = true;
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogWarning($"IndexViewModel::DownloadAsync -> {excp.Message}");
                continue;
            }
        }

        if (IsPendingRestart is false)
        {
            HasError = true;
        }
    }

    private void UpdateDownloadProgress(DownloadOperation download)
    {
        ProcessValue = download.Progress.BytesReceived / download.Progress.TotalBytesToReceive * 100;
    }
}
