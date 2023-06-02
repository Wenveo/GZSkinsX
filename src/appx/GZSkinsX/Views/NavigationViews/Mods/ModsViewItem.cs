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

namespace GZSkinsX.Views.NavigationViews.Mods;

[Shared, ExportNavigationItem]
[NavigationItemMetadata(OwnerGuid = NavigationConstants.NAVIGATIONROOT_NV_GUID, Group = NavigationConstants.GROUP_NAVIGATIONROOT_NV_MAIN, Order = 10,
    Guid = "6ADAA585-3915-4689-A1E3-7418FD3055CD", Header = "resx:Resources/NavigationViewItem_Mods_Header", PageType = typeof(ModsPage))]
internal sealed class ModsViewItem : INavigationItem
{
    public IconElement Icon => new SegoeFluentIcon { Glyph = "\uE74C" };

    public async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnNavigatedToAsync(NavigationEventArgs args)
    {
        await Task.CompletedTask;
    }
}
