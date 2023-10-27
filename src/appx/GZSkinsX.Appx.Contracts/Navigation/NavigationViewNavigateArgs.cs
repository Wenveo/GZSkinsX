// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Media.Animation;

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 用于在导航至导航视图页面时，指定要跳转的子导航项所传递的参数。
/// </summary>
public sealed class NavigationViewNavigateArgs : System.EventArgs
{
    /// <summary>
    /// 指定的与子导航项关联的 <see cref="System.Guid"/> 值。
    /// </summary>
    public required string NavItemGuid { get; init; }

    /// <summary>
    /// 用于传递给子导航项的参数。
    /// </summary>
    public object? Parameter { get; init; }

    /// <summary>
    /// 表示用于页面过渡的切换动画参数。
    /// </summary>
    public NavigationTransitionInfo? NavigationTransitionInfo { get; init; }
}
