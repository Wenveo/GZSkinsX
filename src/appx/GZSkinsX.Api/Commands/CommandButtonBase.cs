// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.Commands;

/// <summary>
/// 表示派生自 <see cref="ICommandButton"/> 的抽象基类，并提供基本的接口成员实现
/// </summary>
public abstract class CommandButtonBase : ICommandButton
{
    /// <inheritdoc/>
    public virtual string? DisplayName { get; protected set; }

    /// <inheritdoc/>
    public virtual IconElement? Icon { get; protected set; }

    /// <inheritdoc/>
    public virtual bool IsEnabled { get; protected set; } = true;

    /// <inheritdoc/>
    public virtual bool IsVisible { get; protected set; } = true;

    /// <inheritdoc/>
    public virtual CommandShortcutKey? ShortcutKey { get; protected set; }

    /// <inheritdoc/>
    public virtual object? ToolTip { get; protected set; }

    /// <inheritdoc/>
    public virtual void OnClick(object sender, RoutedEventArgs e) { }

    /// <inheritdoc/>
    public virtual void OnInitialize() { }
}
