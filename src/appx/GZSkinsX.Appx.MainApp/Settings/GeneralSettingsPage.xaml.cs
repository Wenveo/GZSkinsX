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
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Navigation;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MainApp.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class GeneralSettingsPage : Page
{
    private ElementTheme CurrentTheme { get; set; }

    public GeneralSettingsPage()
    {
        InitializeComponent();
        UpdateSelectedTheme(CurrentTheme = AppxContext.ThemeService.CurrentTheme);
        GameFolderTextBlock.Text = AppxContext.GameService.RootDirectory;
    }

    private async void OnBrowserGameFolder(object sender, RoutedEventArgs e)
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            folderPicker, AppxContext.AppxWindow.MainWindowHandle);

        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder is not null)
        {
            if (AppxContext.GameService.TryUpdate(folder.Path, AppxContext.GameService.CurrentRegion))
            {
                GameFolderTextBlock.Text = folder.Path;
            }
            else
            {
                await new ContentDialog()
                {
                    XamlRoot = XamlRoot,
                    DefaultButton = ContentDialogButton.Close,
                    CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/StartUp_Initialize_OKButton/Content"),
                    Title = ResourceHelper.GetLocalized("GZSkinsX.Appx.MainApp/Resources/StartUp_Initialize_Invalid_Title"),
                    Content = folder.Path
                }.ShowAsync();
            }
        }
    }

    private async void OnSwitchTheme(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        if (args.Parameter is ElementTheme newTheme)
        {
            var previousTheme = CurrentTheme;
            if (previousTheme != newTheme)
            {
                CurrentTheme = newTheme;
                UpdateSelectedTheme(newTheme);
                await AppxContext.ThemeService.SetElementThemeAsync(newTheme);
            }
        }
    }

    public void UpdateSelectedTheme(ElementTheme newTheme)
    {
        Settings_Default_ThemeMenu_Light.IsChecked = false;
        Settings_Default_ThemeMenu_Dark.IsChecked = false;
        Settings_Default_ThemeMenu_Default.IsChecked = false;

        switch (newTheme)
        {
            case ElementTheme.Light:
                Settings_Default_ThemeMenu_Light.IsChecked = true;
                break;
            case ElementTheme.Dark:
                Settings_Default_ThemeMenu_Dark.IsChecked = true;
                break;
            default:
                Settings_Default_ThemeMenu_Default.IsChecked = true;
                break;
        }

        SelectThemeButton.Content = GetThemeLocalizedName(newTheme);
    }

    private static string GetThemeLocalizedName(ElementTheme elementTheme) => ResourceHelper.GetLocalized(elementTheme switch
    {
        ElementTheme.Dark => "GZSkinsX.Appx.MainApp/Resources/Main_GlobalMenu_Themes_Dark",
        ElementTheme.Light => "GZSkinsX.Appx.MainApp/Resources/Main_GlobalMenu_Themes_Light",
        _ => "GZSkinsX.Appx.MainApp/Resources/Main_GlobalMenu_Themes_Default",
    });

    [Shared, NavigationItemContract(
        OwnerGuid = NavigationConstants.SETTINGS_NAV_GUID,
        Group = NavigationConstants.GROUP_SETTINGS_NAV_DEFAULT,
        Guid = "A0176E55-C5C4-41F1-9B69-0E6E0C7FB7E9",
        Header = "resx:GZSkinsX.Appx.MainApp/Resources/Settings_General_Title",
        PageType = typeof(GeneralSettingsPage), Order = 0)]
    internal sealed class GeneralSettingsItem : INavigationItem
    {
        public IconElement Icon => new SegoeFluentIcon { Glyph = "\uE9E9" };

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        public Task OnNavigatedToAsync(NavigationEventArgs args) => Task.CompletedTask;
    }
}
