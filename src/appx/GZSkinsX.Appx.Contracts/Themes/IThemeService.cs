// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;

using Windows.UI.ViewManagement;

namespace GZSkinsX.Contracts.Themes;

/// <summary>
/// 表示应用程序的主题服务，并提供对主题的访问和设置。
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// 获取和访问通用的 UI 设置。
    /// </summary>
    UISettings UISettings { get; }

    /// <summary>
    /// 获取当前应用程序中 UI 的实际元素主题。
    /// </summary>
    ElementTheme ActualTheme { get; }

    /// <summary>
    /// 获取当前应用程序中的 UI 元素主题。
    /// </summary>
    ElementTheme CurrentTheme { get; }

    /// <summary>
    /// 获取当前是否为高对比度主题。
    /// </summary>
    bool IsHighContrast { get; }

    /// <summary>
    /// 获取高对比度配色方案的名称。
    /// </summary>
    string HighContrastScheme { get; }

    /// <summary>
    /// UI 主题更改事件，在跟随系统主题更改或手动切换时触发。
    /// </summary>
    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// 设置当前应用程序的 UI 主题。
    /// </summary>
    /// <param name="newTheme">需要设置的新的应用主题。</param>
    Task SetElementThemeAsync(ElementTheme newTheme);
}
