// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Commands;

/// <summary>
/// 表示命令项位于命令栏中所处在的位置
/// </summary>
public enum CommandPlacement
{
    /// <summary>
    /// 表示位于命令栏中主要的集合，默认为展示的状态
    /// </summary>
    Primary,

    /// <summary>
    /// 表示位于命令栏中次要的集合，默认为收缩的状态
    /// </summary>
    Secondary
}
