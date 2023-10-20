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
public abstract partial class CommandBarButtonBase<TUIContext> : ObservableObject, ICommandBarButton
    where TUIContext : ICommandBarUIContext
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

    /// <inheritdoc cref="ICommandBarButton.OnClick(ICommandBarUIContext?)"/>
    public virtual void OnClick(TUIContext? ctx) { }

    /// <inheritdoc cref="ICommandBarItem.OnInitialize(ICommandBarUIContext?)"/>
    public virtual void OnInitialize(TUIContext? ctx) { }

    /// <inheritdoc cref="ICommandBarItem.OnLoaded(ICommandBarUIContext?)"/>
    public virtual void OnLoaded(TUIContext? ctx) { }

    /// <inheritdoc cref="ICommandBarItem.OnUnloaded(ICommandBarUIContext?)"/>
    public virtual void OnUnloaded(TUIContext? ctx) { }

    /// <inheritdoc/>
    void ICommandBarButton.OnClick(ICommandBarUIContext? ctx) => OnClick((TUIContext?)ctx);

    /// <inheritdoc/>
    void ICommandBarItem.OnInitialize(ICommandBarUIContext? ctx) => OnInitialize((TUIContext?)ctx);

    /// <inheritdoc/>
    void ICommandBarItem.OnLoaded(ICommandBarUIContext? ctx) => OnLoaded((TUIContext?)ctx);

    /// <inheritdoc/>
    void ICommandBarItem.OnUnloaded(ICommandBarUIContext? ctx) => OnUnloaded((TUIContext?)ctx);
}
