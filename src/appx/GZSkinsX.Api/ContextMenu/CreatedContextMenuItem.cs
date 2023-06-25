// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示为通过自定义创建的上下文菜单项
/// </summary>
public readonly struct CreatedContextMenuItem
{
    /// <summary>
    /// 获取该上下文菜单项的元数据
    /// </summary>
    public ContextMenuItemMetadataAttribute? Metadata { get; }

    /// <summary>
    /// 获取该上下文菜单项
    /// </summary>
    public IContextMenuItem? ContextMenuItem { get; }

    /// <summary>
    /// 用于表示当前结构体中的成员是否未经过构造函数赋值且内容为空
    /// </summary>
    [MemberNotNullWhen(false, nameof(Metadata), nameof(ContextMenuItem))]
    public bool IsEmpty { get; }

    /// <summary>
    /// 初始化 <see cref="CreatedContextMenuItem"/> 的新实例
    /// </summary>
    public CreatedContextMenuItem()
    {
        IsEmpty = true;
    }

    /// <summary>
    /// 初始化 <see cref="CreatedContextMenuItem"/> 的新实例
    /// </summary>
    /// <param name="metadata">与上下文菜单项所关联的元数据</param>
    /// <param name="contextMenuItem">上下文菜单项的派生实现的实例</param>
    public CreatedContextMenuItem(ContextMenuItemMetadataAttribute metadata, IContextMenuItem contextMenuItem)
    {
        Metadata = metadata;
        ContextMenuItem = contextMenuItem;
    }
}
