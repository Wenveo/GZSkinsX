// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Game;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.MainApp.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class StartUpPage : Page
{
    public StartUpPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var isInvalid = e.Parameter is bool b && b;

        // 设置标题
        var localizedTitle = ResourceHelper.GetLocalized(isInvalid
            ? "GZSkinsX.Appx.MainApp/Resources/StartUp_Initialize_Invalid_Title"
            : "GZSkinsX.Appx.MainApp/Resources/StartUp_Initialize_Default_Title");
        StartUp_Initialize_Title.Text = localizedTitle;

        // 添加游戏区域枚举的本地化字符串至选择器列表
#if RIOT_SUPPORTED
        var riot = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/StartUp_Initialize_Region_Riot");
        StartUp_Initialize_Region_Selector.Items.Add(riot);
#endif

        var tencent = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/StartUp_Initialize_Region_Tencent");
        StartUp_Initialize_Region_Selector.Items.Add(tencent);

#if RIOT_SUPPORTED
        StartUp_Initialize_Region_Helper.Visibility = Visibility.Visible;
#endif
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        StartUp_Initialize_Title.Text = string.Empty;
        StartUp_Initialize_Region_Selector.Items.Clear();
    }

    private async void OnBrowse(object sender, RoutedEventArgs e)
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            folderPicker, AppxContext.AppxWindow.MainWindowHandle);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            StartUp_Initialize_Directory_TextBox.Text = folder.Path;
        }
    }

    private void OnDone(object sender, RoutedEventArgs e)
    {
        var directoryPath = StartUp_Initialize_Directory_TextBox.Text;
        if (string.IsNullOrEmpty(directoryPath))
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/StartUp_Error_Directory_Null"));
            return;
        }

        var selector = StartUp_Initialize_Region_Selector;
        if (selector.SelectedIndex is -1)
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/StartUp_Error_Region_Null"));
            return;
        }

#if RIOT_SUPPORTED
        var selectedRegion = (GameRegion)(selector.SelectedIndex + 1);
#else
        var selectedRegion = (GameRegion)(selector.SelectedIndex + 2);
#endif
        if (AppxContext.GameService.TryUpdate(directoryPath, selectedRegion) is false)
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/StartUp_Error_Directory_Invalid"));
            return;
        }

        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);
    }

    private void ShowErrorMessage(string errorMsg)
    {
        StartUp_Initialize_Error_InfoBar.Title = errorMsg;
        StartUp_Initialize_Error_InfoBar.IsOpen = true;
    }
}
