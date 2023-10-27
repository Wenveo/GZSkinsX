// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 表示存放导航项相关的元数据静态类。
/// </summary>
public static class NavigationConstants
{
    /// <summary>
    /// Main / NavigationView
    /// </summary>
    public const string MAIN_NAV_GUID = "3CD3B21D-BC3D-4344-95C2-52C2EF68B626";

    /// <summary>
    /// Main / NavigationView / Mods
    /// </summary>
    public const string MAIN_NAV_MODS_GUID = "6ADAA585-3915-4689-A1E3-7418FD3055CD";

    /// <summary>
    /// Main / NavigationView / Group / General
    /// </summary>
    public const string GROUP_MAIN_NAV_GENERAL = "0,C1E738C6-0FBE-4AD7-8F42-D384A935540B";

    /// <summary>
    /// Main / NavigationView / Group / DevTools
    /// </summary>
    public const string GROUP_MAIN_NAV_DEVTOOLS = "1000,4C575955-C122-4CD2-A23F-06EAB2E1D378";

    /// <summary>
    /// Main / NavigationView / Mods - Order
    /// </summary>
    public const double ORDER_MAIN_NAV_MODS = 10;

    /// <summary>
    /// Settings / NavigationView
    /// </summary>
    public const string SETTINGS_NAV_GUID = "32BB66C9-7405-4E08-9577-43EEA165EBC2";

    /// <summary>
    /// Settings / NavigationView / General
    /// </summary>
    public const string SETTINGS_NAV_GENERAL_GUID = "A0176E55-C5C4-41F1-9B69-0E6E0C7FB7E9";

    /// <summary>
    /// Settings / NavigationView / Mods
    /// </summary>
    public const string SETTINGS_NAV_MODS_GUID = "B587DDD7-39CF-4BA0-B03E-7DE82F777A7F";

    /// <summary>
    /// Settings / NavigationView / Licenses
    /// </summary>
    public const string SETTINGS_NAV_LICENSES_GUID = "DD332445-54CE-4B42-906A-15795F607340";

    /// <summary>
    /// Settings / NavigationView / About
    /// </summary>
    public const string SETTINGS_NAV_ABOUT_GUID = "469065ED-D729-443A-8135-E0B6B34FD1B1";

    /// <summary>
    /// Settings / NavigationView / Group / Default
    /// </summary>
    public const string GROUP_SETTINGS_NAV_DEFAULT = "0,4C25B186-13B3-4AFC-AA58-80935C5E6A70";

    /// <summary>
    /// Settings / NavigationView / General - Order
    /// </summary>
    public const double ORDER_SETTINGS_NAV_GENERAL = 0;

    /// <summary>
    /// Settings / NavigationView / Mods - Order
    /// </summary>
    public const double ORDER_SETTINGS_NAV_MODS = 10;

    /// <summary>
    /// Settings / NavigationView / Licenses - Order
    /// </summary>
    public const double ORDER_SETTINGS_NAV_LICENSES = 100;

    /// <summary>
    /// Settings / NavigationView / About - Order
    /// </summary>
    public const double ORDER_SETTINGS_NAV_ABOUT = 10000;
}
