// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.ContextMenu;

/// <summary>
/// 关于在创建上下文命令菜单的可选配置。
/// </summary>
public interface ICommandBarMenuOptions : IContextMenuOptions
{
    /// <summary>
    /// 指示 CommandBarFlyout 是否应始终保持其“已展开”状态并阻止用户进入“已折叠”状态。 默认为 false。
    /// </summary>
    bool AlwaysExpanded { get; }

    /// <summary>
    /// 应用于子级上下文菜单的配置选项。
    /// </summary>
    IContextMenuOptions? SubMenuOptions { get; }
}
