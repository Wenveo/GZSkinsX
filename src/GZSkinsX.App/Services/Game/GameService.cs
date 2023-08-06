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

internal interface IGameService2
{
    Task InitializeAsync();
}

/// <inheritdoc cref="IGameService"/>
[Shared, Export(typeof(IGameService))]
internal sealed class GameService : IGameService, IGameService2
{
    /// <summary>
    /// 用于存储以及更新配置
    /// </summary>
    private readonly GameSettings _gameSettings;

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
    public StorageFolder? RootFolder { get; private set; }

    /// <summary>
    /// 初始化 <see cref="GameService"/> 的新实例
    /// </summary>
    [method: ImportingConstructor]
    public GameService(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
    }

    public async Task InitializeAsync()
    {
        RootFolder = await AppxContext.FutureAccessService.TryGetFolderAsync(FutureAccessItemConstants.Game_RootFolder_Name);
    }

    /// <inheritdoc/>
    public async Task<bool> TryUpdateAsync(StorageFolder? rootFolder, GameRegion region)
    {
        if (await _gameData.TryUpdateAsync(rootFolder, region))
        {
            _futureAccessService.TryRemove(FutureAccessItemConstants.Game_RootFolder_Name);
            _futureAccessService.Add(rootFolder!, FutureAccessItemConstants.Game_RootFolder_Name);

            RootFolder = rootFolder;
            _gameSettings.CurrentRegion = region;

            _loggingService.LogOkay($"GameService: Update game data successfully /p:RootDirectory={rootFolder!.Path} /p:GameRegion={region}");
            return true;
        }

        var path = rootFolder is not null ? rootFolder.Path : "<null>";
        _loggingService.LogWarning($"GameService: Failed to update game data /p:RootDirectory={path} /p:GameRegion={region}");
        return false;
    }
}
