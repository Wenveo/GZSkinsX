// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.SDK.Controls;
using GZSkinsX.SDK.Navigation;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GZSkinsX.Views.NavigationViews.Home;

[Shared, ExportNavigationItem]
[NavigationItemMetadata(OwnerGuid = NavigationConstants.NAVIGATIONROOT_NV_GUID, Group = NavigationConstants.GROUP_NAVIGATIONROOT_NV_MAIN, Order = 0,
    Guid = "CEF94E82-AA3D-4D0B-84BD-3B01671B7165", Header = "resx:Resources/NavigationViewItem_Home_Header", PageType = typeof(HomePage))]
internal sealed class HomeViewItem : INavigationItem
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
