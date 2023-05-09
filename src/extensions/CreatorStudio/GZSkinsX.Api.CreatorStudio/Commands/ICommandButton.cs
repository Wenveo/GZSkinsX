// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.CreatorStudio.Commands;

/// <summary>
/// 表示位于命令栏中默认的按钮元素
/// </summary>
public interface ICommandButton : ICommandItem
{
    /// <summary>
    /// 表示用于在 UI 中显示的名称
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// 表示用于在 UI 中显示的图标
    /// </summary>
    IconElement? Icon { get; }

    /// <summary>
    /// 表示该命令项的快捷键
    /// </summary>
    CommandShortcutKey? ShortcutKey { get; }

    /// <summary>
    /// 表示该命令项的工具提示
    /// </summary>
    object? ToolTip { get; }

    /// <summary>
    /// 表示是否启用该 UI 元素
    /// </summary>
    /// <returns>如果返回 true 则表示启用 UI 元素，否则将表示禁用此 UI 元素</returns>
    bool IsEnabled();

    /// <summary>
    /// 表示该 UI 元素是否可见
    /// </summary>
    /// <returns>如果返回 true 则表示可见，否则将表示不可见</returns>
    bool IsVisible();

    /// <summary>
    /// 表示按钮的默认点击行为
    /// </summary>
    void OnClick(object sender, RoutedEventArgs e);
}
