// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Api.ContextMenu;

using Windows.UI.Xaml;

namespace GZSkinsX.Api.Tabs;

public sealed class TabViewManagerOptions
{
    public Style? TabViewStyle { get; set; }

    public Style? TabViewItemStyle { get; set; }

    public ContextMenuOptions? ContextMenuOptions { get; set; }

    public CoerceContextMenuUIContextCallback? ContextMenuUIContextCallback { get; set; }

    public string TabViewManagerGuid { get; set; }

    public TabViewManagerOptions(string tabViewManagerGuid)
    {
        TabViewManagerGuid = tabViewManagerGuid;
    }
}
