// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Game;

/// <summary>
/// 表示提供对游戏基本信息的获取，但不包含具体的游戏内容
/// </summary>
public interface IGameService
{
    /// <summary>
    /// 表示当前游戏的区域/服务器
    /// </summary>
    GameRegion CurrentRegion { get; }

    /// <summary>
    /// 获取有关游戏的基本信息数据
    /// </summary>
    IGameData GameData { get; }

    /// <summary>
    /// 获取当前游戏的根目录路径
    /// </summary>
    string RootDirectory { get; }

    /// <summary>
    /// 尝试从传入指定的游戏目录以及区域来更新当前游戏数据的基本路径信息
    /// </summary>
    /// <param name="rootDirectory">游戏的根目录文件夹</param>
    /// <param name="region">游戏所在的区域服务器</param>
    /// <returns>在成功更新数据时返回 true，否则返回 false</returns>
    bool TryUpdate(string rootDirectory, GameRegion region);
}
