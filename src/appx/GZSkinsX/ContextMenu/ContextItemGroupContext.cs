// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace GZSkinsX.ContextMenu;

/// <summary>
/// 用于存储子菜单项的组的上下文信息
/// </summary>
internal sealed class ContextItemGroupContext(string name, double order)
{
    /// <summary>
    /// 获取该组的名称
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 获取该组的排序顺序
    /// </summary>
    public double Order { get; } = order;

    /// <summary>
    /// 获取该组中的子菜单项
    /// </summary>
    public List<ContextMenuItemContext> Items { get; } = new List<ContextMenuItemContext>();
}
