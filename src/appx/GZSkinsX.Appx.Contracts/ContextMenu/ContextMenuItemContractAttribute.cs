// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

namespace GZSkinsX.Contracts.ContextMenu;

/// <summary>
/// 包含有关上下文菜单项的附加信息，并声明目标类以 <see cref="IContextMenuItem"/> 类型导出。
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ContextMenuItemContractAttribute : ExportAttribute
{
    /// <summary>
    /// 初始化 <see cref="ContextMenuItemContractAttribute"/> 的新实例，并以 <see cref="IContextMenuItem"/> 类型导出。
    /// </summary>
    public ContextMenuItemContractAttribute() : base(typeof(IContextMenuItem)) { }

    /// <summary>
    /// 表示该菜单项的 <see cref="System.Guid"/> 字符串值，该值具有唯一性。
    /// </summary>
    public required string? Guid { get; init; }

    /// <summary>
    /// 表示该菜单项所归属的父菜单项的 <see cref="System.Guid"/> 字符串值。
    /// </summary>
    public string? OwnerGuid { get; init; }

    /// <summary>
    /// 表示该菜单项所在的分组，格式以 "double,Guid" 形式表示。
    /// </summary>
    public string? Group { get; init; }

    /// <summary>
    /// 表示该菜单项位于分组中的排序顺序。
    /// </summary>
    public double Order { get; init; }
}
