// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml;

namespace GZSkinsX.Appx.Contracts.Themes;

/// <summary>
/// 应用程序的 UI 主题更改时的事件参数。
/// </summary>
public sealed class ThemeChangedEventArgs(ElementTheme actualTheme, ElementTheme currentTheme, bool isHighContrast) : System.EventArgs
{
    /// <summary>
    /// 获取当前应用程序中的 UI 元素的实际主题。
    /// </summary>
    public ElementTheme ActualTheme { get; } = actualTheme;

    /// <summary>
    /// 获取当前应用程序中的 UI 的元素主题。
    /// </summary>
    public ElementTheme CurrentTheme { get; } = currentTheme;

    /// <summary>
    /// 获取当前是否为高对比度主题。
    /// </summary>
    public bool IsHighContrast { get; } = isHighContrast;
}
