// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace GZSkinsX.ViewModels;

internal sealed partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private StorageFolder? _gameFolder;

    [ObservableProperty]
    private StorageFolder? _modsFolder;

    [ObservableProperty]
    private StorageFolder? _wadsFolder;

    [ObservableProperty]
    private bool _isEnableBlood;

    [ObservableProperty]
    private bool _isEnableEfficiencyMode;

    [ObservableProperty]
    private bool _isEnableAutoExit;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLightTheme))]
    [NotifyPropertyChangedFor(nameof(IsDarkTheme))]
    [NotifyPropertyChangedFor(nameof(IsDefaultTheme))]
    [NotifyPropertyChangedFor(nameof(CurrentThemeDisplayName))]
    private ElementTheme _currentTheme;

    public bool IsLightTheme => CurrentTheme is ElementTheme.Light;

    public bool IsDarkTheme => CurrentTheme is ElementTheme.Dark;

    public bool IsDefaultTheme => CurrentTheme is ElementTheme.Default;

    public string CurrentThemeDisplayName
    {
        get
        {
            var resourceKey = CurrentTheme switch
            {
                ElementTheme.Light => "Resources/Common_Application_Theme_Light",
                ElementTheme.Dark => "Resources/Common_Application_Theme_Dark",
                _ => "Resources/Common_Application_Theme_Default",
            };

            return ResourceHelper.GetLocalized(resourceKey);
        }
    }

    public string VersionString { get; } = AppxContext.AppxVersion.ToString();

    public async Task InitializeAsync()
    {
        IsEnableBlood = await AppxContext.MyModsService.GetIsEnableBloodAsync();
        IsEnableEfficiencyMode = AppxContext.Resolve<AppSettings>().ShouldEnableEfficiencyMode;
        IsEnableAutoExit = !AppxContext.Resolve<AppSettings>().DontNeedCloseWhenGameIsLaunched;

        GameFolder = await AppxContext.GameService.TryGetRootFolderAsync();
        ModsFolder = await AppxContext.MyModsService.GetModsFolderAsync();
        WadsFolder = await AppxContext.MyModsService.GetWadsFolderAsync();

        CurrentTheme = AppxContext.ThemeService.CurrentTheme;
    }

    async partial void OnIsEnableBloodChanged(bool value)
    {
        await AppxContext.MyModsService.SetIsEnableBloodAsync(value);
    }

    partial void OnIsEnableEfficiencyModeChanged(bool value)
    {
        AppxContext.Resolve<AppSettings>().ShouldEnableEfficiencyMode = value;
        EfficiencyManager.SetEfficiencyMode(Environment.ProcessId, value);
    }

    partial void OnIsEnableAutoExitChanged(bool value)
    {
        AppxContext.Resolve<AppSettings>().DontNeedCloseWhenGameIsLaunched = !value;
    }

    [RelayCommand]
    private async Task OnSetCurrentThemeAsync(ElementTheme newTheme)
    {
        var themeService = AppxContext.ThemeService;
        if (themeService.CurrentTheme != newTheme)
        {
            await themeService.SetElementThemeAsync(newTheme);
        }

        CurrentTheme = newTheme;
    }

    [RelayCommand]
    private async Task OnBrowerGameFolderAsync()
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            folderPicker, AppxContext.AppxWindow.MainWindowHandle);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            var b = await AppxContext.GameService.TryUpdateAsync(folder, AppxContext.GameService.CurrentRegion);
            if (b is false)
            {
                var contentDialog = new ContentDialog()
                {
                    XamlRoot = AppxContext.AppxWindow.MainWindow.Content.XamlRoot,
                    CloseButtonText = ResourceHelper.GetLocalized("Resources/Common_Dialog_OK"),
                    Title = ResourceHelper.GetLocalized("Resources/StartUp_Error_Directory_Invalid"),
                    Content = string.Format("\"{0}\"", folder.Path)
                };

                await contentDialog.ShowAsync();
            }
            else
            {
                GameFolder = folder;
            }
        }
    }

    [RelayCommand]
    private async Task OnBrowerModsFolderAsync()
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            folderPicker, AppxContext.AppxWindow.MainWindowHandle);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            await AppxContext.MyModsService.SetModsFolderAsync(folder);
            ModsFolder = folder;
        }

    }

    [RelayCommand]
    private async Task OnBrowerWadsFolderAsync()
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            folderPicker, AppxContext.AppxWindow.MainWindowHandle);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            await AppxContext.MyModsService.SetWadsFolderAsync(folder);
            WadsFolder = folder;
        }
    }
}
