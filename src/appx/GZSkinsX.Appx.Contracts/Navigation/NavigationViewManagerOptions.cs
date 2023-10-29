// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 有关创建 <see cref="INavigationViewManager"/> 时可选的配置选项。
/// </summary>
public sealed class NavigationViewManagerOptions
{
    /// <summary>
    /// （可选）与 <see cref="INavigationViewManager"/> 所绑定的导航视图 UI 对象。
    /// </summary>
    public NavigationView? Target { get; init; }

    /// <summary>
    /// 是否自动创建搜索框以及基本功能实现。
    /// </summary>
    public bool DoNotCreateSearchBox { get; init; }

    /// <summary>
    /// 用于显示在搜索框中的默认占位文本。
    /// </summary>
    public string? DefaultPlaceholderText { get; init; }
}
