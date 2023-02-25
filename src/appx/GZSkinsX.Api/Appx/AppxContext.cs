// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Text;

using Windows.ApplicationModel;

namespace GZSkinsX.Api.Appx;

/// <summary>
/// 当前 Appx 应用的上下文
/// </summary>
public static class AppxContext
{
    /// <summary>
    /// 获取当前应用包的名称
    /// </summary>
    /// <param name="packageFullNameLength">用于返回字符串长度</param>
    /// <param name="packageFullName">用于存储应用包名称</param>
    /// <returns></returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetCurrentPackageFullName(
        ref int packageFullNameLength,
        StringBuilder? packageFullName);

    /// <summary>
    /// 初始化 <see cref="AppxContext"/> 的静态成员
    /// </summary>
    static AppxContext()
    {
        var length = 0;

        IsMSIX = GetCurrentPackageFullName(ref length, null) != 15700L;
        AppxDirectory = IsMSIX ? Package.Current.InstalledLocation.Path : Environment.CurrentDirectory;
        AppxVersion = new Version(2, 0, 0, 0);
    }

    /// <summary>
    /// 获取当前应用程序是否为 MSIX 应用
    /// </summary>
    public static bool IsMSIX { get; }

    /// <summary>
    /// 获取当前应用程序根目录
    /// <para>
    /// 根据 <see cref="IsMSIX"/> 的不同，返回的目录也不同
    /// </para>
    /// </summary>
    public static string AppxDirectory { get; }

    /// <summary>
    /// 获取当前应用程序版本
    /// </summary>
    public static Version AppxVersion { get; }
}
