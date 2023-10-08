// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 表示可在选中和未选中之间切换的按钮。
/// </summary>
public interface ICommandBarToggleButton : ICommandBarButton
{
    /// <summary>
    /// 获取该元素是否为选中的状态。
    /// </summary>
    bool IsChecked { get; }

    /// <summary>
    /// 在切换至选中状态中触发的事件行为。
    /// </summary>
    void OnChecked();

    /// <summary>
    /// 在切换至未选中状态中触发的事件行为。
    /// </summary>
    void OnUnchecked();
}
