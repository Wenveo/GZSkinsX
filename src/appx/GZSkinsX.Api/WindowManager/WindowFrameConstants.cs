// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace GZSkinsX.Api.WindowManager;

/// <summary>
/// 表示存放页面元素相关的元数据的静态类
/// </summary>
public static class WindowFrameConstants
{
    /// <summary>
    /// 表示预加载页的 <seealso cref="Guid"/> 字符串值
    /// </summary>
    public const string Preload_Guid = "1D72AF65-7BDD-44AC-9EFC-D6454F95A4B9";

    /// <summary>
    /// 表示启动页的 <seealso cref="Guid"/> 字符串值
    /// </summary>
    public const string StartUp_Guid = "1C3A5F43-5383-4B87-A365-1D1610B92060";

    /// <summary>
    /// 表示导航页的 <seealso cref="Guid"/> 字符串值
    /// </summary>
    public const string NavigationRoot_Guid = "95FF48E7-E179-4DA5-8F41-1923B7F22963";
}
