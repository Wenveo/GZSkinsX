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
[NavigationGroupMetadata(Guid = NavigationConstants.NAV_DEV_TOOLS_GROUP, Order = NavigationConstants.ORDER_DEV_TOOLS_GROUP)]
internal sealed class NavDevToolsGroup : INavigationGroup { }

[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Guid = NavigationConstants.NAV_FOOTER_GROUP, Order = NavigationConstants.ORDER_FOOTER_GROUP)]
internal sealed class NavFooterGroup : INavigationGroup { }

[Shared, ExportNavigationItem]
[NavigationItemMetadata(Guid = "7C9D47B5-879B-453F-9DBB-8EFF2B1FF96A", Header = "Home", OwnerGuid = NavigationConstants.NAV_MAIN_GROUP, PageType = typeof(TestPageA))]
internal sealed class TestItemA : INavigationItem
{
    internal sealed class TestPageA : Page { }

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
[NavigationItemMetadata(Guid = "CCBF07BC-F727-4686-B6E7-4F70C39AC48F", Header = "Mods", OwnerGuid = NavigationConstants.NAV_MAIN_GROUP, PageType = typeof(TestPageB))]
internal sealed class TestItemB : INavigationItem
{
    internal sealed class TestPageB : Page { }

    public IconElement Icon => new FontIcon { FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"), Glyph = "\uE74C" };

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
[NavigationItemMetadata(Guid = "FDE0A366-0C47-4623-8C23-027A2F45AB4E", Header = "Creator Studio", OwnerGuid = NavigationConstants.NAV_DEV_TOOLS_GROUP, PageType = typeof(TestPageC))]
internal sealed class TestItemC : INavigationItem
{
    internal sealed class TestPageC : Page { }

    public IconElement Icon => new FontIcon { FontFamily = new Windows.UI.Xaml.Media.FontFamily("Segoe Fluent Icons"), Glyph = "\uEB3C" };

    public async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnNavigatedToAsync(NavigationEventArgs args)
    {
        await Task.CompletedTask;
    }
}
