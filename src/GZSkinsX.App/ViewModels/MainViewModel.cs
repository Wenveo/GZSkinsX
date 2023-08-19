// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Mounter;
using GZSkinsX.Controls;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MTCanLaunch))]
    [NotifyPropertyChangedFor(nameof(MTCanTerminate))]
    [NotifyPropertyChangedFor(nameof(MTCanCheckForUpdate))]
    private LaunchButtonStatus _mounterLaunchState;

    [ObservableProperty]
    private double _updatingProgress;

    [ObservableProperty]
    private bool _isShowRunFailedTeachingTip;

    [ObservableProperty]
    private bool _isShowUpToDateTeachingTip;

    [ObservableProperty]
    private bool _isShowUpdateFailedTeachingTip;

    [ObservableProperty]
    private bool _isShowUpdateButServerIsRunningTeachingTip;

    [ObservableProperty]
    private string? _runFailedTeachingTipContent;

    [ObservableProperty]
    private string? _upToDateTeachingTipContent;

    [ObservableProperty]
    private string? _updateFailedTeachingTipContent;

    [ObservableProperty]
    private bool _hasMoreLaunchOptions;

    internal MenuFlyoutSubItem MoreLaunchOptionsMenuItem { get; }

    public bool MTCanLaunch => MounterLaunchState is LaunchButtonStatus.Default;

    public bool MTCanTerminate => MounterLaunchState is LaunchButtonStatus.Running;

    public bool MTCanCheckForUpdate => MounterLaunchState is not LaunchButtonStatus.CheckUpdates and not LaunchButtonStatus.Updating;

    public IMounterService MounterService { get; }

    public MainViewModel(MenuFlyoutSubItem moreLaunchOptionsMenuItem)
    {
        MoreLaunchOptionsMenuItem = moreLaunchOptionsMenuItem;

        MounterService = AppxContext.MounterService;
        MounterService.IsRunningChanged += OnIsRunningChanged;
    }

    public async Task InitializeAsync()
    {
        var b = await MounterService.VerifyContentIntegrityAsync();
        if (b is false)
        {
            await OnUpdateAsync();
        }

        await UpdateMoreLaunchOptionsAsync();
    }

    private async void OnIsRunningChanged(IMounterService sender, bool args)
    {
        var newState = args ? LaunchButtonStatus.Running : LaunchButtonStatus.Default;
        await AppxContext.AppxWindow.MainWindow.Dispatcher.RunAsync(
            Windows.UI.Core.CoreDispatcherPriority.High, () => MounterLaunchState = newState);
    }

    [RelayCommand]
    private async Task OnToggleStateAsync()
    {
        var state = MounterLaunchState;
        if (state is LaunchButtonStatus.Default)
        {
            await OnLaunchAsync(string.Empty);
        }
        else if (state is LaunchButtonStatus.Running)
        {
            await OnTerminateAsync();
        }
        else if (state is LaunchButtonStatus.UpdateFailed)
        {
            await OnUpdateAsync();
        }
    }

    [RelayCommand]
    private async Task OnLaunchAsync(string args)
    {
        try
        {
            if (string.IsNullOrEmpty(args))
            {
                await MounterService.LaunchAsync();
            }
            else
            {
                await MounterService.LaunchAsync(args);
            }
        }
        catch (Exception excp)
        {
            await ShowRunFailedTeachingTipAsync(excp.Message);
        }
    }

    [RelayCommand]
    private async Task OnTerminateAsync()
    {
        try
        {
            await MounterService.TerminateAsync();
        }
        catch (Exception excp)
        {
            await ShowRunFailedTeachingTipAsync(excp.Message);
        }
    }

    [RelayCommand]
    private async Task OnCheckForUpdatesAsync()
    {
        MounterLaunchState = LaunchButtonStatus.CheckUpdates;

        if (await MounterService.CheckForUpdatesAsync() is false &&
            await MounterService.VerifyContentIntegrityAsync())
        {
            await UpdateLaunchStateAsync();
            await ShowUpToDateTeachingTipAsync();
            return;
        }
        else
        {
            if (await MounterService.GetIsRunningAsync())
            {
                await ShowServerIsRunningTeachingTipAsync();
            }
            else
            {
                await OnUpdateAsync();
            }
        }
    }

    [RelayCommand]
    private async Task OnTerminateAndUpdateAsync()
    {
        try
        {
            await MounterService.TerminateAsync();
        }
        catch
        {
            return;
        }

        await OnUpdateAsync();
    }

    [RelayCommand]
    private async Task OnUpdateAsync()
    {
        HideAllTeachingTips();
        MounterService.IsRunningChanged -= OnIsRunningChanged;

        UpdatingProgress = 0;
        MounterLaunchState = LaunchButtonStatus.Updating;

        try
        {
            CancellationTokenSource? tokenSource = null;
            await MounterService.UpdateAsync(new Progress<double>(async (p) =>
            {
                tokenSource?.Cancel();
                tokenSource = new CancellationTokenSource();

                try
                {
                    await ProgressAnimationAsync(p, tokenSource);
                }
                catch
                {
                }
            }));

            await UpdateLaunchStateAsync();
            await UpdateMoreLaunchOptionsAsync();
            await ShowUpToDateTeachingTipAsync();
        }
        catch (Exception excp)
        {
            await ShowUpdateFailedTeachingTipAsync(excp.Message);
            MounterLaunchState = LaunchButtonStatus.UpdateFailed;
        }

        MounterService.IsRunningChanged += OnIsRunningChanged;
    }

    private void HideAllTeachingTips()
    {
        IsShowRunFailedTeachingTip = false;
        IsShowUpToDateTeachingTip = false;
        IsShowUpdateFailedTeachingTip = false;
        IsShowUpdateButServerIsRunningTeachingTip = false;
    }

    private async Task ShowUpToDateTeachingTipAsync()
    {
        HideAllTeachingTips();

        var metadata = await MounterService.
            TryGetCurrentPackageMetadataAsync(
                nameof(MTPackageMetadata.Version));

        if (metadata.IsEmpty)
        {
            UpToDateTeachingTipContent = string.Empty;
        }
        else
        {
            var format = ResourceHelper.GetLocalized("Resources/Main_UpToDateTeachingTip_CurrentVersion");
            UpToDateTeachingTipContent = string.Format(format, metadata.Version);
        }

        IsShowUpToDateTeachingTip = true;
        await Task.CompletedTask;
    }

    private async Task ShowRunFailedTeachingTipAsync(string content)
    {
        HideAllTeachingTips();

        var format = ResourceHelper.GetLocalized("Resources/Main_RunFailedTeachingTip_ErrorMessage");
        RunFailedTeachingTipContent = string.Format(format, content);
        IsShowRunFailedTeachingTip = true;

        await Task.CompletedTask;
    }

    private async Task ShowUpdateFailedTeachingTipAsync(string content)
    {
        HideAllTeachingTips();

        UpdateFailedTeachingTipContent = content;
        IsShowUpdateFailedTeachingTip = true;

        await Task.CompletedTask;
    }

    private async Task ShowServerIsRunningTeachingTipAsync()
    {
        HideAllTeachingTips();

        IsShowUpdateButServerIsRunningTeachingTip = true;

        await Task.CompletedTask;
    }

    private async Task UpdateLaunchStateAsync()
    {
        MounterLaunchState = await MounterService.GetIsRunningAsync()
            ? LaunchButtonStatus.Running : LaunchButtonStatus.Default;
    }

    private async Task UpdateMoreLaunchOptionsAsync()
    {
        MenuFlyoutItem CraeteMenuFlyoutItem(string displayName, string args)
        {
            var menuFlyoutItem = new MenuFlyoutItem
            {
                Tag = args,
                Text = displayName,
                Icon = new SegoeFluentIcon { Glyph = "\uE915" }
            };

            menuFlyoutItem.Click += async (s, e) =>
            {
                var self = s as MenuFlyoutItem;
                if (self is not null && self.Tag is string { } str)
                {
                    await OnLaunchAsync(str);
                }
            };

            return menuFlyoutItem;
        }

        var metadata = await MounterService.
            TryGetCurrentPackageMetadataAsync(
                nameof(MTPackageMetadata.OtherStartupArgs));

        if (metadata.OtherStartupArgs.Length is not < 1)
        {
            for (var i = 0; i < metadata.OtherStartupArgs.Length; i++)
            {
                var item = metadata.OtherStartupArgs[i];
                MoreLaunchOptionsMenuItem.Items.Add(CraeteMenuFlyoutItem(item.Name, item.Value));
            }

            HasMoreLaunchOptions = true;
        }
        else
        {
            HasMoreLaunchOptions = false;
            MoreLaunchOptionsMenuItem.Items.Clear();
        }
    }

    private async Task ProgressAnimationAsync(double value, CancellationTokenSource tokenSource)
    {
        const int AnimationCount = 8;
        const int AnimationDelay = 320;
        const int CompletionDelay = 80;

        if (UpdatingProgress < value)
        {
            var maxValue = Math.Min(value - UpdatingProgress, 100);
            var delay = AnimationDelay / AnimationCount;
            var middle = maxValue / AnimationCount;

            for (var i = AnimationCount; i > 1; i--)
            {
                await Task.Delay(delay, tokenSource.Token);
                UpdatingProgress = Math.Round(UpdatingProgress + middle, 2);
            }

            await Task.Delay(delay, tokenSource.Token);
            UpdatingProgress = Math.Round(value, 2);
        }
        else
        {
            var maxValue = Math.Max(UpdatingProgress - value, 0);
            var delay = AnimationDelay / AnimationCount;
            var middle = maxValue / AnimationCount;

            for (var i = AnimationCount; i > 1; i--)
            {
                await Task.Delay(delay, tokenSource.Token);
                UpdatingProgress = Math.Round(UpdatingProgress - middle);
            }

            await Task.Delay(delay, tokenSource.Token);
            UpdatingProgress = Math.Round(value, 2);
        }

        await Task.Delay(CompletionDelay, tokenSource.Token);
    }
}
