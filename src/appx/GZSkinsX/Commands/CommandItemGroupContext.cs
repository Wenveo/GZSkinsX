// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;

namespace GZSkinsX.Commands;

/// <summary>
/// 用于存储命令项的组的上下文信息
/// </summary>
internal sealed class CommandItemGroupContext(string name, double order)
{
    /// <summary>
    /// 获取该分组的名称
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 获取该分组的排序顺序
    /// </summary>
    public double Order { get; } = order;

    /// <summary>
    /// 获取该分组中的子菜单项
    /// </summary>
    public List<CommandItemContext> Items { get; } = new List<CommandItemContext>();
}
