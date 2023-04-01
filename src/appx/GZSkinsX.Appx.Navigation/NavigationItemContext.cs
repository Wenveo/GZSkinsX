// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Api.Navigation;

namespace GZSkinsX.Appx.Navigation;

/// <summary>
/// 
/// </summary>
internal sealed class NavigationItemContext
{
    /// <summary>
    /// 
    /// </summary>
    private readonly Lazy<INavigationItem, NavigationItemMetadataAttribute> _lazy;

    /// <summary>
    /// 
    /// </summary>
    public INavigationItem Value => _lazy.Value;

    /// <summary>
    /// 
    /// </summary>
    public NavigationItemMetadataAttribute Metadata => _lazy.Metadata;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lazy"></param>
    public NavigationItemContext(Lazy<INavigationItem, NavigationItemMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
