// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.Storage;

namespace GZSkinsX.Api.Game;

/// <summary>
/// 表示 "英雄联盟" 游戏的基本目录路径信息
/// </summary>
public interface IGameData
{
    /// <summary>
    /// 英雄联盟游戏程序所在的文件夹
    /// </summary>
    StorageFolder? GameFolder { get; }

    /// <summary>
    /// 英雄联盟游戏程序的可执行文件
    /// </summary>
    StorageFile? GameExecuteFile { get; }

    /// <summary>
    /// 英雄联盟客户端 (LCU) 所在的文件夹
    /// </summary>
    StorageFolder? LCUFolder { get; }

    /// <summary>
    /// 英雄联盟客户端 (LCU) 的可执行文件
    /// </summary>
    StorageFile? LCUExecuteFile { get; }
}
