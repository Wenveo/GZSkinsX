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

namespace GZSkinsX.Extensions.CreatorStudio;

[Shared, ExportNavigationItem]
[NavigationItemMetadata(OwnerGuid = NavigationConstants.NAVIGATIONROOT_NV_GUID, Group = NavigationConstants.GROUP_NAVIGATIONROOT_NV_DEVTOOLS, Order = 0,
    Guid = "F1698DF7-F971-4558-A07B-27B019DDB239", Header = "resx:GZSkinsX.Extensions.CreatorStudio/Resources/NavItem_Header", PageType = typeof(Shell.ShellPage))]
internal sealed class ExportCreatorStudioNavigationItem : INavigationItem
{
    public IconElement Icon => new SegoeFluentIcon { Glyph = "\uEB3C" };

    public async Task OnNavigatedFromAsync()
    {
        await Task.CompletedTask;
    }

    public async Task OnNavigatedToAsync(NavigationEventArgs args)
    {
        await Task.CompletedTask;
    }
}
