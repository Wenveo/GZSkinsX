// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Navigation;

namespace GZSkinsX.Appx.Navigation;

[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Guid = NavigationConstants.NAV_MAIN_GROUP, Order = NavigationConstants.ORDER_MAIN_GROUP)]
internal sealed class NavMainGroup : INavigationGroup { }

[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Header = "resx:GZSkinsX.Appx.Navigation/Resources/NavigationGroup_DevTools_Header", Guid = NavigationConstants.NAV_DEV_TOOLS_GROUP, Order = NavigationConstants.ORDER_DEV_TOOLS_GROUP)]
internal sealed class NavDevToolsGroup : INavigationGroup { }

[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Guid = NavigationConstants.NAV_FOOTER_GROUP, Order = NavigationConstants.ORDER_FOOTER_GROUP, Placement = NavigationItemPlacement.Footer)]
internal sealed class NavFooterGroup : INavigationGroup { }
