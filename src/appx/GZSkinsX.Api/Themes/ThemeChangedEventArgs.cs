// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Themes;

/// <summary>
/// 
/// </summary>
public sealed class ThemeChangedEventArgs : System.EventArgs
{
    /// <summary>
    /// 
    /// </summary>
    public Windows.UI.Xaml.ElementTheme CurrentTheme { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsHighContrast { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentTheme"></param>
    /// <param name="isHighContrast"></param>
    public ThemeChangedEventArgs(Windows.UI.Xaml.ElementTheme currentTheme, bool isHighContrast)
    {
        CurrentTheme = currentTheme;
        IsHighContrast = isHighContrast;
    }
}
