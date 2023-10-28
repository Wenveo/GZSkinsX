// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Contracts.Navigation;

namespace GZSkinsX.Appx.Navigation;

/// <summary>
/// 用于存储导出的 <see cref="INavigationItem"/> 对象以及上下文数据
/// </summary>
internal sealed class NavigationItemMD(Lazy<INavigationItem, NavigationItemContractAttribute> lazy)
{
    /// <summary>
    /// 当前上下文中的懒加载对象
    /// </summary>
    private readonly Lazy<INavigationItem, NavigationItemContractAttribute> _lazy = lazy;

    /// <summary>
    /// 获取当前上下文的 <see cref="INavigationItem"/> 对象
    /// </summary>
    public INavigationItem Value => _lazy.Value;

    /// <summary>
    /// 获取当前上下文的 <see cref="NavigationItemContractAttribute"/> 元数据
    /// </summary>
    public NavigationItemContractAttribute Metadata => _lazy.Metadata;
}
