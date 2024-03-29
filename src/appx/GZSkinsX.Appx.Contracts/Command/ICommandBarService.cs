// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 有关命令栏的服务接口，提供枚举元素创建命令栏的功能。
/// </summary>
public interface ICommandBarService
{
    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值创建一个新的 <see cref="CommandBar"/> 实现。
    /// </summary>
    /// <param name="ownerGuidString">子命令项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <returns>已创建的 <see cref="CommandBar"/> 类型实例。</returns>
    CommandBar CreateCommandBar(string ownerGuidString);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值创建一个新的 <see cref="CommandBar"/> 实现。
    /// </summary>
    /// <param name="ownerGuidString">子命令项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <param name="uiContext">指定与命令栏所关联的 UI 上下文。</param>
    /// <returns>已创建的 <see cref="CommandBar"/> 类型实例。</returns>
    CommandBar CreateCommandBar(string ownerGuidString, ICommandBarUIContext uiContext);
}
