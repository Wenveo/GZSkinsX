// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Diagnostics.CodeAnalysis;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Game;

namespace GZSkinsX.Appx.Game;

/// <inheritdoc cref="IGameService"/>
[Shared, Export(typeof(IGameService))]
internal sealed class GameService : IGameService
{
    /// <summary>
    /// 用于存储以及更新配置。
    /// </summary>
    private readonly GameSettings _settings;

    /// <summary>
    /// 当前内部游戏数据实例。
    /// </summary>
    private readonly GameData _gameData = new();

    /// <inheritdoc/>
    public GameRegion CurrentRegion => _settings.CurrentRegion;

    /// <inheritdoc/>
    public string? RootDirectory => _settings.RootDirectory;

    /// <inheritdoc/>
    public IGameData GameData => _gameData;

    /// <summary>
    /// 初始化 <see cref="GameService"/> 的新实例。
    /// </summary>
    public GameService()
    {
        _settings = AppxContext.Resolve<GameSettings>();
        TryUpdate(_settings.RootDirectory, _settings.CurrentRegion);
    }

    /// <inheritdoc/>
    public bool TryUpdate([NotNullWhen(true)] string? rootFolder, GameRegion region)
    {
        if (_gameData.TryUpdate(rootFolder, region))
        {
            _settings.CurrentRegion = region;
            _settings.RootDirectory = rootFolder;

            AppxContext.LoggingService.LogOkay("GZSkinsX.Appx.GameService.TryUpdate",
                $"Update game data successfully /p:GameRegion=\"{region}\" /p:RootDirectory=\"{rootFolder}\"");

            return true;
        }

        var path = rootFolder is not null ? rootFolder : "<null>";
        AppxContext.LoggingService.LogWarning("GZSkinsX.Appx.GameService.TryUpdateAsync",
            $"Failed to update game data /p:GameRegion=\"{region}\" /p:RootDirectory=\"{path}\".");

        return false;
    }
}
