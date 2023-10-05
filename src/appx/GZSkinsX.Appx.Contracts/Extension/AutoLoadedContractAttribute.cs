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
/// 包含有关自动加载的扩展的附加信息，并声明目标类型为自动加载的扩展对象。
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AutoLoadedContractAttribute : ExportAttribute
{
    /// <summary>
    /// 初始化 <see cref="AutoLoadedContractAttribute"/> 的新实例，并以 <see cref="IExtensionClass"/> 类型导出。
    /// </summary>
    public AutoLoadedContractAttribute() : base(typeof(IExtensionClass))
    {
        LoadedWhen = AutoLoadedActivationConstraint.AppLoaded;
    }

    /// <summary>
    /// 获取和设置扩展的加载顺序。
    /// </summary>
    public double Order { get; set; }

    /// <summary>
    /// 获取和设置扩展加载应当何时加载。
    /// </summary>
    public AutoLoadedActivationConstraint LoadedWhen { get; set; }
}
