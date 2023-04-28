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
namespace GZSkinsX.Appx.Mods;

[Shared, ExportNavigationItem]
[NavigationItemMetadata(Guid = NavigationConstants.MAIN_MODS_GUID, Header = "Mods", PageType = typeof(ModsView),
    Order = NavigationConstants.ORDER_MAIN_GROUP_MODS, OwnerGuid = NavigationConstants.MAIN_GROUP)]
internal sealed class ExportModsNavigationItem : INavigationItem
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
