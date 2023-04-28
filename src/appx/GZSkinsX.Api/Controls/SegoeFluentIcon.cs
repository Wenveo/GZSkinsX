// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.Controls;

/// <summary>
/// 表示使用了特定字体 <seealso href="https://learn.microsoft.com/zh-cn/windows/apps/design/style/segoe-fluent-icons-font">Segoe Fluent Icon</seealso> 的字形图标
/// </summary>
public sealed class SegoeFluentIcon : FontIcon
{
    /// <summary>
    /// 初始化 <see cref="SegoeFluentIcon"/> 的新实例
    /// </summary>
    public SegoeFluentIcon()
    {
        FontFamily = StaticFontFamilys.SegoeFluentIcons;
    }
}
