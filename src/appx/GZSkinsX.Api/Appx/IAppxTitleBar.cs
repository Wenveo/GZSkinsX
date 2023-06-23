// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI;

namespace GZSkinsX.Api.Appx;

/// <summary>
/// 提供对当前窗口标题栏管理的能力
/// </summary>
public interface IAppxTitleBar
{
    /// <summary>
    /// 获取和设置是否将当前窗口中的内容视图扩展至标题栏
    /// </summary>
    bool ExtendsContentIntoTitleBar { get; set; }

    /// <summary>
    /// 获取或设置标题栏前景色处于非活动状态时的颜色。
    /// </summary>
    /// <returns>非活动时标题栏前景的颜色。(See <see cref="Color"/>)</returns>
    Color? InactiveForegroundColor { get; set; }

    /// <summary>
    /// 获取或设置标题栏非活动时的背景颜色。
    /// </summary>
    /// <returns>非活动时标题栏背景的颜色。(See <see cref="Color"/>)</returns>
    Color? InactiveBackgroundColor { get; set; }

    /// <summary>
    /// 获取或设置标题栏前景的颜色。
    /// </summary>
    /// <returns>标题栏前景的颜色。(See <see cref="Color"/>)</returns>
    Color? ForegroundColor { get; set; }

    /// <summary>
    /// 获取或设置按下标题栏按钮时的前景色。
    /// </summary>
    /// <returns>按下标题栏按钮时的前景色。(See <see cref="Color"/>)</returns>
    Color? ButtonPressedForegroundColor { get; set; }

    /// <summary>
    /// 获取或设置按下标题栏按钮时的背景颜色。
    /// </summary>
    /// <returns>按下标题栏按钮时的背景颜色。(See <see cref="Color"/>)</returns>
    Color? ButtonPressedBackgroundColor { get; set; }

    /// <summary>
    /// 获取或设置处于非活动状态时标题栏按钮的前景色。
    /// </summary>
    /// <returns>标题栏按钮处于非活动状态时的前景色。(See <see cref="Color"/>)</returns>
    Color? ButtonInactiveForegroundColor { get; set; }

    /// <summary>
    /// 获取或设置处于非活动状态时标题栏按钮的背景颜色。
    /// </summary>
    /// <returns>标题栏按钮处于非活动状态时的背景颜色。(See <see cref="Color"/>)</returns>
    Color? ButtonInactiveBackgroundColor { get; set; }

    /// <summary>
    /// 获取或设置指针位于标题栏按钮上方时的前景色。
    /// </summary>
    /// <returns>当指针位于标题栏按钮上方时，该按钮的前景色。(See <see cref="Color"/>)</returns>
    Color? ButtonHoverForegroundColor { get; set; }

    /// <summary>
    /// 获取或设置指针位于标题栏按钮上方时的背景颜色。
    /// </summary>
    /// <returns>当指针在标题栏按钮上时，它的背景颜色。(See <see cref="Color"/>)</returns>
    Color? ButtonHoverBackgroundColor { get; set; }

    /// <summary>
    /// 获取或设置标题栏按钮的前景色。
    /// </summary>
    /// <returns>标题栏按钮的前景色。(See <see cref="Color"/>)</returns>
    Color? ButtonForegroundColor { get; set; }

    /// <summary>
    /// 获取或设置标题栏按钮的背景颜色。
    /// </summary>
    /// <returns>标题栏按钮的背景颜色。(See <see cref="Color"/>)</returns>
    Color? ButtonBackgroundColor { get; set; }

    /// <summary>
    /// 获取或设置标题栏背景的颜色。
    /// </summary>
    /// <returns>标题栏背景的颜色。(See <see cref="Color"/>)</returns>
    Color? BackgroundColor { get; set; }

    /// <summary>
    /// 设置当前窗口标题栏的界面元素
    /// </summary>
    /// <param name="value">需要设为标题栏的 UI 元素</param>
    void SetTitleBar(Microsoft.UI.Xaml.UIElement? value);
}
