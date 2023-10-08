// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

namespace GZSkinsX.Contracts.Command;

/// <summary>
/// 包含有关命令栏元素的附加信息，并声明目标类以 <see cref="ICommandBarItem"/> 类型导出。
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class CommandBarItemContractAttribute : ExportAttribute
{
    /// <summary>
    /// 初始化 <see cref="CommandBarItemContractAttribute"/> 的新实例，并以 <see cref="ICommandBarItem"/> 类型导出。
    /// </summary>
    public CommandBarItemContractAttribute() : base(typeof(ICommandBarItem)) { }

    /// <summary>
    /// 表示该元素所归属的父命令栏的 <see cref="Guid"/> 字符串值。
    /// </summary>
    public required string OwnerGuid { get; init; }

    /// <summary>
    /// 表示该元素所在的分组，格式以 "double,Guid" 形式表示。
    /// </summary>
    public string? Group { get; init; }

    /// <summary>
    /// 表示该元素位于分组中的排序顺序。
    /// </summary>
    public double Order { get; init; }

    /// <summary>
    /// 表示该元素位于命令栏中所处的位置。
    /// </summary>
    public CommandBarItemPlacement Placement { get; init; }
}
