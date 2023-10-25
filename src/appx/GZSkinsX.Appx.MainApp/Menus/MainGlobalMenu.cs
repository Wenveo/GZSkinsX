// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.ContextMenu;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.MainApp.Menus;

#region --- Main Global Menu / Group / Themes ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MAIN_GLOBALMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_MAIN_GLOBALMENU_CTX_THEMES_GUID,
    Guid = "1634B07B-2D6E-43DD-A799-C65A3445BFCC", Order = 0)]
sealed file class ThemeLightMenuItem : ContextRadioMenuItemBase<MainGlobalMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE793");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Main_GlobalMenu_Themes_Light");

    public override bool IsChecked(MainGlobalMenuUIContext context)
    {
        return context.CurrentTheme is ElementTheme.Light;
    }

    public override async void OnExecute(MainGlobalMenuUIContext context)
    {
        await AppxContext.ThemeService.SetElementThemeAsync(ElementTheme.Light);
    }
}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MAIN_GLOBALMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_MAIN_GLOBALMENU_CTX_THEMES_GUID,
    Guid = "99388C43-8E3C-44B2-8ED1-214AA6D93C38", Order = 1)]
sealed file class ThemeDarkMenuItem : ContextRadioMenuItemBase<MainGlobalMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE708");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Main_GlobalMenu_Themes_Dark");

    public override bool IsChecked(MainGlobalMenuUIContext context)
    {
        return context.CurrentTheme is ElementTheme.Dark;
    }

    public override async void OnExecute(MainGlobalMenuUIContext context)
    {
        await AppxContext.ThemeService.SetElementThemeAsync(ElementTheme.Dark);
    }
}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MAIN_GLOBALMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_MAIN_GLOBALMENU_CTX_THEMES_GUID,
    Guid = "2E5A89F7-4AA4-4FB3-ADAA-0BF356D93E5C", Order = 2)]
sealed file class ThemeDefaultMenuItem : ContextRadioMenuItemBase<MainGlobalMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE770");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Main_GlobalMenu_Themes_Default");

    public override bool IsChecked(MainGlobalMenuUIContext context)
    {
        return context.CurrentTheme is ElementTheme.Default;
    }

    public override async void OnExecute(MainGlobalMenuUIContext context)
    {
        await AppxContext.ThemeService.SetElementThemeAsync(ElementTheme.Default);
    }
}

#endregion  --- Main Global Menu / Group / Themes ---


#region  --- Main Global Menu / Group / Settings ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MAIN_GLOBALMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_MAIN_GLOBALMENU_CTX_SETTINGS_GUID,
    Guid = "4CF9F342-EE2C-4FE8-8F0F-B50BF0CE537E", Order = 0)]
sealed file class SettingsMenuItem : ContextRadioMenuItemBase
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE713");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Main_GlobalMenu_Settings");

    public override void OnExecute(IContextMenuUIContext context)
    {
        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Settings_Guid);
    }
}

#endregion  --- Main Global Menu / Group / Settings ---
