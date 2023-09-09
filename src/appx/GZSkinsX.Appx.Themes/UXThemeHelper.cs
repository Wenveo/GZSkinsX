// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;

namespace GZSkinsX.Appx.Themes;

internal static partial class UXThemeHelper
{
    private static bool IsWindows10_1809_AtLatest { get; } = OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763);

    private static bool IsWindows10_1903_AtLatest { get; } = OperatingSystem.IsWindowsVersionAtLeast(10, 0, 18362);

    public static void ApplyThemeForApp(bool isDark)
    {
        if (IsWindows10_1903_AtLatest)
        {
            SetPreferredAppMode(isDark ? PreferredAppMode.AllowDark : PreferredAppMode.Default);
            FlushMenuThemes();
            return;
        }

        if (IsWindows10_1809_AtLatest)
        {
            AllowDarkModeForApp(isDark);
            FlushMenuThemes();
            return;
        }
    }

    // 1809
    [LibraryImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
    private static partial int AllowDarkModeForApp([MarshalAs(UnmanagedType.Bool)] bool allow);

    // 1809
    [LibraryImport("uxtheme.dll", EntryPoint = "#136", SetLastError = true)]
    private static partial int FlushMenuThemes();

    // 1903
    [LibraryImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
    private static partial int SetPreferredAppMode(PreferredAppMode preferredAppMode);

    private enum PreferredAppMode
    {
        Default,
        AllowDark,
        ForceDark,
        ForceLight
    };
}
