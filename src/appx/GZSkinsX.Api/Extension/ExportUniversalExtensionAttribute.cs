// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

namespace GZSkinsX.Api.Extension;

/// <summary>
/// 声明目标类以 <see cref="IUniversalExtension"/> 类型导出
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ExportUniversalExtensionAttribute : ExportAttribute
{
    /// <summary>
    /// 初始化 <see cref="ExportUniversalExtensionAttribute"/> 的新实例，并以 <see cref="IUniversalExtension"/> 类型导出
    /// </summary>
    public ExportUniversalExtensionAttribute()
        : base(typeof(IUniversalExtension)) { }
}
