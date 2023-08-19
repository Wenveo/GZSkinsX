// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.MRTCore;

namespace GZSkinsX.Contracts.Helpers;

/// <summary>
/// 提供快速获取本地化资源的帮助类
/// </summary>
public static class ResourceHelper
{
    /// <summary>
    /// 用于获取本地化资源的内部 <see cref="IMRTCoreMap"/> 成员对象
    /// </summary>
    private static readonly IMRTCoreMap s_mrtCoreMap;

    /// <summary>
    /// 用于缓存本地化资源的字典
    /// </summary>
    private static readonly Dictionary<string, WeakReference> s_resxCache;

    /// <summary>
    /// 初始化 <see cref="ResourceHelper"/> 的静态成员
    /// </summary>
    static ResourceHelper()
    {
        s_mrtCoreMap = AppxContext.MRTCoreService.MainResourceMap;
        s_resxCache = new Dictionary<string, WeakReference>();
    }

    /// <summary>
    /// 根据传入的资源键的名称以获取本地化资源
    /// </summary>
    /// <param name="resourceKey">需要获取的本地化的资源的键</param>
    /// <returns>返回获取到的本地化的资源</returns>
    public static string GetLocalized(string resourceKey)
    {
        string? result;
        if (s_resxCache.TryGetValue(resourceKey, out var weakResx))
        {
            result = weakResx.Target as string;
            if (result is not null)
            {
                return result;
            }
        }

        result = s_mrtCoreMap.GetString(resourceKey);
        s_resxCache[resourceKey] = new WeakReference(result);

        return result;
    }

    /// <summary>
    /// 根据传入具有特定的标识符的资源键的名称以获取本地化资源
    /// </summary>
    /// <param name="resourceKey">需要获取的本地化的资源的键</param>
    /// <returns>如果传入的 <paramref name="resourceKey"/> 包含特定的标识符则会获取本地化的资源，否则将会返回原对象</returns>
    public static string GetResxLocalizedOrDefault(string resourceKey)
    {
        if (resourceKey.StartsWith("resx:"))
        {
            string? result;
            var cacheKey = resourceKey[5..];

            if (s_resxCache.TryGetValue(cacheKey, out var weakResx))
            {
                result = weakResx.Target as string;
                if (result is not null)
                {
                    return result;
                }
            }

            result = s_mrtCoreMap.GetString(cacheKey);
            s_resxCache[resourceKey] = new WeakReference(result);

            return result;
        }
        else
        {
            return resourceKey;
        }
    }
}
