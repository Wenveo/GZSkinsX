// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.Commands;

/// <summary>
/// 提供用于枚举和创建命令栏的的基本功能实现
/// </summary>
public interface ICommandBarService
{
    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值创建一个新的 <see cref="CommandBar"/> 实现
    /// </summary>
    /// <param name="ownerGuidString">子命令项所归属的 <see cref="System.Guid"/> 字符串值</param>
    /// <returns>已创建的 <see cref="CommandBar"/> 类型实例</returns>
    CommandBar CreateCommandBar(string ownerGuidString);
}
