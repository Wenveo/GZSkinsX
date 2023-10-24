// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.Helpers;

using Microsoft.UI.Xaml.Media;

namespace GZSkinsX.Appx.Navigation;

/// <summary>
/// 用于存储有关查询项的数据的类。
/// </summary>
/// <param name="Title">用于呈现的标题字符串。</param>
/// <param name="Glyph">用于呈现的图标字形。</param>
/// <param name="FontFamily">用于呈现的图标字体。</param>
/// <param name="GuidString">与导航项关联的 <see cref="System.Guid"/> 字符串值。</param>
internal sealed record class QueryNavigationViewItem(string Title, string Glyph, FontFamily FontFamily, string GuidString)
{
    /// <summary>
    /// 表示空的查询项。
    /// </summary>
    public static QueryNavigationViewItem Empty = new(
        ResourceHelper.GetLocalized("GZSkinsX.Appx.Navigation/Resources/NavigationView_Query_NotResultFound"),
        string.Empty, FontFamily.XamlAutoFontFamily, string.Empty);

    /// <summary>
    /// 表示空的查询项的集合。
    /// </summary>
    public static QueryNavigationViewItem[] EmptyArray = new[] { Empty };

    /// <inheritdoc/>
    public override string ToString() => Title;
}
