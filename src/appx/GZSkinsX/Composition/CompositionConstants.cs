// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.Appx;

namespace GZSkinsX.Composition;

/// <summary>
/// 存放静态成员或常量，该类通常会被 <see cref="CompositionContainerV2"/> 中使用
/// </summary>
internal static class CompositionConstants
{
    /// <summary>
    /// 缓存文件的名称 
    /// </summary>
    public const string CacheFileName = "mef-cacheV2.bin";

    /// <summary>
    /// 缓存文件的完整名称
    /// </summary>
    public static readonly string CacheFileFullPath;

    /// <summary>
    /// 初始化 <see cref="CompositionConstants"/> 的静态成员
    /// </summary>
    static CompositionConstants()
    {
        CacheFileFullPath = Path.Combine(AppxContext.AppxDirectory, CacheFileName);
    }
}
