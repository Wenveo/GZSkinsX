// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 扩展类的配置。
/// </summary>
public sealed record class ExtensionConfiguration
{
    /// <summary>
    /// 获取有关扩展的描述性信息。
    /// </summary>
    public required ExtensionMetadata Metadata { get; init; }

    /// <summary>
    /// 初始化 <see cref="ExtensionConfiguration"/> 的新实例。
    /// </summary>
    public ExtensionConfiguration() { }
}
