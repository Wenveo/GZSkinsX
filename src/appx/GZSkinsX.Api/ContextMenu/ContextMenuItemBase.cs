// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示派生自 <see cref="IContextMenuItem"/> 的抽象基类，并提供基本的接口成员实现
/// </summary>
public abstract class ContextMenuItemBase : IContextMenuItem
{
    /// <inheritdoc/>
    public string? Header { get; protected set; }

    /// <inheritdoc/>
    public IconElement? Icon { get; protected set; }

    /// <inheritdoc/>
    public ContextMenuItemShortcutKey? ShortcutKey { get; protected set; }

    /// <inheritdoc/>
    public object? ToolTip { get; protected set; }

    /// <inheritdoc/>
    public virtual bool IsEnabled(IContextMenuUIContext context) => true;

    /// <inheritdoc/>
    public virtual bool IsVisible(IContextMenuUIContext context) => true;

    /// <inheritdoc/>
    public virtual void OnExecute(IContextMenuUIContext context) { }
}
