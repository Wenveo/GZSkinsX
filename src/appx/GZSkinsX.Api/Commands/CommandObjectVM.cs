// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;

namespace GZSkinsX.SDK.Commands;

/// <summary>
/// 表示派生自 <see cref="ICommandObject"/> 的抽象类，提供从接口成员到 UI 属性的双向绑定支持
/// </summary>
public abstract class CommandObjectVM : ObservableObject, ICommandObject
{
    /// <inheritdoc/>
    public virtual object? UIObject
    {
        get => _uiObject;
        protected set => SetProperty(ref _uiObject, value);
    }

    /// <inheritdoc/>
    public virtual void OnInitialize() { }

    protected object? _uiObject;
}
