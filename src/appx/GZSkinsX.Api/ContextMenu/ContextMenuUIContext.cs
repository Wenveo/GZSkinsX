// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示派生自 <see cref="IContextMenuUIContext"/> 的默认实现
/// </summary>
public class ContextMenuUIContext : IContextMenuUIContext
{
    /// <inheritdoc/>
    public object UIObject { get; }

    /// <inheritdoc/>
    public object Parameter { get; }

    /// <summary>
    /// 初始化 <see cref="ContextMenuUIContext"/> 的新实例
    /// </summary>
    /// <param name="uiObject">指定当前 UI 上下文中的 UI 对象</param>
    /// <param name="parameter">指定当前 UI 上下文中的参数</param>
    public ContextMenuUIContext(object uiObject, object parameter)
    {
        UIObject = uiObject;
        Parameter = parameter;
    }
}
