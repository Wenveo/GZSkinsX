// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Game;
using GZSkinsX.Contracts.Settings;

namespace GZSkinsX.Appx.Game;

/// <summary>
/// 表示用于存储游戏服务的基本数据配置。
/// </summary>
[Shared, Export]
internal sealed class GameSettings
{
    /// <summary>
    /// 表示当前设置节点的 <seealso cref="Guid"/> 字符串值。
    /// </summary>
    private const string THE_GUID = "BFEEF60A-222B-422C-B459-83FC27E84290";

    /// <summary>
    /// 用于存储当前游戏区域的键字符串常量。
    /// </summary>
    private const string CURRENT_REGION_GUID = "CurrentRegion";

    /// <summary>
    /// 用于存储本地数据的数据节点。
    /// </summary>
    private readonly ISettingsSection _settingsSection;

    /// <summary>
    /// 表示当前游戏区域的字段。
    /// </summary>
    private GameRegion _currentRegion;

    /// <summary>
    /// 获取或设置当前游戏所在的区域。
    /// </summary>
    public GameRegion CurrentRegion
    {
        get => _currentRegion;
        set
        {
            if (_currentRegion != value)
            {
                _currentRegion = value;
                _settingsSection.Attribute(CURRENT_REGION_GUID, value);
            }
        }
    }

    /// <summary>
    /// 初始化 <see cref="GameSettings"/> 的新实例。
    /// </summary>
    public GameSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID, SettingsType.Local);
        _currentRegion = _settingsSection.Attribute<GameRegion>(CURRENT_REGION_GUID);
    }
}
