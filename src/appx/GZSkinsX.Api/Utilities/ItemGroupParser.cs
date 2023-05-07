// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Utilities;

/// <summary>
/// 提供解析以特定格式表示组的字符串值的能力
/// </summary>
public static class ItemGroupParser
{
    /// <summary>
    /// 通过解析以特定格式表示组的字符串值来获取与组相关的元数据信息
    /// </summary>
    /// <param name="group">以特定格式表示组的字符串值</param>
    /// <param name="name">通过解析获得的组名。如果解析失败则表示为 <see cref="string.Empty"/></param>
    /// <param name="order">通过解析获得的组的排序顺序。如果解析失败则表示为 <see cref="double.NaN"/></param>
    /// <returns>如果解析成功后则会返回 <see cref="true"/>，否则将返回 <see cref="false"/></returns>
    public static bool TryParseGroup(string group, out string name, out double order)
    {
        var indexOfSeparator = group.IndexOf(',');
        if (indexOfSeparator == -1 || !double.TryParse(group[..indexOfSeparator++], out order))
        {
            name = string.Empty;
            order = double.NaN;
            return false;
        }

        name = group[indexOfSeparator..];
        return true;
    }
}
