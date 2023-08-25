// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Mounter;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Controls;

internal sealed partial class LaunchButton : UserControl
{
#pragma warning disable format
    internal const string DefaultState         = "LaunchButton_State_Default";
    internal const string RunningState         = "LaunchButton_State_Running";
    internal const string CheckForUpdatesState = "LaunchButton_State_CheckForUpdates";
    internal const string UpdateFailedState    = "LaunchButton_State_UpdateFailed";
    internal const string UpdatingState        = "LaunchButton_State_Updating";
#pragma warning restore format

    public object SyncRoot { get; } = new();

    public event EventHandler? UpdateCompleted;

    public LaunchButton()
    {
        InitializeComponent();
    }

    public async Task InitializeAsync()
    {
        await UpdateLaunchStateAsync();
        await UpdateMoreLaunchOptionsAsync();

        AppxContext.MounterService.IsRunningChanged -= OnIsRunningChanged;
        AppxContext.MounterService.IsRunningChanged += OnIsRunningChanged;
    }

    private async void OnIsRunningChanged(IMounterService sender, bool args)
    {
        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        {
            await UpdateLaunchStateAsync(args ? RunningState : DefaultState);
        });
    }

    public async Task OnToggleStateAsync()
    {
        var state = MultiStateLaunchButton.State;
        switch (state)
        {
            case DefaultState:
                await OnLaunchAsync(string.Empty);
                break;

            case RunningState:
                await OnTerminateAsync();
                break;

            case UpdateFailedState:
                await OnUpdateAsync();
                break;

            default:
                break;
        }
    }

    public async Task OnLaunchAsync(string args)
    {
        if (EnsureState(UpdatingState) || EnsureState(DefaultState) is false)
        {
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(args))
            {
                await AppxContext.MounterService.LaunchAsync();
            }
            else
            {
                await AppxContext.MounterService.LaunchAsync(args);
            }
        }
        catch (Exception excp)
        {
            await ShowRunFailedTeachingTipAsync(excp.Message);
        }
    }

    public async Task OnTerminateAsync()
    {
        if (EnsureState(UpdatingState) || EnsureState(RunningState) is false)
        {
            return;
        }

        try
        {
            await AppxContext.MounterService.TerminateAsync();
        }
        catch (Exception excp)
        {
            await ShowRunFailedTeachingTipAsync(excp.Message);
        }
    }

    public async Task OnCheckForUpdatesAsync()
    {
        if (EnsureState(CheckForUpdatesState) || EnsureState(UpdatingState))
        {
            return;
        }

        await UpdateLaunchStateAsync(CheckForUpdatesState);

        // 使用 BackgroundWorker 进行后台操作，
        // 避免在 UI 线程上执行而导致 UI 卡死。
        var bgCheckUpdatesWorker = new BackgroundWorker();
        bgCheckUpdatesWorker.DoWork += BgCheckUpdatesWorker_DoWork;
        bgCheckUpdatesWorker.RunWorkerAsync();
    }

    private async void BgCheckUpdatesWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        bool needUpdate;
        try
        {
            needUpdate = await AppxContext.MounterService.CheckForUpdatesAsync();
        }
        catch
        {
            await Dispatcher.RunAsync(default, async () =>
            {
                await UpdateLaunchStateAsync();
                await ShowFailedToCheckUpdatesTeachingTipAsync();
            });
            return;
        }

        if (needUpdate is false && await AppxContext.MounterService.VerifyContentIntegrityAsync())
        {
            await Dispatcher.RunAsync(default, async () =>
            {
                await UpdateLaunchStateAsync();
                await ShowUpToDateTeachingTipAsync();
            });
        }
        else
        {
            await Dispatcher.RunAsync(default, async () =>
            {
                if (await AppxContext.MounterService.GetIsRunningAsync())
                {
                    await ShowServerIsRunningTeachingTipAsync();
                }
                else
                {
                    await OnUpdateAsync();
                }
            });
        }
    }

    public async Task OnTerminateAndUpdateAsync()
    {
        if (EnsureState(UpdatingState) || EnsureState(RunningState) is false)
        {
            return;
        }

        try
        {
            await AppxContext.MounterService.TerminateAsync();
        }
        catch (Exception excp)
        {
            await ShowRunFailedTeachingTipAsync(excp.Message);
            return;
        }

        await OnUpdateAsync();
    }

    public async Task OnUpdateAsync()
    {
        if (EnsureState(UpdatingState))
        {
            return;
        }

        HideAllTeachingTips();

        LaunchButton_State_Updating_ProgressRing.Value = 0;
        AppxContext.MounterService.IsRunningChanged -= OnIsRunningChanged;

        await UpdateLaunchStateAsync(UpdatingState);

        // 使用 BackgroundWorker 进行后台操作，
        // 避免在 UI 线程上执行而导致 UI 卡死。
        var bgDownloadWorker = new BackgroundWorker();
        bgDownloadWorker.DoWork += BgDownloadWorker_DoWork;
        bgDownloadWorker.RunWorkerAsync();
    }

    private async void BgDownloadWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        try
        {
            CancellationTokenSource? tokenSource = null;
            await AppxContext.MounterService.UpdateAsync(new Progress<double>(async (p) =>
            {
                await Dispatcher.RunAsync(default, async () =>
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
                });
            }));

            await Dispatcher.RunAsync(default, async () =>
            {
                await Task.Delay(800);
                await UpdateLaunchStateAsync();
                await UpdateMoreLaunchOptionsAsync();
                await ShowUpToDateTeachingTipAsync();

                UpdateCompleted?.Invoke(this, EventArgs.Empty);
            });
        }
        catch (Exception excp)
        {
            await Dispatcher.RunAsync(default, async () =>
            {
                await ShowUpdateFailedTeachingTipAsync(excp.Message);
                await UpdateLaunchStateAsync(UpdateFailedState);
            });
        }

        AppxContext.MounterService.IsRunningChanged += OnIsRunningChanged;
    }

    private bool EnsureState(string state)
    {
        return StringComparer.Ordinal.Equals(MultiStateLaunchButton.State, state);
    }

    private void HideAllTeachingTips()
    {
        LaunchButton_FailedToCheckUpdatesTeachingTip.IsOpen = false;
        LaunchButton_RunFailedTeachingTip.IsOpen = false;
        LaunchButton_UpToDateTeachingTip.IsOpen = false;
        LaunchButton_UpdateFailedTeachingTip.IsOpen = false;
        LaunchButton_UpdateButIsRunningTeachingTip.IsOpen = false;
    }

    private async Task ShowFailedToCheckUpdatesTeachingTipAsync()
    {
        HideAllTeachingTips();

        LaunchButton_FailedToCheckUpdatesTeachingTip.IsOpen = true;

        await Task.CompletedTask;
    }

    private async Task ShowRunFailedTeachingTipAsync(string content)
    {
        HideAllTeachingTips();

        var format = ResourceHelper.GetLocalized("Resources/LaunchButton_RunFailedTeachingTip_ErrorMessage");
        LaunchButton_RunFailedTeachingTip.Subtitle = string.Format(format, content);
        LaunchButton_RunFailedTeachingTip.IsOpen = true;

        await Task.CompletedTask;
    }

    private async Task ShowUpToDateTeachingTipAsync()
    {
        HideAllTeachingTips();

        var metadata = await AppxContext.MounterService.TryGetCurrentPackageMetadataAsync(nameof(MTPackageMetadata.Version));
        if (metadata.IsEmpty)
        {
            LaunchButton_UpToDateTeachingTip.Subtitle = string.Empty;
        }
        else
        {
            var format = ResourceHelper.GetLocalized("Resources/LaunchButton_UpToDateTeachingTip_CurrentVersion");
            LaunchButton_UpToDateTeachingTip.Subtitle = string.Format(format, metadata.Version);
        }

        LaunchButton_UpToDateTeachingTip.IsOpen = true;

        await Task.CompletedTask;
    }

    private async Task ShowUpdateFailedTeachingTipAsync(string content)
    {
        HideAllTeachingTips();

        LaunchButton_UpdateFailedTeachingTip.Subtitle = content;
        LaunchButton_UpdateFailedTeachingTip.IsOpen = true;

        await Task.CompletedTask;
    }

    private async Task ShowServerIsRunningTeachingTipAsync()
    {
        HideAllTeachingTips();

        LaunchButton_UpdateButIsRunningTeachingTip.IsOpen = true;

        await Task.CompletedTask;
    }

    private async Task UpdateLaunchStateAsync()
    {
        await UpdateLaunchStateAsync(await AppxContext.MounterService.GetIsRunningAsync() ? RunningState : DefaultState);
    }

    private async Task UpdateLaunchStateAsync(string state)
    {
        lock (SyncRoot)
        {
            if (StringComparer.Ordinal.Equals(state, DefaultState))
            {
                LaunchButton_Launch_MenuItem.IsEnabled = true;
                LaunchButton_MoreLaunchOptions_MenuItem.IsEnabled = true;
            }
            else
            {
                LaunchButton_Launch_MenuItem.IsEnabled = false;
                LaunchButton_MoreLaunchOptions_MenuItem.IsEnabled = false;
            }

            if (StringComparer.Ordinal.Equals(state, RunningState))
            {
                LaunchButton_Terminate_MenuItem.IsEnabled = true;
            }
            else
            {
                LaunchButton_Terminate_MenuItem.IsEnabled = false;
            }

            if (StringComparer.Ordinal.Equals(state, UpdatingState) ||
                StringComparer.Ordinal.Equals(state, CheckForUpdatesState))
            {
                LaunchButton_CheckForUpdats_MenuItem.IsEnabled = false;
            }
            else
            {
                LaunchButton_CheckForUpdats_MenuItem.IsEnabled = true;
            }

            MultiStateLaunchButton.State = state;
        }

        await Task.CompletedTask;
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

        var metadata = await AppxContext.MounterService.TryGetCurrentPackageMetadataAsync(nameof(MTPackageMetadata.OtherStartupArgs));
        if (metadata.OtherStartupArgs.Length is not < 1)
        {
            for (var i = 0; i < metadata.OtherStartupArgs.Length; i++)
            {
                var item = metadata.OtherStartupArgs[i];
                LaunchButton_MoreLaunchOptions_MenuItem.Items.Add(CraeteMenuFlyoutItem(item.Name, item.Value));
            }

            LaunchButton_MoreLaunchOptions_MenuItem.Visibility = Visibility.Visible;
            LaunchButton_HaveMoreLaunchOptions_Separator.Visibility = Visibility.Visible;
            LaunchButton_DontHaveMoreLaunchOptions_Separator.Visibility = Visibility.Collapsed;
        }
        else
        {
            LaunchButton_MoreLaunchOptions_MenuItem.Visibility = Visibility.Collapsed;
            LaunchButton_HaveMoreLaunchOptions_Separator.Visibility = Visibility.Collapsed;
            LaunchButton_DontHaveMoreLaunchOptions_Separator.Visibility = Visibility.Visible;

            LaunchButton_MoreLaunchOptions_MenuItem.Items.Clear();
        }
    }

    private async Task ProgressAnimationAsync(double value, CancellationTokenSource tokenSource)
    {
        const int AnimationCount = 8;
        const int AnimationDelay = 320;
        const int CompletionDelay = 80;

        if (LaunchButton_State_Updating_ProgressRing.Value < value)
        {
            var maxValue = Math.Min(value - LaunchButton_State_Updating_ProgressRing.Value, 100);
            var delay = AnimationDelay / AnimationCount;
            var middle = maxValue / AnimationCount;

            for (var i = AnimationCount; i > 1; i--)
            {
                await Task.Delay(delay, tokenSource.Token);
                LaunchButton_State_Updating_ProgressRing.Value = Math.Round(LaunchButton_State_Updating_ProgressRing.Value + middle, 2);
            }

            await Task.Delay(delay, tokenSource.Token);
            LaunchButton_State_Updating_ProgressRing.Value = Math.Round(value, 2);
        }
        else
        {
            var maxValue = Math.Max(LaunchButton_State_Updating_ProgressRing.Value - value, 0);
            var delay = AnimationDelay / AnimationCount;
            var middle = maxValue / AnimationCount;

            for (var i = AnimationCount; i > 1; i--)
            {
                await Task.Delay(delay, tokenSource.Token);
                LaunchButton_State_Updating_ProgressRing.Value = Math.Round(LaunchButton_State_Updating_ProgressRing.Value - middle);
            }

            await Task.Delay(delay, tokenSource.Token);
            LaunchButton_State_Updating_ProgressRing.Value = Math.Round(value, 2);
        }

        await Task.Delay(CompletionDelay, tokenSource.Token);
    }

    private async void MultiStateLaunchButton_Click(MultiStateToggleButton sender, EventArgs args)
    {
        await OnToggleStateAsync();
    }

    private async void LaunchButton_Launch_MenuItem_Click(object sender, RoutedEventArgs e)
    {
        await OnLaunchAsync(string.Empty);
    }

    private async void LaunchButton_Terminate_MenuItem_Click(object sender, RoutedEventArgs e)
    {
        await OnTerminateAsync();
    }

    private async void LaunchButton_CheckForUpdats_MenuItem_Click(object sender, RoutedEventArgs e)
    {
        await OnCheckForUpdatesAsync();
    }

    private void LaunchButton_About_MenuItem_Click(object sender, RoutedEventArgs e)
    {

    }

    private async void LaunchButton_UpdateButIsRunningTeachingTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
    {
        await OnTerminateAndUpdateAsync();
    }
}
