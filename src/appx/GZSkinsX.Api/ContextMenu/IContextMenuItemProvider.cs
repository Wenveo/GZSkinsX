// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace GZSkinsX.SDK.ContextMenu;

/// <summary>
/// 提供用于实现菜单项的自定义子级上下文菜单的菜单项集合
/// </summary>
public interface IContextMenuItemProvider
{
    /// <summary>
    /// 创建用作于子级的上下文菜单的子菜单项集合
    /// </summary>
    /// <returns>返回已创建的子菜单项集合</returns>
    IEnumerable<CreatedContextMenuItem> CreateSubItems();
}
