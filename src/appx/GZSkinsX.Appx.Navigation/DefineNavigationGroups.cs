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
[NavigationItemMetadata(Guid = "8373CCBE-3DF6-48DB-81BE-760DF2759189", Header = "TestPageA",
    OwnerGuid = NavigationConstants.NAV_MAIN_GROUP, Order = 0d, PageType = typeof(TestPageA))]
internal sealed class NavTestPageA : INavigationItem
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
[NavigationItemMetadata(Guid = "A4BD5AC5-5F9C-41E9-86C6-047C66F9E7D9", Header = "TestPageb",
    OwnerGuid = NavigationConstants.NAV_DEV_TOOLS_GROUP, Order = 0d, PageType = typeof(TestPageB))]
internal sealed class NavTestPageB : INavigationItem
{
    public IconElement Icon => new SymbolIcon(Symbol.Bookmarks);

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
[NavigationItemMetadata(Guid = "E4BD5AC5-5F9C-41E9-86C6-047C66F9E7D9", Header = "TestPageC",
    OwnerGuid = NavigationConstants.NAV_DEV_TOOLS_GROUP, Order = 0d)]
internal sealed class NavTestPageC : INavigationItem
{
    public IconElement Icon => new SymbolIcon(Symbol.Bookmarks);

    public async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnNavigatedToAsync(NavigationEventArgs args)
    {
        await Task.CompletedTask;
    }
}
