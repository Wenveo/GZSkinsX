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
public abstract partial class CommandBarObjectVM : ObservableObject, ICommandBarObject
{
    /// <inheritdoc cref="ICommandBarItem.IsEnabled"/>
    [ObservableProperty] protected bool _isEnabled = true;

    /// <inheritdoc cref="ICommandBarItem.IsVisible"/>
    [ObservableProperty] protected bool _isVisible = true;

    /// <inheritdoc cref="ICommandBarObject.UIObject"/>
    [ObservableProperty] protected object? _uIObject;

    /// <inheritdoc/>
    public virtual void OnInitialize() { }
}
