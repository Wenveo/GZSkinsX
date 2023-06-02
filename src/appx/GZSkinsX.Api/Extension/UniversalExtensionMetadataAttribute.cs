// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

namespace GZSkinsX.SDK.Extension;

/// <summary>
/// 表示通用扩展的元数据类
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class UniversalExtensionMetadataAttribute : Attribute
{
    /// <summary>
    /// 扩展的加载顺序
    /// </summary>
    public double Order { get; set; }
}
