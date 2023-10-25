// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;

namespace GZSkinsX.Contracts.MyMods;

/// <summary>
/// 有关模组视图中触发选择事件的相关参数。
/// </summary>
public sealed class MyModsViewSelectionChangedArgs : EventArgs
{
    /// <summary>
    /// 获取追加的选中项集合。
    /// </summary>
    public required IEnumerable<MyModItemViewModel> AddedItems { get; init; }

    /// <summary>
    /// 获取被移除的选中项集合。
    /// </summary>
    public required IEnumerable<MyModItemViewModel> RemovedItems { get; init; }
}
