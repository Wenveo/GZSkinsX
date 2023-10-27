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

namespace GZSkinsX.Appx.MyMods.Views;

[Shared, NavigationItemContract(
    OwnerGuid = NavigationConstants.MAIN_NAV_GUID,
    Group = NavigationConstants.GROUP_MAIN_NAV_GENERAL,
    Guid = NavigationConstants.MAIN_NAV_MODS_GUID,
    Header = "resx:GZSkinsX.Appx.MyMods/Resources/ModsViewItem_Header",
    PageType = typeof(ModsView), Order = 10)]
internal sealed class ModsViewItem : INavigationItem
{
    public IconElement Icon => new SegoeFluentIcon { Glyph = "\uE74C" };

    public Task OnNavigatedFromAsync() => Task.CompletedTask;

    public Task OnNavigatedToAsync(NavigationEventArgs args) => Task.CompletedTask;
}
