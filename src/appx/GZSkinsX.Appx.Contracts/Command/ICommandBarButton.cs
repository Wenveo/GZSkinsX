// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 表示位于命令栏中基本的按钮元素。
/// </summary>
public interface ICommandBarButton : ICommandBarItem
{
    /// <summary>
    /// 获取在 UI 中显示的名称。
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// 获取在 UI 中显示的图标。
    /// </summary>
    IconElement? Icon { get; }

    /// <summary>
    /// 获取与命令项所绑定的键盘快捷键。
    /// </summary>
    IEnumerable<KeyboardAccelerator> KeyboardAccelerators { get; }

    /// <summary>
    /// 获取键盘快捷键的替代字符串。
    /// </summary>
    string? KeyboardAcceleratorTextOverride { get; }

    /// <summary>
    /// 获取该命令项的提示信息。
    /// </summary>
    object? ToolTip { get; }

    /// <summary>
    /// 命令按钮的默认点击事件行为。
    /// </summary>
    /// <param name="ctx">与当前命令栏有关的 UI 上下文。</param>
    void OnClick(ICommandBarUIContext? ctx);
}
