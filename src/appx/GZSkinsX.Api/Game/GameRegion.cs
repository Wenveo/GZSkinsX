// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Game;

/// <summary>
/// 表示英雄联盟游戏的区域/服务器
/// </summary>
public enum GameRegion
{
    /// <summary>
    /// 表示未知的区域/服务器，只有在未设置具体的区域时才使用
    /// </summary>
    Unknown,
    /// <summary>
    /// 拳头游戏直营服务器 (官服)，除大陆服以外的所有地区都是由拳头游戏管理
    /// </summary>
    Riot,
    /// <summary>
    /// 国服 (马服) 所在的服务器，由腾讯游戏运营
    /// </summary>
    Tencent
}
