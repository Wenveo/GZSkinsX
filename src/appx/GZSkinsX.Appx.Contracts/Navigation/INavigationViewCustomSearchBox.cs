// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 有关自定义导航视图中的搜索框的相关定义。
/// </summary>
public interface INavigationViewCustomSearchBox
{
    /// <summary>
    /// 获取搜索框控件的对象实例。
    /// </summary>
    AutoSuggestBox SearchBoxControl { get; }

    /// <summary>
    /// 获取默认的搜索框占位文本。
    /// </summary>
    string? DefaultPlaceholderText { get; }
}
