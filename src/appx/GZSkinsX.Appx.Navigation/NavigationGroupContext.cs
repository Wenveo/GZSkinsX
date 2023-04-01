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
internal sealed class NavigationGroupContext
{
    /// <summary>
    /// 
    /// </summary>
    private readonly Lazy<INavigationGroup, NavigationGroupMetadataAttribute> _lazy;

    /// <summary>
    /// 
    /// </summary>
    public INavigationGroup Value => _lazy.Value;

    /// <summary>
    /// 
    /// </summary>
    public NavigationGroupMetadataAttribute Metadata => _lazy.Metadata;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lazy"></param>
    public NavigationGroupContext(Lazy<INavigationGroup, NavigationGroupMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
