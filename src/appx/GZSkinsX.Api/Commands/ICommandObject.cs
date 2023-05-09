// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

namespace GZSkinsX.Api.Commands;

/// <summary>
/// 表示在命令栏中自定义的元素对象
/// </summary>
public interface ICommandObject : ICommandItem
{
    /// <summary>
    /// 用于显示的 UI 元素对象
    /// </summary>
    object? UIObject { get; }
}
