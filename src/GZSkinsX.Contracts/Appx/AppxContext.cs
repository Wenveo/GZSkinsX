// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using Windows.ApplicationModel;
using Windows.Storage;

namespace GZSkinsX.Contracts.Appx;

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
        AppxDirectory = Package.Current.InstalledLocation;

        var packageVersion = Package.Current.Id.Version;
        AppxVersion = new(
            packageVersion.Major, packageVersion.Minor,
            packageVersion.Build, packageVersion.Revision);
    }

    /// <summary>
    /// 获取当前应用程序根目录
    /// </summary>
    public static StorageFolder AppxDirectory { get; }

    /// <summary>
    /// 获取当前应用程序版本
    /// </summary>
    public static Version AppxVersion { get; }
}
