// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using CommunityToolkit.Mvvm.ComponentModel;

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 表示派生自 <see cref="ICommandBarObject"/> 的抽象类，提供从接口成员到 UI 属性的双向绑定的支持实现。
/// </summary>
public abstract partial class CommandBarObjectBase<TUIContext> : ObservableObject, ICommandBarObject
    where TUIContext : ICommandBarUIContext
{
    /// <inheritdoc cref="ICommandBarItem.IsEnabled"/>
    [ObservableProperty] protected bool _isEnabled = true;

    /// <inheritdoc cref="ICommandBarItem.IsVisible"/>
    [ObservableProperty] protected bool _isVisible = true;

    /// <inheritdoc cref="ICommandBarObject.UIObject"/>
    [ObservableProperty] protected object? _uIObject;

    /// <inheritdoc cref="ICommandBarItem.OnInitialize(ICommandBarUIContext?)"/>
    public virtual void OnInitialize(TUIContext? ctx) { }

    /// <inheritdoc cref="ICommandBarItem.OnLoaded(ICommandBarUIContext?)"/>
    public virtual void OnLoaded(TUIContext? ctx) { }

    /// <inheritdoc cref="ICommandBarItem.OnUnloaded(ICommandBarUIContext?)"/>
    public virtual void OnUnloaded(TUIContext? ctx) { }

    /// <inheritdoc/>
    void ICommandBarItem.OnInitialize(ICommandBarUIContext? ctx) => OnInitialize((TUIContext?)ctx);

    /// <inheritdoc/>
    void ICommandBarItem.OnLoaded(ICommandBarUIContext? ctx) => OnLoaded((TUIContext?)ctx);

    /// <inheritdoc/>
    void ICommandBarItem.OnUnloaded(ICommandBarUIContext? ctx) => OnUnloaded((TUIContext?)ctx);
}
