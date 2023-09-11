// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;

using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace GZSkinsX.ViewModels;

/// <inheritdoc/>
internal sealed partial class IndexViewModel : ObservableObject
{
    /// <summary>
    /// 获取和设置下载进度的值。
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InProgress))]
    private double _progressValue;

    /// <summary>
    /// 表示当前是否出现了错误。
    /// </summary>
    [ObservableProperty]
    private bool _hasError;

    /// <summary>
    /// 表示当前是否需要等待应用重新启动。
    /// </summary>
    [ObservableProperty]
    private bool _isPendingRestart;

    /// <summary>
    /// 表示当前是否已处于下载操作中。
    /// </summary>
    [ObservableProperty]
    private bool _isDownloading;

    /// <summary>
    /// 表示当前是否正在进行下载中。
    /// </summary>
    public bool InProgress => ProgressValue != 0;

    /// <summary>
    /// 表示内核模块的下载链接列表。
    /// </summary>
    private static Uri[] KernelModules { get; } = new Uri[]
    {
        new("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/Kernel/3.0.0.0/GZSkinsX.Kernel.dll"),
        new("http://x1.gzskins.com/MounterV3/Kernel/3.0.0.0/GZSkinsX.Kernel.dll")
    };

    /// <summary>
    /// 开始进行下载模块的操作。
    /// </summary>
    public void StartDownload()
    {
        var bgDownloadWorker = new BackgroundWorker();
        bgDownloadWorker.DoWork += OnDoWork;
        bgDownloadWorker.RunWorkerAsync(DispatcherQueue.GetForCurrentThread());
    }

    private async void OnDoWork(object? sender, DoWorkEventArgs e)
    {
        var dispatcherQueue = (e.Argument as DispatcherQueue)!;
        dispatcherQueue.TryEnqueue(() => IsDownloading = true);

        var destFile = await (await ApplicationData.Current.RoamingFolder
            .CreateFolderAsync("Kernel", CreationCollisionOption.OpenIfExists))
            .CreateFileAsync("GZSkinsX.Kernel.dll", CreationCollisionOption.OpenIfExists);

        using var outputStream = await destFile.OpenStreamForWriteAsync();
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
        });

        foreach (var uri in KernelModules)
        {
            try
            {
                await httpClient.DownloadAsync(uri, outputStream, new Progress<double>((progress)
                    => dispatcherQueue.TryEnqueue(() => ProgressValue = progress * 100)));

                await Task.Delay(200);

                //// Should complete in there
                var result = AppInstance.Restart(string.Empty);
                if (result is AppRestartFailureReason.NotInForeground or AppRestartFailureReason.Other)
                {
                    dispatcherQueue.TryEnqueue(() =>
                    {
                        IsDownloading = false;
                        IsPendingRestart = true;
                    });
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.App.ViewModels.IndexViewModel.DownloadAsync",
                    $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

                outputStream.Seek(0, SeekOrigin.Begin);
                continue;
            }
        }

        dispatcherQueue.TryEnqueue(() =>
        {
            if (IsPendingRestart is false)
            {
                IsDownloading = false;
                HasError = true;
            }
        });
    }
}
