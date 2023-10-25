// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Command;
using GZSkinsX.Contracts.ContextMenu;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;
using GZSkinsX.Contracts.Navigation;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MyMods.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class ModsView : Page, INavigationViewSearchHolder
{
    public MyModsView MyModsView { get; }

    private string CollectionCount_FormatString { get; }

    public ModsView()
    {
        InitializeComponent();
        MyModsView = new(MyModsGridView);
        CollectionCount_FormatString = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/ModsView_Collection_Count");

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 13))
        {
            ContentGrid.ContextFlyout = AppxContext.ContextMenuService.CreateCommandBarMenu(ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
                new CommandBarMenuOptions { Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft }, (s, e) => new MyModsViewContextMenuUIContext(s, MyModsView));
        }
        else
        {
            ContentGrid.ContextFlyout = AppxContext.ContextMenuService.CreateContextMenu(ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
                new ContextMenuOptions { Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft }, (s, e) => new MyModsViewContextMenuUIContext(s, MyModsView));
        }

        var commandBar = AppxContext.CommandBarService.CreateCommandBar(CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, new MyModsViewCommandBarUIContext(MyModsView));
        commandBar.Resources.Add("CommandBarBackgroundOpen", new SolidColorBrush(Colors.Transparent));
        commandBar.Resources.Add("CommandBarBorderBrushOpen", new SolidColorBrush(Colors.Transparent));
        commandBar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
        ModsCommandBarPresenter.Content = commandBar;
    }

    private void OnItemsVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
    {
        void OnItemsVectorChangedCore()
        {
            ModsView_Subtitle.Text = string.Format(CollectionCount_FormatString, sender.Count);
        }

        if (DispatcherQueue.HasThreadAccess)
        {
            OnItemsVectorChangedCore();
        }
        else
        {
            DispatcherQueue.TryEnqueue(OnItemsVectorChangedCore);
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        OnItemsVectorChanged(MyModsGridView.Items, null!);
        MyModsGridView.Items.VectorChanged += OnItemsVectorChanged;

        MyModsView.RefreshStarting += OnModsViewRefreshStarting;
        MyModsView.RefreshCompleted += OnModsViewRefreshCompleted;
        DispatcherQueue.EnqueueAsync(MyModsView.RefreshAsync).FireAndForget();
    }

    private void OnModsViewRefreshStarting(object? sender, EventArgs e)
    {
        void OnModsViewRefreshStartingCore()
        {
            MyModsGridView.IsEnabled = false;
            ModsCommandBarPresenter.IsEnabled = false;
            LoadingProgressRing.Visibility = Visibility.Visible;
        }

        if (DispatcherQueue.HasThreadAccess)
        {
            OnModsViewRefreshStartingCore();
        }
        else
        {
            DispatcherQueue.TryEnqueue(OnModsViewRefreshStartingCore);
        }
    }

    private void OnModsViewRefreshCompleted(object? sender, EventArgs e)
    {
        void OnModsViewRefreshCompletedCore()
        {
            LoadingProgressRing.Visibility = Visibility.Collapsed;
            ModsCommandBarPresenter.IsEnabled = true;
            MyModsGridView.IsEnabled = true;
        }

        if (DispatcherQueue.HasThreadAccess)
        {
            OnModsViewRefreshCompletedCore();
        }
        else
        {
            DispatcherQueue.TryEnqueue(OnModsViewRefreshCompletedCore);
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        MyModsGridView.Items.VectorChanged -= OnItemsVectorChanged;
        MyModsView.RefreshStarting -= OnModsViewRefreshStarting;
        MyModsView.RefreshCompleted -= OnModsViewRefreshCompleted;
    }

    private void ContentGrid_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key is Windows.System.VirtualKey.V)
        {
            MyModsView.IsShowInstalledIndex = true;
        }
    }

    private void ContentGrid_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (MyModsView.IsShowInstalledIndex)
        {
            MyModsView.IsShowInstalledIndex = false;
        }
    }

    private void ContentGrid_LostFocus(object sender, RoutedEventArgs e)
    {
        if (MyModsView.IsShowInstalledIndex)
        {
            MyModsView.IsShowInstalledIndex = false;
        }
    }

    private void ContentGrid_DragOver(object sender, DragEventArgs e)
    {
        // Is from the application itself ?
        if (e.Data is { Properties.Title: "DragMyModFiles" })
        {
            return;
        }

        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }

    private async void ContentGrid_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            await MyModsView.ImportAsync(items.OfType<StorageFile>());
        }
    }

    private async void MyModsGridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        e.Data.Properties.Title = "DragMyModFiles";
        e.Data.RequestedOperation = DataPackageOperation.Copy;

        var items = new List<StorageFile>();
        foreach (var item in e.Items.OfType<MyModItemViewModel>())
        {
            if (File.Exists(item.ModInfo.FileInfo.FullName))
            {
                items.Add(await StorageFile.GetFileFromPathAsync(item.ModInfo.FileInfo.FullName));
            }
        }

        e.Data.SetStorageItems(items);
    }

    private void MyModsGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        MyModsGridView.SmoothScrollIntoViewWithItemAsync(e.ClickedItem);
    }

    private async void MyModsGridView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement { DataContext: MyModItemViewModel item })
        {
            if (item.IsInstalled)
            {
                await MyModsView.UninstallAsync(item);
            }
            else
            {
                await MyModsView.InstallAsync(item);
            }
        }
    }

    private void MyModsGridView_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement { DataContext: MyModItemViewModel item })
        {
            if (MyModsGridView.SelectedItems.Contains(item))
            {
                return;
            }

            MyModsGridView.SelectedItems.Clear();
            MyModsGridView.SelectedItems.Add(item);
        }
    }

    public string GetPlaceholderText()
    {
        return ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/ModsView_SearchBox_PlaceholderText");
    }

    public void OnSearchTextChanged(INavigationViewManager sender, string? newText)
    {
        MyModsView.ItemsFilter = newText;
    }
}
