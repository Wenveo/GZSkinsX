// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Net.Http;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using GZSkinsX.Contracts.Appx;

using Windows.ApplicationModel.Core;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace GZSkinsX.ViewModels;

/// <inheritdoc/>
internal sealed partial class IndexViewModel : ObservableObject
{
    /// <summary>
    /// 获取和设置下载进度的值
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InProgress))]
    private double _progressValue;

    /// <summary>
    /// 表示当前是否出现了错误
    /// </summary>
    [ObservableProperty]
    private bool _hasError;

    /// <summary>
    /// 表示当前是否需要等待应用重新启动
    /// </summary>
    [ObservableProperty]
    private bool _isPendingRestart;

    /// <summary>
    /// 表示当前是否已处于下载操作中
    /// </summary>
    [ObservableProperty]
    private bool _isDownloading;

    /// <summary>
    /// 表示当前是否正在进行下载中
    /// </summary>
    public bool InProgress => ProgressValue != 0;

    /// <summary>
    /// 表示内核模块的下载链接列表
    /// </summary>
    private static Uri[] KernelModules { get; } = new Uri[]
    {
        new("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/Kernel/3.0.0.0/GZSkinsX.Kernel.dll"),
        new("http://x1.gzskins.com/MounterV3/Kernel/3.0.0.0/GZSkinsX.Kernel.dll")
    };

    /// <summary>
    /// 开始进行下载模块的操作
    /// </summary>
    public async Task DownloadAsync()
    {
        IsDownloading = true;

        var destFile = await (await ApplicationData.Current.RoamingFolder
            .CreateFolderAsync("Kernel", CreationCollisionOption.OpenIfExists))
            .CreateFileAsync("GZSkinsX.Kernel.dll", CreationCollisionOption.OpenIfExists);

        var downloader = new BackgroundDownloader();
        foreach (var uri in KernelModules)
        {
            try
            {
                await downloader.CreateDownload(uri, destFile).StartAsync().AsTask(new Progress<DownloadOperation>((download) =>
                {
                    ProgressValue = (double)download.Progress.BytesReceived / download.Progress.TotalBytesToReceive * 100;
                }));

                await Task.Delay(200);

                // Should complete in there
                var result = await CoreApplication.RequestRestartAsync(string.Empty);
                if (result is AppRestartFailureReason.NotInForeground or AppRestartFailureReason.Other)
                {
                    IsDownloading = false;
                    IsPendingRestart = true;
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.App.ViewModels.IndexViewModel.DownloadAsync",
                    $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

                continue;
            }
        }

        if (IsPendingRestart is false)
        {
            HasError = true;
        }
    }
}
