// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using GZSkinsX.Api.AccessCache;
using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Game;
using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.DotNet.Diagnostics;

using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Views.WindowFrames.StartUp;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StartUpPage : Page
{
    private readonly IWindowManagerService _windowManagerService;
    private readonly IFutureAccessService _futureAccessService;
    private readonly IGameService _gameService;

    public StartUpPage()
    {
        _windowManagerService = AppxContext.WindowManagerService;
        _futureAccessService = AppxContext.FutureAccessService;
        _gameService = AppxContext.GameService;

        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var isInvalid = e.Parameter is bool b && b;

        // 设置标题
        var localizedTitle = ResourceHelper.GetLocalized(isInvalid
            ? "Resources/StartUp_Initialize_Invalid_Title"
            : "Resources/StartUp_Initialize_Default_Title");
        StartUp_Initialize_Title.Text = localizedTitle;

        // 添加游戏区域枚举的本地化字符串至选择器列表
        var riot = ResourceHelper.GetLocalized("Resources/StartUp_Initialize_Region_Riot");
        StartUp_Initialize_Region_Selector.Items.Add(riot);

        var tencent = ResourceHelper.GetLocalized("Resources/StartUp_Initialize_Region_Tencent");
        StartUp_Initialize_Region_Selector.Items.Add(tencent);

        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        StartUp_Initialize_Title.Text = string.Empty;
        StartUp_Initialize_Region_Selector.Items.Clear();
        base.OnNavigatedFrom(e);
    }

    private async void OnBrowser(object sender, RoutedEventArgs e)
    {
        var folder = await new FolderPicker().PickSingleFolderAsync();
        if (folder is not null)
        {
            _futureAccessService.Add(folder, FutureAccessItemConstants.Game_Directory_Name);

            StartUp_Initialize_Directory_TextBox.Text = folder.Path;
        }
    }

    private void ShowErrorMessage(string errorMsg)
    {
        StartUp_Initialize_Error_InfoBar.Title = errorMsg;
        StartUp_Initialize_Error_InfoBar.IsOpen = true;
    }

    private void OnOK(object sender, RoutedEventArgs e)
    {
        Debug2.Assert(_gameService is not null);
        Debug2.Assert(_windowManagerService is not null);

        var directoryPath = StartUp_Initialize_Directory_TextBox.Text;
        if (string.IsNullOrEmpty(directoryPath))
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("Resources/StartUp_Error_Directory_Null"));
            return;
        }

        var selector = StartUp_Initialize_Region_Selector;
        if (selector.SelectedIndex is -1)
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("Resources/StartUp_Error_Region_Null"));
            return;
        }

        if (!_gameService.TryUpdate(directoryPath, (GameRegion)(selector.SelectedIndex + 1)))
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("Resources/StartUp_Error_Directory_Invalid"));
            return;
        }

        _windowManagerService.NavigateTo(WindowFrameConstants.NavigationRoot_Guid);
    }
}
