// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 包含有关导航项的附加信息，并声明目标类以 <see cref="INavigationItem"/> 类型导出。
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class NavigationItemContractAttribute : ExportAttribute
{
    /// <summary>
    /// 初始化 <see cref="NavigationItemContractAttribute"/> 的新实例，并以 <see cref="INavigationItem"/> 类型导出。
    /// </summary>
    public NavigationItemContractAttribute() : base(typeof(INavigationItem)) { }

    /// <summary>
    /// 表示该导航项的 <see cref="System.Guid"/> 字符串值，该值具有唯一性。
    /// </summary>
    public required string Guid { get; init; }

    /// <summary>
    /// 表示该导航项所在的分组，格式以 "double,Guid" 形式表示。
    /// </summary>
    public string? Group { get; init; }

    /// <summary>
    /// 表示该导航项标头，用于在 UI 中显示。
    /// </summary>
    public string? Header { get; init; }

    /// <summary>
    /// 表示该导航项位于分组中的排序顺序。
    /// </summary>
    public double Order { get; init; }

    /// <summary>
    /// 表示该导航项所归属的父对象的 <see cref="System.Guid"/> 字符串值。
    /// </summary>
    public string? OwnerGuid { get; init; }

    /// <summary>
    /// 表示该导航项所关联的页面类型。
    /// </summary>
    public Type? PageType { get; init; }

    /// <summary>
    /// 表示该导航项所处的位置。
    /// </summary>
    public NavigationItemPlacement Placement { get; init; }
}
