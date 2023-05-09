// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

namespace GZSkinsX.Api.CreatorStudio.Commands;

/// <summary>
/// 表示命令项的元数据类
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class CommandItemMetadataAttribute : Attribute
{
    /// <summary>
    /// 获取和设置该命令项所归属的 <see cref="Guid"/> 字符串值
    /// </summary>
    public string? OwnerGuid { get; set; }

    /// <summary>
    /// 获取和设置该命令项所处在的组
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// 获取和设置该命令项位于组中的排序顺序
    /// </summary>
    public double Order { get; set; }

    /// <summary>
    /// 获取和设置命令项位于命令栏中所处的位置
    /// </summary>
    public CommandPlacement Placement { get; set; }
}
