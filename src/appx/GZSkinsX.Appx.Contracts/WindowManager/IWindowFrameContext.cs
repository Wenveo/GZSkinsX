// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.WindowManager;

/// <summary>
/// 用于存储导出的 <see cref="IWindowFrame"/> 对象以及上下文数据。
/// </summary>
public interface IWindowFrameContext
{
    /// <summary>
    /// 获取当前上下文的 <see cref="IWindowFrame"/> 对象。
    /// </summary>
    IWindowFrame Value { get; }

    /// <summary>
    /// 获取当前上下文的 <see cref="WindowFrameContractAttribute"/> 元数据。
    /// </summary>
    WindowFrameContractAttribute Metadata { get; }
}
