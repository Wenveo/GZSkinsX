// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api.Controls;
using GZSkinsX.Api.Navigation;

using Windows.UI.Xaml.Controls;

using Windows.UI.Xaml.Navigation;

namespace GZSkinsX.Appx.Home;

[Shared, ExportNavigationItem]
[NavigationItemMetadata(Guid = NavigationConstants.MAIN_HOME_GUID, Header = "Home", PageType = typeof(HomeView),
    Order = NavigationConstants.ORDER_MAIN_GROUP_HOME, OwnerGuid = NavigationConstants.MAIN_GROUP)]
internal sealed class ExportHomeNavigationItem : INavigationItem
{
    public IconElement Icon => new SegoeFluentIcon { Glyph = "\uE10F" };

    public async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnNavigatedToAsync(NavigationEventArgs args)
    {
        await Task.CompletedTask;
    }
}
