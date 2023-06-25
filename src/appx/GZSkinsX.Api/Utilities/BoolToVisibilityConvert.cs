// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml;

namespace GZSkinsX.Api.Utilities;

/// <summary>
/// 提供对 <see cref="bool"/> 和 <see cref="Visibility"/> 类型相互转换的能力
/// </summary>
public static class BoolToVisibilityConvert
{
    /// <summary>
    /// 将 <see cref="Visibility"/> 类型的值转换为 <see cref="bool"/> 类型
    /// </summary>
    /// <param name="value">需要转换的值</param>
    /// <returns>如果传入的值为 <see cref="Visibility.Visible"/> 则返回 true，否则返回 false </returns>
    public static bool ToBoolean(Visibility value)
    {
        return value == Visibility.Visible;
    }

    /// <summary>
    /// 将 <see cref="Visibility"/> 类型的值转换为 <see cref="bool"/> 类型
    /// </summary>
    /// <param name="value">需要转换的值</param>
    /// <returns>如果传入的值为 <see cref="Visibility.Collapsed"/> 则返回 true，否则返回 false </returns>
    public static bool ToBoolean2(Visibility value)
    {
        return value == Visibility.Collapsed;
    }

    /// <summary>
    /// 将 <see cref="bool"/> 类型的值转换为 <see cref="Visibility"/> 类型
    /// </summary>
    /// <param name="value">需要转换的值</param>
    /// <returns>如果传入的值为 true 则返回 <see cref="Visibility.Visible"/>，否则返回 <see cref="Visibility.Collapsed"/> </returns>
    public static Visibility ToVisibility(bool value)
    {
        return value ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// 将 <see cref="bool"/> 类型的值转换为 <see cref="Visibility"/> 类型
    /// </summary>
    /// <param name="value">需要转换的值</param>
    /// <returns>如果传入的值为 true 则返回 <see cref="Visibility.Collapsed"/>，否则返回 <see cref="Visibility.Visible"/> </returns>
    public static Visibility ToVisibility2(bool value)
    {
        return value ? Visibility.Collapsed : Visibility.Visible;
    }
}
