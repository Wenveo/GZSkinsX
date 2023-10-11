// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.ContextMenu;

/// <summary>
/// 表示与其组中其他单选菜单项互斥的菜单项。
/// </summary>
/// <remarks>
/// 如果该元素存在于菜单的根部，并且通过 <see cref="IContextMenuService.CreateCommandBarMenu"/> 进行创建上下文命令菜单，
/// 那么此元素类型将会被替换为 <see cref="IContextToggleMenuItem"/>，因为在上下文命令菜单中不支持此类型。
/// 不过在其它方面表现都正常。
/// </remarks>
public interface IContextRadioMenuItem : IContextMenuItem
{
    /// <summary>
    /// 通过当前 UI 上下文判断当前菜单项是否为选中状态。
    /// </summary>
    /// <param name="context">与当前上下文菜单所关联的 UI 上下文内容。</param>
    /// <returns>如果返回 true 则表示为选中状态，否则将表示为未选中的状态。</returns>
    bool IsChecked(IContextMenuUIContext context);

    /// <summary>
    /// 表示菜单项的默认点击行为。
    /// </summary>
    /// <param name="isChecked">表示是否为选中状态。</param>
    /// <param name="context">与当前上下文菜单所关联的 UI 上下文内容。</param>
    void OnClick(bool isChecked, IContextMenuUIContext context);
}
