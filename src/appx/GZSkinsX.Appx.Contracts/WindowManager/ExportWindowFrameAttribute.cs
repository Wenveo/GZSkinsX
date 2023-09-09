// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

namespace GZSkinsX.Contracts.WindowManager;

/// <summary>
/// 声明并将目标类以 <see cref="IWindowFrame"/> 类型导出。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ExportWindowFrameAttribute : ExportAttribute
{
    /// <summary>
    /// 初始化 <see cref="ExportWindowFrameAttribute"/> 的新实例，并以 <see cref="IWindowFrame"/> 类型导出。
    /// </summary>
    public ExportWindowFrameAttribute() : base(typeof(IWindowFrame)) { }
}
