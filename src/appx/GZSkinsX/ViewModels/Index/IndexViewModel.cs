// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;

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
    /// 开始进行下载模块的操作
    /// </summary>
    public async Task TryDownloadAsync()
    {
        IsDownloading = true;

        try
        {
            var mainWindowDispatcherQueue = AppxContext.AppxWindow.MainWindow.DispatcherQueue;
            await AppxContext.KernelService.UpdateModuleAsync(new Progress<double>(async (d) =>
            {
                await mainWindowDispatcherQueue.EnqueueAsync(() => ProgressValue = d * 100);
            }));

            await Task.Delay(200);

            IsDownloading = false;
            IsPendingRestart = true;
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.App.ViewModels.IndexViewModel.DownloadAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

            IsDownloading = false;
            HasError = true;
        }
    }
}
