// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示上下文菜单项的元数据类
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ContextMenuItemMetadataAttribute : Attribute
{
    /// <summary>
    /// 表示该菜单项的 <see cref="System.Guid"/> 字符串值，该值具有唯一性
    /// </summary>
    public string? Guid { get; set; }

    /// <summary>
    /// 表示该菜单项所归属的父菜单项的 <see cref="System.Guid"/> 字符串值
    /// </summary>
    public string? OwnerGuid { get; set; }

    /// <summary>
    /// 表示该菜单项所在的分组，格式以 "double,Guid" 形式表示
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// 表示该菜单项位于分组中的排序顺序
    /// </summary>
    public double Order { get; set; }
}
