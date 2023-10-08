// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 表示在命令栏中自定义的元素对象。
/// </summary>
public interface ICommandBarObject : ICommandBarItem
{
    /// <summary>
    /// 获取用于显示的 UI 元素对象。
    /// </summary>
    object? UIObject { get; }
}

