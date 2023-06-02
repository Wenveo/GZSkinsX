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
/// 表示派生自 <see cref="ICommandToggleButton"/> 的抽象基类，并提供基本的接口成员实现
/// </summary>
public abstract class CommandToggleButtonBase : CommandButtonBase, ICommandToggleButton
{
    /// <inheritdoc/>
    public virtual bool IsChecked { get; protected set; }

    /// <inheritdoc/>
    public virtual void OnChecked(object sender, RoutedEventArgs e) { }

    /// <inheritdoc/>
    public virtual void OnUnchecked(object sender, RoutedEventArgs e) { }
}
