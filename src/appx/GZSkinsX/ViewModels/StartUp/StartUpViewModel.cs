// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Game;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace GZSkinsX.ViewModels;

internal sealed partial class StartUpViewModel : ObservableObject
{
    public bool IsRiotSupported { get; }
#if RIOT_SUPPORTED
    = true;
#endif

    [ObservableProperty]
    private StorageFolder? _selectedFolder;

    [ObservableProperty]
    private int _regionsSelectedIndex = -1;

    [ObservableProperty]
    private ObservableCollection<string> _regions;

    [ObservableProperty]
    private string? _infoBarTitle;

    [ObservableProperty]
    private bool _infoBarIsOpen;

    private readonly IWindowManagerService _windowManagerService;
    private readonly IGameService _gameService;

    public StartUpViewModel()
    {
        _gameService = AppxContext.GameService;
        _windowManagerService = AppxContext.WindowManagerService;

        _regions = new ObservableCollection<string>
        {
#if RIOT_SUPPORTED
            ResourceHelper.GetLocalized("Resources/StartUp_Initialize_Region_Riot"),
#endif
            ResourceHelper.GetLocalized("Resources/StartUp_Initialize_Region_Tencent")
        };
    }

    [RelayCommand]
    private async Task OnBrowser()
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            folderPicker, AppxContext.AppxWindow.MainWindowHandle);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            SelectedFolder = folder;
        }
    }

    [RelayCommand]
    private async Task OnDone()
    {
        if (SelectedFolder is null || string.IsNullOrEmpty(SelectedFolder.Path))
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("Resources/StartUp_Error_Directory_Null"));
            return;
        }

        if (RegionsSelectedIndex is -1)
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("Resources/StartUp_Error_Region_Null"));
            return;
        }

        var selectedRegion = (GameRegion)(RegionsSelectedIndex + (IsRiotSupported ? 1 : 2));
        if (await _gameService.TryUpdateAsync(SelectedFolder, selectedRegion) is false)
        {
            ShowErrorMessage(ResourceHelper.GetLocalized("Resources/StartUp_Error_Directory_Invalid"));
            return;
        }

        _windowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);
    }

    private void ShowErrorMessage(string msg)
    {
        InfoBarTitle = msg;
        InfoBarIsOpen = true;
    }
}
