// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;

namespace GZSkinsX.Api.Themes;

/// <summary>
/// 表示应用程序的主题服务，并提供对主题的访问和设置
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// 获取当前应用程序中 UI 的实际元素主题
    /// </summary>
    ElementTheme ActualTheme { get; }

    /// <summary>
    /// 获取当前应用程序中的 UI 元素主题
    /// </summary>
    ElementTheme CurrentTheme { get; }

    /// <summary>
    /// UI 主题更改事件，在跟随系统主题更改或手动切换时触发
    /// </summary>
    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// 设置当前应用程序的 UI 主题
    /// </summary>
    /// <param name="newTheme">新的 UI 主题</param>
    /// <returns>当成功应用主题后返回 true，否则返回 false</returns>
    Task<bool> SetElementThemeAsync(ElementTheme newTheme);
}
