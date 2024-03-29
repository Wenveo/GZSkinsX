// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MotClient;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Contracts.Controls;

public sealed partial class LaunchButton : UserControl
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
        await UpdateAboutMenuItemVisibility();

        AppxContext.MotClientService.IsRunningChanged -= OnIsRunningChanged;
        AppxContext.MotClientService.IsRunningChanged += OnIsRunningChanged;
    }

    private void OnIsRunningChanged(IMotClientService sender, bool args)
    {
        DispatcherQueue.TryEnqueue(async () =>
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
                await AppxContext.MotClientService.LaunchAsync();
            }
            else
            {
                await AppxContext.MotClientService.LaunchAsync(args);
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
            await AppxContext.MotClientService.TerminateAsync();
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
        await Task.Run(OnCheckForUpdatesCoreAsync);
    }

    private async Task OnCheckForUpdatesCoreAsync()
    {
        bool needUpdate;
        try
        {
            needUpdate = await AppxContext.MotClientService.CheckForUpdatesAsync();
        }
        catch
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                await UpdateLaunchStateAsync();
                await ShowFailedToCheckUpdatesTeachingTipAsync();
            });
            return;
        }

        if (needUpdate is false && await AppxContext.MotClientService.VerifyContentIntegrityAsync())
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                await UpdateLaunchStateAsync();
                await ShowUpToDateTeachingTipAsync();
            });
        }
        else
        {
            await DispatcherQueue.EnqueueAsync(async () =>
            {
                if (AppxContext.MotClientService.IsMTRunning)
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
        if ((EnsureState(UpdatingState) || EnsureState(RunningState) is false) && EnsureState(CheckForUpdatesState) is false)
        {
            return;
        }

        try
        {
            await AppxContext.MotClientService.TerminateAsync();
            await OnUpdateAsync();
        }
        catch (Exception excp)
        {
            await ShowRunFailedTeachingTipAsync(excp.Message);
            await UpdateLaunchStateAsync();
            return;
        }
    }

    public async Task OnUpdateAsync()
    {
        if (EnsureState(UpdatingState))
        {
            return;
        }

        HideAllTeachingTips();

        LaunchButton_State_Updating_ProgressRing.Value = 0;
        LaunchButton_State_Updating_ProgressRing.IsIndeterminate = true;
        AppxContext.MotClientService.IsRunningChanged -= OnIsRunningChanged;

        await UpdateLaunchStateAsync(UpdatingState);
        await Task.Run(OnUpdateCoreAsync);
    }

    private async Task OnUpdateCoreAsync()
    {
        try
        {
            await AppxContext.MotClientService.UpdateAsync(new Progress<double>(async (p) =>
            {
                await DispatcherQueue.EnqueueAsync(() =>
                {
                    LaunchButton_State_Updating_ProgressRing.Value = p;
                    LaunchButton_State_Updating_ProgressRing.IsIndeterminate = false;
                });
            }));

            await DispatcherQueue.EnqueueAsync(async () =>
            {
                await Task.Delay(800);
                await UpdateLaunchStateAsync();
                await UpdateMoreLaunchOptionsAsync();
                await ShowUpToDateTeachingTipAsync();
                await UpdateAboutMenuItemVisibility();

                UpdateCompleted?.Invoke(this, EventArgs.Empty);
            });
        }
        catch (Exception excp)
        {
            await DispatcherQueue.EnqueueAsync(async () =>
            {
                await ShowUpdateFailedTeachingTipAsync(excp.Message);
                await UpdateLaunchStateAsync(UpdateFailedState);
            });
        }

        AppxContext.MotClientService.IsRunningChanged += OnIsRunningChanged;
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

        var format = ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/LaunchButton_RunFailedTeachingTip_ErrorMessage");
        LaunchButton_RunFailedTeachingTip.Subtitle = string.Format(format, content);
        LaunchButton_RunFailedTeachingTip.IsOpen = true;

        await Task.CompletedTask;
    }

    private async Task ShowUpToDateTeachingTipAsync()
    {
        HideAllTeachingTips();

        var metadata = await AppxContext.MotClientService.TryGetCurrentPackageMetadataAsync(nameof(MTPackageMetadata.Version));
        if (metadata is not null)
        {
            var format = ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/LaunchButton_UpToDateTeachingTip_CurrentVersion");
            LaunchButton_UpToDateTeachingTip.Subtitle = string.Format(format, metadata.Version);
        }
        else
        {
            LaunchButton_UpToDateTeachingTip.Subtitle = string.Empty;
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
        await UpdateLaunchStateAsync(AppxContext.MotClientService.IsMTRunning ? RunningState : DefaultState);
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

        var metadata = await AppxContext.MotClientService.TryGetCurrentPackageMetadataAsync(nameof(MTPackageMetadata.OtherStartupArgs));
        if (metadata is not null && metadata.OtherStartupArgs.Length is not < 1)
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

        await Task.CompletedTask;
    }

    private async Task UpdateAboutMenuItemVisibility()
    {
        var metadata = await AppxContext.MotClientService.TryGetCurrentPackageMetadataAsync(nameof(MTPackageMetadata.Author));
        LaunchButton_About_MenuItem.Visibility = metadata is null ? Visibility.Collapsed : Visibility.Visible;
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

    private async void LaunchButton_About_MenuItem_Click(object sender, RoutedEventArgs e)
    {
        var metadata = await AppxContext.MotClientService.TryGetCurrentPackageMetadataAsync(
            nameof(MTPackageMetadata.Author), nameof(MTPackageMetadata.Version),
            nameof(MTPackageMetadata.Description), nameof(MTPackageMetadata.AboutTheAuthor));

        if (metadata is null)
        {
            return;
        }

        var richTextBlock = new RichTextBlock();
        if (!string.IsNullOrEmpty(metadata.Version) && !string.IsNullOrWhiteSpace(metadata.Version))
        {
            var versionPara = new Paragraph();
            versionPara.Inlines.Add(new Run() { Text = ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/LaunchButton_About_Dialog_Version") });
            versionPara.Inlines.Add(new Run() { Text = metadata.Version });

            richTextBlock.Blocks.Add(versionPara);
        }

        if (!string.IsNullOrEmpty(metadata.Author) && !string.IsNullOrWhiteSpace(metadata.Author))
        {
            var authorPara = new Paragraph();
            authorPara.Inlines.Add(new Run() { Text = ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/LaunchButton_About_Dialog_Author") });

            if (string.IsNullOrEmpty(metadata.AboutTheAuthor) || string.IsNullOrWhiteSpace(metadata.AboutTheAuthor))
            {
                authorPara.Inlines.Add(new Run() { Text = metadata.Author });
            }
            else
            {
                var authorLink = new Hyperlink() { NavigateUri = new Uri(metadata.AboutTheAuthor) };
                authorLink.Inlines.Add(new Run() { Text = metadata.Author });
                authorPara.Inlines.Add(authorLink);
            }

            richTextBlock.Blocks.Add(authorPara);
        }

        if (!string.IsNullOrEmpty(metadata.Description) && !string.IsNullOrWhiteSpace(metadata.Description))
        {
            var descriptionPara = new Paragraph();
            descriptionPara.Inlines.Add(new Run() { Text = ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/LaunchButton_About_Dialog_Description") });
            descriptionPara.Inlines.Add(new Run() { Text = metadata.Description });

            richTextBlock.Blocks.Add(descriptionPara);
        }

        var contentDialog = new ContentDialog()
        {
            XamlRoot = XamlRoot,
            Content = richTextBlock,
            CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/LaunchButton_About_Dialog_CloseButtonText"),
            Title = ResourceHelper.GetLocalized("GZSkinsX.Appx.Contracts/Resources/LaunchButton_About_Dialog_Title"),
        };

        await contentDialog.ShowAsync();
    }

    private async void LaunchButton_UpdateButIsRunningTeachingTip_ActionButtonClick(TeachingTip sender, object args)
    {
        await OnTerminateAndUpdateAsync();
    }

    private async void LaunchButton_UpdateButIsRunningTeachingTip_CloseButtonClick(TeachingTip sender, object args)
    {
        await UpdateLaunchStateAsync();
    }
}
