// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Navigation;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MainApp.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class LicensesViewPage : Page
{
    public LicensesViewPage()
    {
        InitializeComponent();
    }

    [Shared, NavigationItemContract(
        OwnerGuid = NavigationConstants.SETTINGS_NAV_GUID,
        Guid = NavigationConstants.SETTINGS_NAV_LICENSES_GUID,
        Group = NavigationConstants.GROUP_SETTINGS_NAV_DEFAULT,
        Order = NavigationConstants.ORDER_SETTINGS_NAV_LICENSES,
        Header = "resx:GZSkinsX.Appx.MainApp/Resources/Settings_Licenses_Title",
        PageType = typeof(LicensesViewPage))]
    private sealed class LicensesViewItem : INavigationItem
    {
        public IconElement Icon => new SegoeFluentIcon { Glyph = "\uF571" };

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        public Task OnNavigatedToAsync(NavigationEventArgs args) => Task.CompletedTask;
    }

}
