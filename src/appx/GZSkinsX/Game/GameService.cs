// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.Api.Game;
using GZSkinsX.Api.Logging;

namespace GZSkinsX.Game;

/// <inheritdoc cref="IGameService"/>
[Shared, Export(typeof(IGameService))]
internal sealed class GameService : IGameService
{
    /// <summary>
    /// 用于存储以及更新配置
    /// </summary>
    private readonly GameSettings _gameSettings;

    /// <summary>
    /// 当前内部游戏数据实例
    /// </summary>
    private readonly GameData _gameData;

    /// <summary>
    /// 用于记录日志的日志服务
    /// </summary>
    private readonly ILoggingService _loggingService;

    /// <inheritdoc/>
    public GameRegion CurrentRegion => _gameSettings.CurrentRegion;

    /// <inheritdoc/>
    public IGameData GameData => _gameData;

    /// <inheritdoc/>
    public string RootDirectory => _gameSettings.RootDirectory;

    /// <summary>
    /// 初始化 <see cref="GameService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public GameService(GameSettings gameSettings, ILoggingService loggingService)
    {
        _gameSettings = gameSettings;
        _gameData = new GameData();

        _loggingService = loggingService;
    }

    /// <inheritdoc/>
    public bool TryUpdate(string rootDirectory, GameRegion region)
    {
        if (_gameData.TryUpdate(rootDirectory, region))
        {
            _gameSettings.RootDirectory = rootDirectory;
            _gameSettings.CurrentRegion = region;

            _loggingService.LogOkay($"GameService: Update game data successfully /p:RootDirectory={rootDirectory} /p:GameRegion={region}");
            return true;
        }

        _loggingService.LogWarning($"GameService: Failed to update game data /p:RootDirectory={rootDirectory} /p:GameRegion={region}");
        return false;
    }
}
