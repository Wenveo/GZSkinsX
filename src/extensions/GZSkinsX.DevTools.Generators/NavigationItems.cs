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
using GZSkinsX.DevTools.Generators.Assets.Icons;
using GZSkinsX.DevTools.Generators.Views;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GZSkinsX.DevTools.Generators;

[Shared, NavigationItemContract(OwnerGuid = NavigationConstants.MAIN_NAV_GUID,
    Group = NavigationConstants.GROUP_MAIN_NAV_DEVTOOLS, Guid = THE_GUID,
    Header = "resx:GZSkinsX.DevTools.Generators.x/Resources/Generators_Title")]
sealed file class GeneratorsItem : INavigationItem
{
    public const string THE_GUID = "C32A5C93-AE89-41F4-A526-13DDF00EF08D";

    public IconElement? Icon => new SegoeFluentIcon("\uE945");

    public Task OnNavigatedFromAsync() => Task.CompletedTask;

    public Task OnNavigatedToAsync(NavigationEventArgs args) => Task.CompletedTask;
}

[Shared, NavigationItemContract(OwnerGuid = GeneratorsItem.THE_GUID, Order = 0,
    Guid = "C0AC7BB3-779A-4278-8A10-E8B6881697AE", PageType = typeof(HashGeneratorPage),
    Header = "resx:GZSkinsX.DevTools.Generators.x/Resources/HashGenerator_Title")]
sealed file class HashGeneratorItem : INavigationItem
{
    public IconElement? Icon => new SegoeFluentIcon("\uE928");

    public Task OnNavigatedFromAsync() => Task.CompletedTask;

    public Task OnNavigatedToAsync(NavigationEventArgs args) => Task.CompletedTask;
}

[Shared, NavigationItemContract(OwnerGuid = GeneratorsItem.THE_GUID, Order = 1,
    Guid = "B99113DB-37BB-4928-8CA5-EBA2376026B0", PageType = typeof(ChecksumGeneratorPage),
    Header = "resx:GZSkinsX.DevTools.Generators.x/Resources/ChecksumGenerator_Title")]
sealed file class ChecksumGeneratorItem : INavigationItem
{
    public IconElement? Icon => new ChecksumGeneratorIcon();

    public Task OnNavigatedFromAsync() => Task.CompletedTask;

    public Task OnNavigatedToAsync(NavigationEventArgs args) => Task.CompletedTask;
}
