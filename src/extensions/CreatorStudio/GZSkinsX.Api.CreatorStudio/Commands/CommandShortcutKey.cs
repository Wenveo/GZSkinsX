// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.System;

namespace GZSkinsX.Api.CreatorStudio.Commands;

/// <summary>
/// 用于表示位于命令栏中的命令项所指定的快捷键
/// </summary>
public sealed class CommandShortcutKey
{
    /// <summary>
    /// 表示虚拟密钥的值
    /// </summary>
    public VirtualKey Key { get; }

    /// <summary>
    /// 表示用于修改另一个键压的虚拟密钥
    /// </summary>
    public VirtualKeyModifiers Modifiers { get; }

    /// <summary>
    /// 初始化 <see cref="CommandShortcutKey"/> 的新实例
    /// </summary>
    /// <param name="key">指定虚拟密钥的值</param>
    /// <param name="modifiers">指定用于修改另一个键压的虚拟密钥</param>
    public CommandShortcutKey(VirtualKey key, VirtualKeyModifiers modifiers)
    {
        Key = key;
        Modifiers = modifiers;
    }
}
