// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace GZSkinsX.Contracts.WindowManager;

/// <summary>
/// 表示存放页面元素相关的元数据的静态类。
/// </summary>
public static class WindowFrameConstants
{
    /// <summary>
    /// 表示索引页的 <seealso cref="Guid"/> 字符串值。
    /// </summary>
    public const string Index_Guid = "D1E8EAD9-F18C-4BAD-9846-E97A27390A93";

    /// <summary>
    /// 表示主页的 <seealso cref="Guid"/> 字符串值。
    /// </summary>
    public const string Main_Guid = "95FF48E7-E179-4DA5-8F41-1923B7F22963";

    /// <summary>
    /// 表示预加载页的 <seealso cref="Guid"/> 字符串值。
    /// </summary>
    public const string Preload_Guid = "1D72AF65-7BDD-44AC-9EFC-D6454F95A4B9";

    /// <summary>
    /// 表示设置页的 <seealso cref="Guid"/> 字符串值。
    /// </summary>
    public const string Settings_Guid = "CE54A6CA-F261-401D-A811-AF211292686D";

    /// <summary>
    /// 表示启动页的 <seealso cref="Guid"/> 字符串值
    /// </summary>
    public const string StartUp_Guid = "1C3A5F43-5383-4B87-A365-1D1610B92060";
}
