// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Navigation;

/// <summary>
/// 表示存放导航项相关的元数据静态类
/// </summary>
public static class NavigationConstants
{
    /// <summary>
    /// 表示 MAIN 组的 Guid 字符串常量
    /// </summary>
    public const string MAIN_GROUP = "3689AAA9-73AB-43A2-822D-2CBFDB941C3A";

    /// <summary>
    /// 表示 DEV_TOOLS 组的 Guid 字符串常量
    /// </summary>
    public const string DEV_TOOLS_GROUP = "855F915C-FCFC-4C44-880A-BE9EEAF9ACBB";

    /// <summary>
    /// 表示 MAIN 组在列表中的顺序
    /// </summary>
    public const double ORDER_MAIN_GROUP = 0d;

    /// <summary>
    /// 表示 DEV_TOOLS 组在列表中的顺序
    /// </summary>
    public const double ORDER_DEV_TOOLS_GROUP = 1000d;

    /// <summary>
    /// 表示位于 MAIN 组的 Home 导航项的 Guid 字符串常量
    /// </summary>
    public const string MAIN_HOME_GUID = "7C9D47B5-879B-453F-9DBB-8EFF2B1FF96A";

    /// <summary>
    /// 表示位于 MAIN 组的 Mods 导航项的 Guid 字符串常量
    /// </summary>
    public const string MAIN_MODS_GUID = "CCBF07BC-F727-4686-B6E7-4F70C39AC48F";

    /// <summary>
    /// 表示位于 DEV_TOOLS 组的 Creator Studio 导航项的 Guid 字符串常量
    /// </summary>
    public const string DEV_TOOLS_CREATORSTUDIO_GUID = "FDE0A366-0C47-4623-8C23-027A2F45AB4E";

    /// <summary>
    /// 表示 Home 导航项位于 MAIN 组中的排序顺序
    /// </summary>
    public const double ORDER_MAIN_GROUP_HOME = 0d;

    /// <summary>
    /// 表示 Mods 导航项位于 MAIN 组中的排序顺序
    /// </summary>
    public const double ORDER_MAIN_GROUP_MODS = 1d;

    /// <summary>
    /// 表示 Creator Studio 导航项位于 DEV_TOOLS 组中的排序顺序
    /// </summary>
    public const double ORDER_DEV_TOOLS_GROUP_CREATORSTUDIO = 0d;
}
