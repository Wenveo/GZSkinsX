// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Mounter;

using Windows.System;
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

    public LaunchButton()
    {
        InitializeComponent();

        // Initialize
        DispatcherQueue.GetForCurrentThread().EnqueueAsync(async () =>
        {
            var b = await AppxContext.MounterService.VerifyContentIntegrityAsync();
            if (b is false)
            {
                await OnUpdateAsync();
            }
            else
            {
                await UpdateLaunchStateAsync();
                await UpdateMoreLaunchOptionsAsync();
            }

            MultiStateLaunchButton.IsEnabled = true;

            AppxContext.MounterService.IsRunningChanged -= OnIsRunningChanged;
            AppxContext.MounterService.IsRunningChanged += OnIsRunningChanged;
        }).FireAndForget();
    }

    private async void OnIsRunningChanged(IMounterService sender, bool args)
    {
        await AppxContext.AppxWindow.MainWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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
        await UpdateLaunchStateAsync(CheckForUpdatesState);

        bool needUpdate;
        try
        {
            needUpdate = await AppxContext.MounterService.CheckForUpdatesAsync();
        }
        catch
        {
            await UpdateLaunchStateAsync();
            await ShowFailedToCheckUpdatesTeachingTipAsync();
            return;
        }

        if (needUpdate is false && await AppxContext.MounterService.VerifyContentIntegrityAsync())
        {
            await UpdateLaunchStateAsync();
            await ShowUpToDateTeachingTipAsync();
        }
        else
        {
            if (await AppxContext.MounterService.GetIsRunningAsync())
            {
                await ShowServerIsRunningTeachingTipAsync();
            }
            else
            {
                await OnUpdateAsync();
            }
        }
    }

    public async Task OnTerminateAndUpdateAsync()
    {
        try
        {
            await AppxContext.MounterService.TerminateAsync();
        }
        catch
        {
            return;
        }

        await OnUpdateAsync();
    }

    public async Task OnUpdateAsync()
    {
        HideAllTeachingTips();

        LaunchButton_State_Updating_ProgressRing.Value = 0;
        AppxContext.MounterService.IsRunningChanged -= OnIsRunningChanged;

        await UpdateLaunchStateAsync(UpdatingState);

        try
        {
            CancellationTokenSource? tokenSource = null;
            await AppxContext.MounterService.UpdateAsync(new Progress<double>(async (p) =>
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
            await UpdateLaunchStateAsync(UpdateFailedState);
        }

        AppxContext.MounterService.IsRunningChanged += OnIsRunningChanged;
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

    private void MultiStateToggleButtonFlyout_Opening(object sender, object e)
    {
        static void SyncThemeCore(IEnumerable<MenuFlyoutItemBase> items, ElementTheme requestedTheme)
        {
            foreach (var item in items)
            {
                if (item is MenuFlyoutSubItem subItem)
                {
                    SyncThemeCore(subItem.Items, requestedTheme);
                }

                item.RequestedTheme = requestedTheme;
            }
        }

        if (sender is MenuFlyout flyout)
        {
            // Fix the theme of sub items
            SyncThemeCore(flyout.Items, AppxContext.ThemeService.ActualTheme);
        }
    }
}
