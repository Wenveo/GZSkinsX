// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml;

namespace GZSkinsX.Api.Themes;

/// <summary>
/// 应用程序的 UI 主题更改时的事件参数
/// </summary>
public sealed class ThemeChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// 获取当前应用程序中的 UI 元素的实际主题
    /// </summary>
    public ElementTheme ActualTheme { get; }

    /// <summary>
    /// 获取当前应用程序中的 UI 的元素主题
    /// </summary>
    public ElementTheme CurrentTheme { get; }

    /// <summary>
    /// 表示是否为高对比度主题
    /// </summary>
    public bool IsHighContrast { get; }

    /// <summary>
    /// 初始化 <see cref="ThemeChangedEventArgs"/> 的新实例
    /// </summary>
    public ThemeChangedEventArgs(ElementTheme actualTheme, ElementTheme currentTheme, bool isHighContrast)
    {
        ActualTheme = actualTheme;
        CurrentTheme = currentTheme;
        IsHighContrast = isHighContrast;
    }
}
