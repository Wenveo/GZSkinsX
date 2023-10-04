// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.ContextMenu;

/// <summary>
/// 表示派生自 <see cref="IContextMenuUIContext"/> 的基本实现，并可通过泛型参数指定上下文中的成员类型。
/// </summary>
/// <typeparam name="T1">指定 <see cref="UIObject"/> 的类型。</typeparam>
/// <typeparam name="T2">指定 <see cref="Parameter"/> 的类型。</typeparam>
/// <param name="uiObject">指定当前 UI 上下文中的 UI 对象。</param>
/// <param name="parameter">指定当前 UI 上下文中的参数。</param>
public class ContextMenuUIContext<T1, T2>(T1 uiObject, T2 parameter)
    : ContextMenuUIContext(uiObject!, parameter!), IContextMenuUIContext<T1, T2>
{
    /// <inheritdoc cref="IContextMenuUIContext{T1, T2}.UIObject"/>
    public new T1 UIObject => ((T1)base.UIObject)!;

    /// <inheritdoc cref="IContextMenuUIContext{T1, T2}.Parameter"/>
    public new T2 Parameter => ((T2)base.Parameter)!;

    /// <inheritdoc/>
    T1 IContextMenuUIContext<T1, T2>.UIObject => ((T1)base.UIObject)!;

    /// <inheritdoc/>
    T2 IContextMenuUIContext<T1, T2>.Parameter => ((T2)base.Parameter)!;
}
