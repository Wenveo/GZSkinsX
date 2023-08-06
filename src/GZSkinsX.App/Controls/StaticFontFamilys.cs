// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml.Media;

namespace GZSkinsX.Controls;

/// <summary>
/// 用于存放程序内部中引用的一些字体
/// </summary>
internal static class StaticFontFamilys
{
    /// <summary>
    /// See <see href="https://learn.microsoft.com/zh-cn/windows/apps/design/style/segoe-ui-symbol-font"/>
    /// </summary>
    public static readonly FontFamily SegoeFluentIcons = new("/Assets/Fonts/Segoe Fluent Icons.ttf#Segoe Fluent Icons");

    /// <summary>
    /// See <see href="https://learn.microsoft.com/zh-cn/windows/apps/design/style/segoe-fluent-icons-font"/>
    /// </summary>
    public static readonly FontFamily SegoeMDL2Assets = new("/Assets/Fonts/Segoe MDL2 Assets.ttf#Segoe MDL2 Assets");
}
