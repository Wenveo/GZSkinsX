// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;

using CommunityToolkit.WinUI;

using GZSkinsX.Activation;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;
using GZSkinsX.Contracts.WindowManager;
using GZSkinsX.ViewModels;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GZSkinsX.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class MainPage : Page
{
    public bool HasLoad { get; private set; }

    public MainViewModel ViewModel { get; } = new();

    public MainPage()
    {
        InitializeComponent();

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 13))
        {
            ContentGrid.ContextFlyout = ContentGrid_ContextFlyout_Win11;
        }
        else
        {
            ContentGrid.ContextFlyout = ContentGrid_ContextFlyout_Win10;
        }

        AppxContext.ActivationService.UnregisterHandler(MainActivationHandler.Instance);
        AppxContext.ActivationService.RegisterHandler(MainActivationHandler.Instance);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        DispatcherQueue.EnqueueAsync(async () =>
        {
            if (HasLoad is false)
            {
                if (AppxContext.MounterService.TryGetMounterWorkingDirectory(out _) is false)
                {
                    MainLaunchButton.UpdateCompleted += MainLaunchButton_UpdateCompleted;
                    UninitializedContent.Visibility = Visibility.Visible;
                    ContentGrid.Visibility = Visibility.Collapsed;

                    await MainLaunchButton.OnUpdateAsync();
                    return;
                }

                ViewModel.EnableWorkspace = false;
                ContentGrid.Visibility = Visibility.Visible;
                UninitializedContent.Visibility = Visibility.Collapsed;

                if (await AppxContext.MounterService.VerifyContentIntegrityAsync() is false)
                {
                    await MainLaunchButton.OnUpdateAsync();
                }
                else
                {
                    await MainLaunchButton.InitializeAsync();
                }

                HasLoad = true;
            }

            if (e.Parameter is IEnumerable<StorageFile> modFiles)
            {
                await ViewModel.ImportAsync(modFiles);
            }
            else
            {
                await ViewModel.OnRefreshAsync();
            }

        }, DispatcherQueuePriority.Normal).FireAndForget();

        AppTitleBar.SetValue(WinUITitleBar.TargetWindowProperty, AppxContext.AppxWindow.MainWindow);
        DataTransferManagerHelper.GetDataTransferManager().DataRequested += DataTransferManager_DataRequested;
    }

    private async void MainLaunchButton_UpdateCompleted(object? sender, EventArgs e)
    {
        MainLaunchButton.UpdateCompleted -= MainLaunchButton_UpdateCompleted;
        UninitializedContent.Visibility = Visibility.Collapsed;
        ContentGrid.Visibility = Visibility.Visible;

        await ViewModel.OnRefreshAsync();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        AppTitleBar.SetValue(WinUITitleBar.TargetWindowProperty, null);
        DataTransferManagerHelper.GetDataTransferManager().DataRequested -= DataTransferManager_DataRequested;
    }

    private void ContentGrid_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is Windows.System.VirtualKey.V)
        {
            ViewModel.IsShowInstalledIndex = true;
        }
    }

    private void ContentGrid_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (ViewModel.IsShowInstalledIndex)
        {
            ViewModel.IsShowInstalledIndex = false;
        }
    }

    private void MyModsGridView_LostFocus(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsShowInstalledIndex)
        {
            ViewModel.IsShowInstalledIndex = false;
        }
    }

    private void MyModsGridView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement { DataContext: MyModViewModel item })
        {
            if (MyModsGridView.SelectedItems.Contains(item))
            {
                return;
            }

            MyModsGridView.SelectedItems.Clear();
            MyModsGridView.SelectedItems.Add(item);
        }
    }

    private void MyModsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MyModsGridView.SelectedItems.Count == 0)
        {
            return;
        }

        var format = ResourceHelper.GetLocalized("Resources/Main_MyMods_SelectedItemsCount");
        Main_MyMods_Commands_DeselectAll2.Label = string.Format(format, MyModsGridView.SelectedItems.Count.ToString());
    }

    private void OnMainSettingsMenuFlyoutOpening(object sender, object e)
    {
        switch (AppxContext.ThemeService.CurrentTheme)
        {
            case ElementTheme.Light:
                Main_SettingsMenu_Theme_Light.IsChecked = true;
                break;
            case ElementTheme.Dark:
                Main_SettingsMenu_Theme_Dark.IsChecked = true;
                break;
            default:
                Main_SettingsMenu_Theme_Default.IsChecked = true;
                break;
        }
    }

    private void Main_SettingsMenu_Settings_Click(object sender, RoutedEventArgs e)
    {
        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Settings_Guid);
    }

    private void CloseMyModsContextMenu_Click(object sender, RoutedEventArgs e)
    {
        ContentGrid.ContextFlyout?.Hide();
    }

    private void OnMyModsContextMenuShare_Click(object sender, RoutedEventArgs e)
    {
        ContentGrid.ContextFlyout?.Hide();

        if (MyModsGridView.SelectedItems.Count > 0)
        {
            var interop = DataTransferManagerHelper.GetDataTransferManagerInterop();
            interop.ShowShareUIForWindow(AppxContext.AppxWindow.MainWindowHandle);
        }
    }

    private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        var deferral = args.Request.GetDeferral();
        await DispatcherQueue.EnqueueAsync(async () =>
        {
            var items = new List<StorageFile>();
            foreach (var item in MyModsGridView.SelectedItems)
            {
                if (item is MyModViewModel modViewModel && File.Exists(modViewModel.FileInfo.FullName))
                {
                    items.Add(await StorageFile.GetFileFromPathAsync(modViewModel.FileInfo.FullName));
                }
            }

            if (items.Count > 0)
            {
                if (items.Count == 1)
                {
                    var format = ResourceHelper.GetLocalized("Resources/Main_Share_Dialog_Title");
                    args.Request.Data.Properties.Title = string.Format(format, items[0].Name);
                }
                else
                {
                    var format = ResourceHelper.GetLocalized("Resources/Main_Share_Dialog_Multiple_Items_Title");
                    args.Request.Data.Properties.Title = string.Format(format, items[0].Name, items[1].Name);
                }

                args.Request.Data.SetStorageItems(items, true);
            }
            else
            {
                args.Request.Data.Properties.Title = ResourceHelper.GetLocalized("Resources/Main_Share_Dialog_Failed_Title");
            }
        });

        deferral.Complete();
    }

    private void Main_Loading_ProgressRing_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is ProgressRing self)
        {
            self.SetValue(ProgressRing.IsIndeterminateProperty, false);
            self.SetValue(ProgressRing.IsIndeterminateProperty, true);
        }
    }
}
