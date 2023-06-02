// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.SDK.Controls;

/// <summary>
/// 表示使用了特定的字体中的图标源 <seealso href="https://learn.microsoft.com/zh-cn/windows/apps/design/style/segoe-ui-symbol-font">Segoe MDL2 Assets</seealso> 作为字形
/// </summary>
public sealed class SegoeMDL2AssetsSource : Microsoft.UI.Xaml.Controls.FontIconSource
{
    /// <summary>
    /// 初始化 <see cref="SegoeMDL2AssetsSource"/> 的新实例
    /// </summary>
    public SegoeMDL2AssetsSource()
    {
        FontFamily = StaticFontFamilys.SegoeMDL2Assets;
    }
}
