// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 用于表示上下文菜单中所关联的 UI 上下文
/// </summary>
public interface IContextMenuUIContext
{
    /// <summary>
    /// 获取当前 UI 上下文中的 UI 对象
    /// </summary>
    object UIObject { get; }

    /// <summary>
    /// 获取当前 UI 上下文中的参数
    /// </summary>
    object Parameter { get; }
}
