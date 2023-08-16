// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Game;

using Windows.Storage;

namespace GZSkinsX.Game;

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
    private const string GAME_EXECUTE_File_NAME = "League of Legends.exe";

    /// <summary>
    /// 表示 LCU 文件夹的名称，该名称始终为一个固定值
    /// </summary>
    private const string LCU_DIRECTORY_NAME = "LeagueClient";

    /// <summary>
    /// 表示 LCU 客户端程序的文件名称，该名称始终为一个固定值
    /// </summary>
    private const string LCU_EXECUTE_File_NAME = "LeagueClient.exe";

    /// <inheritdoc/>
    public StorageFolder? GameFolder { get; private set; }

    /// <inheritdoc/>
    public StorageFile? GameExecuteFile { get; private set; }

    /// <inheritdoc/>
    public StorageFolder? LCUFolder { get; private set; }

    /// <inheritdoc/>
    public StorageFile? LCUExecuteFile { get; private set; }

    /// <summary>
    /// 尝试从传入指定的游戏目录以及区域来更新当前游戏数据的基本路径信息
    /// </summary>
    /// <param name="rootDirectory">游戏的根目录文件夹</param>
    /// <param name="region">游戏所在的区域服务器</param>
    /// <returns>在成功更新数据时返回 true，否则返回 false</returns>
    [MemberNotNullWhen(true, nameof(LCUFolder), nameof(LCUExecuteFile))]
    [MemberNotNullWhen(true, nameof(GameFolder), nameof(GameExecuteFile))]
    public async Task<bool> TryUpdateAsync(StorageFolder? rootFolder, GameRegion region)
    {
        if (rootFolder is not null && region is not GameRegion.Unknown)
        {
            IStorageItem gameFolder, lcuFolder, gameExecuteFile, lcuExecuteFile;

            gameFolder = await rootFolder.TryGetItemAsync(GAME_DIRECTORY_NAME);
            if (gameFolder is null || !gameFolder.IsOfType(StorageItemTypes.Folder))
            {
                return false;
            }

            if (region is GameRegion.Riot)
            {
                lcuFolder = rootFolder;
            }
            else
            {
                lcuFolder = await rootFolder.TryGetItemAsync(LCU_DIRECTORY_NAME);
                if (lcuFolder is null || !lcuFolder.IsOfType(StorageItemTypes.Folder))
                {
                    return false;
                }
            }

            gameExecuteFile = await ((StorageFolder)gameFolder).TryGetItemAsync(GAME_EXECUTE_File_NAME);
            if (gameExecuteFile is null || !gameExecuteFile.IsOfType(StorageItemTypes.File))
            {
                return false;
            }

            lcuExecuteFile = await ((StorageFolder)lcuFolder).TryGetItemAsync(LCU_EXECUTE_File_NAME);
            if (lcuExecuteFile is null || !lcuExecuteFile.IsOfType(StorageItemTypes.File))
            {
                return false;
            }

            GameFolder = (StorageFolder)gameFolder;
            GameExecuteFile = (StorageFile)gameExecuteFile;

            LCUFolder = (StorageFolder)lcuFolder;
            LCUExecuteFile = (StorageFile)lcuExecuteFile;

            return true;
        }

        return false;
    }
}
