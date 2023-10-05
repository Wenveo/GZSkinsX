// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 包含有关应用程序扩展的附加信息，并声明目标类型为应用程序扩展对象。
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ExtensionContractAttribute : ExportAttribute
{
    /// <summary>
    /// 初始化 <see cref="ExtensionContractAttribute"/> 的新实例，并以 <see cref="IExtension"/> 类型导出。
    /// </summary>
    public ExtensionContractAttribute() : base(typeof(IExtension)) { }

    /// <summary>
    /// 获取和设置扩展的加载顺序。
    /// </summary>
    public double Order { get; set; }
}
