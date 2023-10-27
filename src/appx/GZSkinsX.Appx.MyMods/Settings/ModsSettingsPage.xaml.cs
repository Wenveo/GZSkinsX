// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Navigation;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MyMods.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class ModsSettingsPage : Page
{
    public ModsSettingsPage()
    {
        InitializeComponent();
        InitializeUIObject();
    }

    private async void InitializeUIObject()
    {
        BloodToogleSwitch.IsOn = await AppxContext.MyModsService.GetIsEnableBloodAsync();
        ModFolderTextBlock.Text = await AppxContext.MyModsService.GetModFolderAsync();
        WadFolderTextBlock.Text = await AppxContext.MyModsService.GetWadFolderAsync();

        BloodToogleSwitch.Toggled += OnBloodToogleSwitchToggled;
    }

    private async void OnBloodToogleSwitchToggled(object sender, RoutedEventArgs e)
    {
        await AppxContext.MyModsService.SetIsEnableBloodAsync(BloodToogleSwitch.IsOn);
    }

    private async void OnBrowserModFolder(object sender, RoutedEventArgs e)
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            folderPicker, AppxContext.AppxWindow.MainWindowHandle);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            await AppxContext.MyModsService.SetModFolderAsync(folder.Path);

            ModFolderTextBlock.Text = folder.Path;
        }
    }

    private async void OnBrowserWadFolder(object sender, RoutedEventArgs e)
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            folderPicker, AppxContext.AppxWindow.MainWindowHandle);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            await AppxContext.MyModsService.SetWadFolderAsync(folder.Path);

            WadFolderTextBlock.Text = folder.Path;
        }
    }

    [Shared, NavigationItemContract(
        OwnerGuid = NavigationConstants.SETTINGS_NAV_GUID,
        Group = NavigationConstants.GROUP_SETTINGS_NAV_DEFAULT,
        Guid = "B587DDD7-39CF-4BA0-B03E-7DE82F777A7F",
        Header = "resx:GZSkinsX.Appx.MyMods/Resources/Settings_Mods_Title",
        PageType = typeof(ModsSettingsPage), Order = 10)]
    internal sealed class ModsSettingsItem : INavigationItem
    {
        public IconElement Icon => new SegoeFluentIcon { Glyph = "\uE74C" };

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        public Task OnNavigatedToAsync(NavigationEventArgs args) => Task.CompletedTask;
    }
}
