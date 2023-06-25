// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Api.ContextMenu;

namespace GZSkinsX.ContextMenu;

/// <summary>
/// 用于存储导出的 <see cref="IContextMenuItem"/> 对象以及 <see cref="ContextMenuItemMetadataAttribute"/> 元数据
/// </summary>
internal sealed class ContextMenuItemContext(Lazy<IContextMenuItem, ContextMenuItemMetadataAttribute> lazy)
{
    /// <summary>
    /// 当前上下文中的懒加载对象
    /// </summary>
    private readonly Lazy<IContextMenuItem, ContextMenuItemMetadataAttribute> _lazy = lazy;

    /// <summary>
    /// 获取当前上下文的 <see cref="IContextMenuItem"/> 对象
    /// </summary>
    public IContextMenuItem Value => _lazy.Value;

    /// <summary>
    /// 获取当前上下文的 <see cref="ContextMenuItemMetadataAttribute"/> 元数据
    /// </summary>
    public ContextMenuItemMetadataAttribute Metadata => _lazy.Metadata;
}
