// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.CreatorStudio.Commands;

/// <summary>
/// 表示存放命令项相关的元数据静态类
/// </summary>
public static class CommandConstants
{
    /// <summary>
    /// 表示位于 Creator Studio 中的命令栏的 <see cref="System.Guid"/> 字符串值
    /// </summary>
    public const string CREATOR_STUDIO_CB_GUID = "28D7FC51-F574-4E51-9E94-5645CDFCE13A";

    /// <summary>
    /// 表示位于 Creator Studio 中的命令栏的 'New' 组
    /// </summary>
    public const string GROUP_CREATORSTUDIO_CB_MAIN_NEW = "0,7EA4D40A-8A42-4C77-81AA-B30BE0A539D1";

    /// <summary>
    /// 表示位于 Creator Studio 中的命令栏的 'File' 组
    /// </summary>
    public const string GROUP_CREATORSTUDIO_CB_MAIN_FILE = "1000,B960CF32-F332-47E2-8AD2-DB5482607DAC";

    /// <summary>
    /// 表示位于 Creator Studio 中的命令栏的 'Edit' 组
    /// </summary>
    public const string GROUP_CREATORSTUDIO_CB_MAIN_EDIT = "2000,E420D6D2-670B-41A1-B33F-A2FECBE65B4A";

    /// <summary>
    /// 表示位于 Creator Studio 中的命令栏的 'View' 组
    /// </summary>
    public const string GROUP_CREATORSTUDIO_CB_MAIN_VIEW = "3000,AC652CBE-3333-45A6-9F13-D2F3A3D43806";
}
