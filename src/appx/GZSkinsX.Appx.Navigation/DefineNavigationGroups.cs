// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api.Navigation;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GZSkinsX.Appx.Navigation;

[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Guid = NavigationConstants.NAV_MAIN_GROUP, Order = NavigationConstants.ORDER_MAIN_GROUP)]
internal sealed class NavMainGroup : INavigationGroup { }


[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Header = "Dev Tools", Guid = NavigationConstants.NAV_DEV_TOOLS_GROUP, Order = NavigationConstants.ORDER_DEV_TOOLS_GROUP)]
internal sealed class NavDevToolsGroup : INavigationGroup { }

[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Guid = NavigationConstants.NAV_FOOTER_GROUP, Order = NavigationConstants.ORDER_FOOTER_GROUP, Placement = NavigationItemPlacement.Footer)]
internal sealed class NavFooterGroup : INavigationGroup { }


[Shared, ExportNavigationItem]
[NavigationItemMetadata(Guid = "804F13B3-CE9B-4640-ACE0-960B6FA97AA3", Header = "TestPageA", OwnerGuid = NavigationConstants.NAV_MAIN_GROUP, PageType = typeof(NavigationRootPage))]
internal sealed class TestPageA : INavigationItem
{
    public IconElement Icon => new SymbolIcon(Symbol.Home);

    public async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnNavigatedToAsync(NavigationEventArgs args)
    {
        await Task.CompletedTask;
    }
}

[Shared, ExportNavigationItem]
[NavigationItemMetadata(Guid = "536A2186-C444-4AA9-ACF4-4E6602E82765", Header = "TestPageB", OwnerGuid = NavigationConstants.NAV_MAIN_GROUP, PageType = typeof(NavigationRootPage))]
internal sealed class TestPageB : INavigationItem
{
    public IconElement Icon => new SymbolIcon(Symbol.SlideShow);

    public async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnNavigatedToAsync(NavigationEventArgs args)
    {
        await Task.CompletedTask;
    }
}
