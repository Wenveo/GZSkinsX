// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml;

namespace GZSkinsX.SDK.Commands;

/// <summary>
/// 表示派生自 <see cref="ICommandToggleButton"/> 的抽象类，提供从接口成员到 UI 属性的双向绑定支持
/// </summary>
public abstract class CommandToggleButtonVM : CommandButtonVM, ICommandToggleButton
{
    /// <inheritdoc/>
    public virtual bool IsChecked
    {
        get => _isChecked;
        protected set => SetProperty(ref _isChecked, value);
    }

    /// <inheritdoc/>
    public virtual void OnChecked(object sender, RoutedEventArgs e) => _isChecked = true;

    /// <inheritdoc/>
    public virtual void OnUnchecked(object sender, RoutedEventArgs e) => _isChecked = false;

    protected bool _isChecked;
}

