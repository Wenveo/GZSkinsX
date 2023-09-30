// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.IO;

namespace GZSkinsX.Contracts.MyMods;

/// <summary>
/// 表示模组的基本信息。
/// </summary>
/// <param name="Name">模组名称。</param>
/// <param name="Author">作者名称。</param>
/// <param name="Description">描述信息。</param>
/// <param name="DateTime">创建日期。</param>
/// <param name="FileInfo">文件信息。</param>
public readonly record struct MyModInfo(string Name, string Author, string Description, string DateTime, FileInfo FileInfo)
{
    /// <summary>
    /// 表示空的模组信息。
    /// </summary>
    public static readonly MyModInfo Empty = new();

    /// <summary>
    /// 判断目标模组信息是否为空。
    /// </summary>
    /// <param name="modInfo">需要确认的模组信息。</param>
    /// <returns>返回 true 代表该模组信息不为空，否则将返回 false。</returns>
    public static bool IsEmpty(in MyModInfo modInfo)
    {
        if (modInfo == Empty) return true;
        if (modInfo.FileInfo is not null) return false;
        if (string.IsNullOrWhiteSpace(modInfo.Name) is false) return false;
        if (string.IsNullOrWhiteSpace(modInfo.Author) is false) return false;
        if (string.IsNullOrWhiteSpace(modInfo.Description) is false) return false;
        if (string.IsNullOrWhiteSpace(modInfo.DateTime) is false) return false;

        return true;
    }
}
