// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace GZSkinsX.Appx.Navigation;

/// <summary>
/// 用于存储导航项的组的上下文信息。
/// </summary>
/// <param name="name">该分组的名称。</param>
/// <param name="order">该分组的排序顺序。</param>
internal sealed class NavigationItemGroupMD(string name, double order)
{
    /// <summary>
    /// 获取该分组的名称。
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 获取该分组的排序顺序。
    /// </summary>
    public double Order { get; } = order;

    /// <summary>
    /// 获取该组中的子导航项。
    /// </summary>
    public List<NavigationItemMD> Items { get; } = new List<NavigationItemMD>();
}
