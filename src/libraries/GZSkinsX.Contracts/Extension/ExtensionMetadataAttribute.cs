// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 应用程序扩展的元数据
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ExtensionMetadataAttribute : Attribute
{
    /// <summary>
    /// 扩展的加载顺序
    /// </summary>
    public double Order { get; set; }
}
