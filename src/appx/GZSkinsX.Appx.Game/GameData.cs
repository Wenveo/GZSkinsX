// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.IO;
using System.Linq;

using GZSkinsX.Api.Game;

namespace GZSkinsX.Appx.Game;

/// <inheritdoc cref="IGameData"/>
internal sealed class GameData : IGameData
{
    /// <summary>
    /// 表示 Game 文件夹的的名称，该名称始终为一个固定值
    /// </summary>
    private const string GAME_DIRECTORY_NAME = "Game";

    /// <summary>
    /// 表示 LOL 游戏程序的的文件名称，该名称始终为一个固定值
    /// </summary>
    private const string GAME_EXECUTE_NAME = "League of Legends.exe";

    /// <summary>
    /// 表示 LCU 文件夹的名称，该名称始终为一个固定值
    /// </summary>
    private const string LCU_DIRECTORY_NAME = "LeagueClient";

    /// <summary>
    /// 表示 LCU 客户端程序的文件名称，该名称始终为一个固定值
    /// </summary>
    private const string LCU_EXECUTE_NAME = "LeagueClient.exe";

    /// <inheritdoc/>
    public string GameDirectory { get; private set; }

    /// <inheritdoc/>
    public string GameExecutePath { get; private set; }

    /// <inheritdoc/>
    public string LCUDirectory { get; private set; }

    /// <inheritdoc/>
    public string LCUExecutePath { get; private set; }

    /// <summary>
    /// 初始化 <see cref="GameData"/> 的新实例
    /// </summary>
    public GameData()
    {
        GameDirectory = string.Empty;
        GameExecutePath = string.Empty;
        LCUDirectory = string.Empty;
        LCUExecutePath = string.Empty;
    }

    /// <summary>
    /// 尝试从传入指定的游戏目录以及区域来更新当前游戏数据的基本路径信息
    /// </summary>
    /// <param name="rootDirectory">游戏的根目录文件夹</param>
    /// <param name="region">游戏所在的区域服务器</param>
    /// <returns>在成功更新数据时返回 true，否则返回 false</returns>
    public bool TryUpdate(string rootDirectory, GameRegion region)
    {
        if (Directory.Exists(rootDirectory) && region != GameRegion.Unknown)
        {
            /// 定义基本的文件或文件夹路径
            var gameDirectory = Path.Combine(rootDirectory, GAME_DIRECTORY_NAME);
            var gameExecutePath = Path.Combine(gameDirectory, GAME_EXECUTE_NAME);

            var lcuDirectory = region == GameRegion.Riot
                ? rootDirectory
                : Path.Combine(rootDirectory, LCU_DIRECTORY_NAME);
            var lcuExecutePath = Path.Combine(lcuDirectory, LCU_EXECUTE_NAME);

            /// 这里用 DirectoryInfo 获取文件夹信息并判断文件是否存在
            var gameDirectoryInfo = new DirectoryInfo(gameDirectory);
            var lcuDirectoryInfo = new DirectoryInfo(lcuDirectory);

            /// 优先判断文件夹是否存在
            if (gameDirectoryInfo.Exists is false || lcuDirectoryInfo.Exists is false)
            {
                return false;
            }

            /// File.Exists 在这里不适用，且无法正确判断，因此只能通过获取文件列表进行判定
            if (gameDirectoryInfo.GetFiles(GAME_EXECUTE_NAME).Any(
                a => StringComparer.Ordinal.Equals(a.FullName, gameExecutePath))
                &&
                lcuDirectoryInfo.GetFiles(LCU_EXECUTE_NAME).Any(
                b => StringComparer.Ordinal.Equals(b.FullName, lcuExecutePath)))
            {
                GameDirectory = gameDirectory;
                GameExecutePath = gameExecutePath;
                LCUDirectory = lcuDirectory;
                LCUExecutePath = lcuExecutePath;

                return true;
            }
        }

        return false;
    }
}
