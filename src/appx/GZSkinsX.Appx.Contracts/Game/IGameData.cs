// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Game;

/// <summary>
/// 表示 "英雄联盟" 游戏的基本目录路径信息。
/// </summary>
public interface IGameData
{
    /// <summary>
    /// 英雄联盟游戏程序所在的文件夹。
    /// </summary>
    string? GameFolder { get; }

    /// <summary>
    /// 英雄联盟游戏程序的可执行文件。
    /// </summary>
    string? GameExecuteFile { get; }

    /// <summary>
    /// 英雄联盟客户端 (LCU) 所在的文件夹。
    /// </summary>
    string? LCUFolder { get; }

    /// <summary>
    /// 英雄联盟客户端 (LCU) 的可执行文件。
    /// </summary>
    string? LCUExecuteFile { get; }
}
