// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.MotClient;

/// <summary>
/// 服务组件的启动参数信息。
/// </summary>
/// <param name="Name">启动参数的名称。</param>
/// <param name="Value">启动参数的具体值。</param>
public record class MTPackageMetadataStartUpArgument(string Name, string Value)
{
}
