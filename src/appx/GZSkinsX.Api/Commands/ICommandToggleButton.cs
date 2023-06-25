// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml;

namespace GZSkinsX.Api.Commands;

/// <summary>
/// 表示可在选中和未选中之间切换的按钮元素
/// </summary>
public interface ICommandToggleButton : ICommandButton
{
    /// <summary>
    /// 表示该元素是否为选中的状态
    /// </summary>
    bool IsChecked { get; }

    /// <summary>
    /// 表示在切换至选中状态中的行为
    /// </summary>
    void OnChecked(object sender, RoutedEventArgs e);

    /// <summary>
    /// 表示在切换至未选中状态中的行为
    /// </summary>
    void OnUnchecked(object sender, RoutedEventArgs e);
}
