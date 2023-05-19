// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Api.Controls;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示上下文菜单中的菜单项，所有导出的上下文菜单项都继承于此接口
/// </summary>
public interface IContextMenuItem
{
    /// <summary>
    /// 获取位于菜单项中显示的标头内容
    /// </summary>
    string? Header { get; }

    /// <summary>
    /// 获取位于菜单项中显示的图标元素
    /// </summary>
    IconElement? Icon { get; }

    /// <summary>
    /// 获取与菜单项所绑定的快捷键
    /// </summary>
    ShortcutKey? ShortcutKey { get; }

    /// <summary>
    /// 获取与菜单项所关联的提示信息
    /// </summary>
    object? ToolTip { get; }

    /// <summary>
    /// 通过当前 UI 上下文判断是否应该显示此菜单项
    /// </summary>
    /// <param name="context">与当前上下文菜单所关联的 UI 上下文内容</param>
    /// <returns>如果返回 true 则表示可见，否则将表示不可见</returns>
    bool IsVisible(IContextMenuUIContext context);

    /// <summary>
    /// 通过当前 UI 上下文判断是否应该启用此菜单项
    /// </summary>
    /// <param name="context">与当前上下文菜单所关联的 UI 上下文内容</param>
    /// <returns>如果返回 true 则表示启用 UI 元素，否则将表示禁用此 UI 元素</returns>
    bool IsEnabled(IContextMenuUIContext context);

    /// <summary>
    /// 表示菜单项的默认点击行为
    /// </summary>
    /// <param name="context">与当前上下文菜单所关联的 UI 上下文内容</param>
    void OnExecute(IContextMenuUIContext context);
}
