// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Controls;

/// <summary>
/// 表示使用了特定的字体中的图标源 <seealso href="https://github.com/microsoft/fluentui-system-icons">Fluent System Icons</seealso> 作为字形。
/// </summary>
public sealed class FluentSystemIconSource : Microsoft.UI.Xaml.Controls.FontIconSource
{
    /// <summary>
    /// 初始化 <see cref="FluentSystemIconSource"/> 的新实例。
    /// </summary>
    public FluentSystemIconSource() => FontFamily = StaticFontFamilys.FluentSystemIcons;

    /// <summary>
    /// 使用特定的字符代码初始化 <see cref="FluentSystemIconSource"/> 的新实例。
    /// </summary>
    public FluentSystemIconSource(string glyph) : this() => Glyph = glyph;
}
