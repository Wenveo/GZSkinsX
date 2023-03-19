// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using GZSkinsX.Api.AccessCache;
using GZSkinsX.Api.Game;
using GZSkinsX.Api.MRT;
using GZSkinsX.Api.Scripting;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.DotNet.Diagnostics;

using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.StartUp;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StartUpPage : Page
{
    private IMRTCoreMap? _mrtCoreMap;
    private IServiceLocator? _serviceLocator;
    private IGameService? _gameService;
    private IWindowManagerService? _viewManagerService;

    public StartUpPage()
    {
        InitializeComponent();
    }

    internal void InitializeContext(IServiceLocator serviceLocator, bool isInvalid)
    {
        _serviceLocator = serviceLocator;
        _gameService = serviceLocator.Resolve<IGameService>();
        _viewManagerService = serviceLocator.Resolve<IWindowManagerService>();

        var mrtCoreService = serviceLocator.Resolve<IMRTCoreService>();
        _mrtCoreMap = mrtCoreService.MainResourceMap.GetSubtree(MRTCoreConstants.Appx_StartUp);

        // 设置标题
        var resTitle = _mrtCoreMap.GetString(isInvalid
            ? "Appx_StartUp_Initialize_Invalid_Title"
            : "Appx_StartUp_Initialize_Default_Title");
        Appx_StartUp_Initialize_Title.Text = resTitle;

        // 添加游戏区域枚举的本地化字符串至选择器列表
        var riotRes = _mrtCoreMap.GetString("Appx_StartUp_Initialize_Region_Riot");
        Appx_StartUp_Initialize_Region_Selector.Items.Add(riotRes);

        var tencentRes = _mrtCoreMap.GetString("Appx_StartUp_Initialize_Region_Tencent");
        Appx_StartUp_Initialize_Region_Selector.Items.Add(tencentRes);
    }

    private async void OnBrowser(object sender, RoutedEventArgs e)
    {
        var folder = await new FolderPicker().PickSingleFolderAsync();
        if (folder is not null)
        {
            Debug2.Assert(_serviceLocator is not null);
            var futureAccessService = _serviceLocator.Resolve<IFutureAccessService>();
            futureAccessService.Add(folder, FutureAccessItemConstants.Game_Directory_Name);

            Appx_StartUp_Initialize_Directory_TextBox.Text = folder.Path;
        }
    }

    private void ShowErrorMessage(string errorMsg)
    {
        Appx_StartUp_Initialize_Error_InfoBar.Title = errorMsg;
        Appx_StartUp_Initialize_Error_InfoBar.IsOpen = true;
    }

    private void OnOK(object sender, RoutedEventArgs e)
    {
        Debug2.Assert(_gameService is not null);
        Debug2.Assert(_mrtCoreMap is not null);
        Debug2.Assert(_viewManagerService is not null);

        var directoryPath = Appx_StartUp_Initialize_Directory_TextBox.Text;
        if (string.IsNullOrEmpty(directoryPath))
        {
            ShowErrorMessage(_mrtCoreMap.GetString("Appx_StartUp_Error_Directory_Null"));
            return;
        }

        var selector = Appx_StartUp_Initialize_Region_Selector;
        if (selector.SelectedIndex is -1)
        {
            ShowErrorMessage(_mrtCoreMap.GetString("Appx_StartUp_Error_Region_Null"));
            return;
        }

        if (!_gameService.TryUpdate(directoryPath, (GameRegion)(selector.SelectedIndex + 1)))
        {
            ShowErrorMessage(_mrtCoreMap.GetString("Appx_StartUp_Error_Directory_Invalid"));
            return;
        }

        _viewManagerService.NavigateTo(WindowFrameConstants.Main_Guid);
    }
}
