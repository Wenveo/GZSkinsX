// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 该接口为所有命令项的基本接口，所有命令项都应继承于此，并以该接口类型导出。
/// </summary>
public interface ICommandBarItem
{
    /// <summary>
    /// 获取是否启用该 UI 元素。
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// 获取该 UI 元素是否可见。
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// 在 UI 初始化时触发的行为。
    /// </summary>
    /// <param name="ctx">与当前命令栏有关的 UI 上下文。</param>
    void OnInitialize(ICommandBarUIContext? ctx);

    /// <summary>
    /// 在载入 UI 元素时触发的行为。
    /// </summary>
    /// <param name="ctx">与当前命令栏有关的 UI 上下文。</param>
    void OnLoaded(ICommandBarUIContext? ctx);

    /// <summary>
    /// 在卸载 UI 元素时触发的行为。
    /// </summary>
    /// <param name="ctx">与当前命令栏有关的 UI 上下文。</param>
    void OnUnloaded(ICommandBarUIContext? ctx);
}
