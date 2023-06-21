// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Navigation;

/// <summary>
/// 表示存放导航项相关的元数据静态类
/// </summary>
public static class NavigationConstants
{
    /// <summary>
    /// 表示 Navigation Root 中的导航视图的 <see cref="System.Guid"/> 字符串值
    /// </summary>
    public const string NAVIGATIONROOT_NV_GUID = "3CD3B21D-BC3D-4344-95C2-52C2EF68B626";

    /// <summary>
    /// 表示 Navigation Root 中的导航视图的 'Main' 组
    /// </summary>
    public const string GROUP_NAVIGATIONROOT_NV_MAIN = "0,C1E738C6-0FBE-4AD7-8F42-D384A935540B";

    /// <summary>
    /// 表示 Navigation Root 中的导航视图的 'DevTools' 组
    /// </summary>
    public const string GROUP_NAVIGATIONROOT_NV_DEVTOOLS = "1000,4C575955-C122-4CD2-A23F-06EAB2E1D378";
}
