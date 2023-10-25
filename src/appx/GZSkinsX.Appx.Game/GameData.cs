// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using GZSkinsX.Contracts.Game;

namespace GZSkinsX.Appx.Game;

/// <inheritdoc cref="IGameData"/>
internal sealed class GameData : IGameData
{
    /// <summary>
    /// 表示 Game 文件夹的的名称，该名称始终为一个固定值。
    /// </summary>
    private const string GAME_DIRECTORY_NAME = "Game";

    /// <summary>
    /// 表示 LOL 游戏程序的的文件名称，该名称始终为一个固定值。
    /// </summary>
    private const string GAME_EXECUTE_File_NAME = "League of Legends.exe";

    /// <summary>
    /// 表示 LCU 文件夹的名称，该名称始终为一个固定值。
    /// </summary>
    private const string LCU_DIRECTORY_NAME = "LeagueClient";

    /// <summary>
    /// 表示 LCU 客户端程序的文件名称，该名称始终为一个固定值。
    /// </summary>
    private const string LCU_EXECUTE_File_NAME = "LeagueClient.exe";

    /// <summary>
    /// 定义此成员用于存储根路径，以及在更新时判断与先前的路径是否相同。
    /// </summary>
    private string? RootFolder { get; set; }

    /// <inheritdoc/>
    public string? GameFolder { get; private set; }

    /// <inheritdoc/>
    public string? GameExecuteFile { get; private set; }

    /// <inheritdoc/>
    public string? LCUFolder { get; private set; }

    /// <inheritdoc/>
    public string? LCUExecuteFile { get; private set; }

    /// <inheritdoc cref="IGameService.TryUpdate(string?, GameRegion)"/>
    [MemberNotNullWhen(true, nameof(LCUFolder), nameof(LCUExecuteFile))]
    [MemberNotNullWhen(true, nameof(GameFolder), nameof(GameExecuteFile))]
    public bool TryUpdate([NotNullWhen(true)] string? rootFolder, GameRegion region)
    {
        if (string.IsNullOrWhiteSpace(rootFolder) || region is GameRegion.Unknown)
        {
            return false;
        }

        if (Directory.Exists(rootFolder) is false)
        {
            return false;
        }

        var previousRootFolder = RootFolder;
        if (string.IsNullOrWhiteSpace(previousRootFolder) is false &&
            StringComparer.Ordinal.Equals(previousRootFolder, rootFolder))
        {
            // 如果与先前的根路径相同，则优先判断此前的成员路径是否有效。
            if (Directory.Exists(GameFolder) && File.Exists(GameExecuteFile)
                && Directory.Exists(LCUFolder) && File.Exists(LCUExecuteFile))
            {
                return true;
            }
        }

        var gameFolder = Path.Combine(rootFolder, GAME_DIRECTORY_NAME);
        if (Directory.Exists(gameFolder) is false)
        {
            return false;
        }

        var lcuFolder = region is GameRegion.Riot ? rootFolder : Path.Combine(rootFolder, LCU_DIRECTORY_NAME);
        if (Directory.Exists(lcuFolder) is false)
        {
            return false;
        }

        var gameExecuteFile = Path.Combine(gameFolder, GAME_EXECUTE_File_NAME);
        if (File.Exists(gameExecuteFile) is false)
        {
            return false;
        }

        var lcuExecuteFile = Path.Combine(lcuFolder, LCU_EXECUTE_File_NAME);
        if (File.Exists(lcuExecuteFile) is false)
        {
            return false;
        }

        RootFolder = rootFolder;
        GameFolder = gameFolder;
        GameExecuteFile = gameExecuteFile;

        LCUFolder = lcuFolder;
        LCUExecuteFile = lcuExecuteFile;

        return true;
    }
}
