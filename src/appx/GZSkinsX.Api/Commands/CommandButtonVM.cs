// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.Commands;

/// <summary>
/// 表示派生自 <see cref="ICommandButton"/> 的抽象类，提供从接口成员到 UI 属性的双向绑定支持
/// </summary>
public abstract class CommandButtonVM : ObservableObject, ICommandButton
{
    /// <inheritdoc/>
    public virtual string? DisplayName
    {
        get => _displayName;
        protected set => SetProperty(ref _displayName, value);
    }

    /// <inheritdoc/>
    public virtual IconElement? Icon
    {
        get => _icon;
        protected set => SetProperty(ref _icon, value);
    }

    /// <inheritdoc/>
    public virtual bool IsEnabled
    {
        get => _isEnabled;
        protected set => SetProperty(ref _isEnabled, value);
    }

    /// <inheritdoc/>
    public virtual bool IsVisible
    {
        get => _isVisible;
        protected set => SetProperty(ref _isVisible, value);
    }

    /// <inheritdoc/>
    public virtual CommandShortcutKey? ShortcutKey
    {
        get => _shortCutKey;
        protected set => SetProperty(ref _shortCutKey, value);
    }

    /// <inheritdoc/>
    public virtual object? ToolTip
    {
        get => _toolTip;
        protected set => SetProperty(ref _toolTip, value);
    }

    /// <inheritdoc/>
    public virtual void OnClick(object sender, RoutedEventArgs e)
    {

    }

    protected string? _displayName;
    protected IconElement? _icon;
    protected bool _isEnabled = true;
    protected bool _isVisible = true;
    protected CommandShortcutKey? _shortCutKey;
    protected object? _toolTip;
}
