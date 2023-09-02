// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Contracts.AccessCache;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Game;
using GZSkinsX.Contracts.Logging;

using Windows.Storage;

namespace GZSkinsX.Services.Game;

/// <inheritdoc cref="IGameService"/>
[Shared, Export(typeof(IGameService))]
[method: ImportingConstructor]
internal sealed class GameService(GameSettings gameSettings) : IGameService
{
    /// <summary>
    /// 表示该游戏的根目录在访问项存储列表中所关联的名称
    /// </summary>
    private const string GAME_ROOTFOLDER = "Game_RootFolder";

    /// <summary>
    /// 用于存储以及更新配置
    /// </summary>
    private readonly GameSettings _gameSettings = gameSettings;

    /// <summary>
    /// 当前内部游戏数据实例
    /// </summary>
    private readonly GameData _gameData = new();

    /// <summary>
    /// 用于记录日志的日志服务
    /// </summary>
    private readonly ILoggingService _loggingService = AppxContext.LoggingService;

    /// <summary>
    /// 用于管理未来访问的文件/文件夹项的服务
    /// </summary>
    private readonly IFutureAccessService _futureAccessService = AppxContext.FutureAccessService;

    /// <inheritdoc/>
    public GameRegion CurrentRegion => _gameSettings.CurrentRegion;

    /// <inheritdoc/>
    public IGameData GameData => _gameData;

    /// <inheritdoc/>
    public async Task<StorageFolder?> TryGetRootFolderAsync()
    {
        return await AppxContext.FutureAccessService.TryGetFolderAsync(GAME_ROOTFOLDER);
    }

    /// <inheritdoc/>
    public async Task<bool> TryUpdateAsync(StorageFolder? rootFolder, GameRegion region)
    {
        if (await _gameData.TryUpdateAsync(rootFolder, region))
        {
            _futureAccessService.Add(rootFolder!, GAME_ROOTFOLDER);
            _gameSettings.CurrentRegion = region;

            _loggingService.LogOkay("GZSkinsX.Services.GameService.TryUpdateAsync",
                $"Update game data successfully /p:GameRegion=\"{region}\" /p:RootDirectory=\"{rootFolder!.Path}\"");

            return true;
        }

        var path = rootFolder is not null ? rootFolder.Path : "<null>";
        _loggingService.LogWarning("GZSkinsX.Services.GameService.TryUpdateAsync",
            $"Failed to update game data /p:GameRegion=\"{region}\" /p:RootDirectory=\"{path}\".");

        return false;
    }
}
