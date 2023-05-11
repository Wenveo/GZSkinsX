// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Api.Navigation;

namespace GZSkinsX.Navigation;

/// <summary>
/// 用于存储导出的 <see cref="INavigationItem"/> 对象以及上下文数据
/// </summary>
internal sealed class NavigationItemContext
{
    /// <summary>
    /// 当前上下文中的懒加载对象
    /// </summary>
    private readonly Lazy<INavigationItem, NavigationItemMetadataAttribute> _lazy;

    /// <summary>
    /// 获取当前上下文的 <see cref="INavigationItem"/> 对象
    /// </summary>
    public INavigationItem Value => _lazy.Value;

    /// <summary>
    /// 获取当前上下文的 <see cref="NavigationItemMetadataAttribute"/> 元数据
    /// </summary>
    public NavigationItemMetadataAttribute Metadata => _lazy.Metadata;

    /// <summary>
    /// 初始化 <see cref="NavigationItemContext"/> 的新实例
    /// </summary>
    public NavigationItemContext(Lazy<INavigationItem, NavigationItemMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
