// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Api.Controls;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示派生自 <see cref="IContextToggleMenuItem"/> 的抽象基类，并提供基本的接口成员实现
/// </summary>
public abstract class ContextToggleMenuItemBase : IContextToggleMenuItem
{
    /// <inheritdoc/>
    public string? Header { get; protected set; }

    /// <inheritdoc/>
    public IconElement? Icon { get; protected set; }

    /// <inheritdoc/>
    public ShortcutKey? ShortcutKey { get; protected set; }

    /// <inheritdoc/>
    public object? ToolTip { get; protected set; }

    /// <inheritdoc/>
    public virtual bool IsChecked(IContextMenuUIContext context) => false;

    /// <inheritdoc/>
    public virtual bool IsEnabled(IContextMenuUIContext context) => true;

    /// <inheritdoc/>
    public virtual bool IsVisible(IContextMenuUIContext context) => true;

    /// <inheritdoc/>
    public virtual void OnExecute(IContextMenuUIContext context) { }

    /// <inheritdoc/>
    public virtual void OnClick(bool newValue, IContextMenuUIContext context) { }
}
