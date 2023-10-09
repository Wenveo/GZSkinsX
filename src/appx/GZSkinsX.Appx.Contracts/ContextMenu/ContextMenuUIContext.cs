// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.ContextMenu;

/// <summary>
/// 表示派生自 <see cref="IContextMenuUIContext"/> 的默认实现。
/// </summary>
/// <param name="uiObject">指定当前 UI 上下文中的 UI 对象。</param>
/// <param name="parameter">指定当前 UI 上下文中的参数。</param>
public class ContextMenuUIContext(object uiObject, object parameter) : IContextMenuUIContext
{
    /// <inheritdoc/>
    public object UIObject { get; } = uiObject;

    /// <inheritdoc/>
    public object Parameter { get; } = parameter;
}