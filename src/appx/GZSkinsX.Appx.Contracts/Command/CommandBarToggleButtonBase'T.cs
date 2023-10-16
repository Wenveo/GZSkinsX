// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 表示派生自 <see cref="ICommandBarToggleButton"/> 的抽象类，提供从接口成员到 UI 属性的双向绑定的支持实现。
/// </summary>
public abstract partial class CommandBarToggleButtonBase<TUIContext> : CommandBarButtonBase, ICommandBarToggleButton
    where TUIContext : ICommandBarUIContext
{
    /// <inheritdoc cref="ICommandBarToggleButton.IsChecked"/>
    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty] protected bool _isChecked;

    /// <inheritdoc/>
    public virtual void OnChecked(TUIContext? ctx) { }

    /// <inheritdoc/>
    public virtual void OnUnchecked(TUIContext? ctx) { }

    /// <inheritdoc/>
    void ICommandBarToggleButton.OnChecked(ICommandBarUIContext? ctx) => OnChecked((TUIContext?)ctx);

    /// <inheritdoc/>
    void ICommandBarToggleButton.OnUnchecked(ICommandBarUIContext? ctx) => OnUnchecked((TUIContext?)ctx);
}
