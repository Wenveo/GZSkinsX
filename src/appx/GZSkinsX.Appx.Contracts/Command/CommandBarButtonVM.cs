// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 表示派生自 <see cref="ICommandBarButton"/> 的抽象类，提供从接口成员到 UI 属性的双向绑定的支持实现。
/// </summary>
public abstract partial class CommandBarButtonVM : ObservableObject, ICommandBarButton
{
    /// <inheritdoc cref="ICommandBarButton.DisplayName"/>
    [ObservableProperty] protected string? _displayName;

    /// <inheritdoc cref="ICommandBarButton.Icon"/>
    [ObservableProperty] protected IconElement? _icon;

    /// <inheritdoc cref="ICommandBarButton.IsEnabled"/>
    [ObservableProperty] protected bool _isEnabled = true;

    /// <inheritdoc cref="ICommandBarButton.IsVisible"/>
    [ObservableProperty] protected bool _isVisible = true;

    /// <inheritdoc cref="ICommandBarButton.KeyboardAccelerators"/>
    [ObservableProperty] protected IEnumerable<KeyboardAccelerator> _keyboardAccelerators = Array.Empty<KeyboardAccelerator>();

    /// <inheritdoc cref="ICommandBarButton.KeyboardAcceleratorTextOverride"/>
    [ObservableProperty] protected string? _keyboardAcceleratorTextOverride;

    /// <inheritdoc cref="ICommandBarButton.ToolTip"/>
    [ObservableProperty] protected object? _toolTip;

    /// <inheritdoc/>
    public virtual void OnClick() { }

    /// <inheritdoc/>
    public virtual void OnInitialize() { }
}
