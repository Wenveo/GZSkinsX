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

namespace GZSkinsX.Appx.Navigation;

[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Guid = NavigationConstants.MAIN_GROUP, Order = NavigationConstants.ORDER_MAIN_GROUP)]
internal sealed class NavMainGroup : INavigationGroup { }

[Shared, ExportNavigationGroup]
[NavigationGroupMetadata(Guid = NavigationConstants.DEV_TOOLS_GROUP, Order = NavigationConstants.ORDER_DEV_TOOLS_GROUP)]
internal sealed class NavDevToolsGroup : INavigationGroup { }
