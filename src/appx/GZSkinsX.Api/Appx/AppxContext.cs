// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Windows.ApplicationModel;

namespace GZSkinsX.Api.Appx;

/// <summary>
/// 当前 Appx 应用的上下文
/// </summary>
public static partial class AppxContext
{
    /// <summary>
    /// 初始化 <see cref="AppxContext"/> 的静态成员
    /// </summary>
    static AppxContext()
    {
        var length = 0;
        if (GetCurrentPackageFullName(ref length, null) != 15700L)
        {
            IsMsix = true;
            AppxDirectory = Package.Current.InstalledLocation.Path;

            var packageVersion = Package.Current.Id.Version;
            AppxVersion = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is not null && entryAssembly.Location is { Length: > 0 } &&
                entryAssembly.GetName(false) is AssemblyName entryAssemblyName &&
                entryAssemblyName.Version is not null)
            {
                AppxDirectory = entryAssembly.Location;
                AppxVersion = entryAssemblyName.Version;
            }
            else
            {
                AppxDirectory = Environment.CurrentDirectory;
                AppxVersion = new Version(2, 0, 0, 0);
            }
        }
    }

    /// <summary>
    /// 获取当前应用程序根目录
    /// </summary>
    public static string AppxDirectory { get; }

    /// <summary>
    /// 获取当前应用程序版本
    /// </summary>
    public static Version AppxVersion { get; }

    public static bool IsMsix { get; }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetCurrentPackageFullName(
        ref int packageFullNameLength,
        StringBuilder? packageFullName);
}
