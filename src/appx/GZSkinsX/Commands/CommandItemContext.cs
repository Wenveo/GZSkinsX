// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using GZSkinsX.SDK.Commands;

namespace GZSkinsX.Commands;

/// <summary>
/// 用于存储导出的 <see cref="ICommandItem"/> 对象以及 <see cref="CommandItemMetadataAttribute"/> 元数据
/// </summary>
internal sealed class CommandItemContext
{
    /// <summary>
    /// 当前上下文中的懒加载对象
    /// </summary>
    private readonly Lazy<ICommandItem, CommandItemMetadataAttribute> _lazy;

    /// <summary>
    /// 获取当前上下文的 <see cref="ICommandItem"/> 对象
    /// </summary>
    public ICommandItem Value => _lazy.Value;

    /// <summary>
    /// 获取当前上下文的 <see cref="CommandItemMetadataAttribute"/> 元数据
    /// </summary>
    public CommandItemMetadataAttribute Metadata => _lazy.Metadata;

    /// <summary>
    /// 初始化 <see cref="CommandItemContext"/> 的新实例
    /// </summary>
    public CommandItemContext(Lazy<ICommandItem, CommandItemMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
