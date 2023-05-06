// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.Buffers;

/// <summary>
/// 提供一个资源池，支持重用派生自 <see cref="FontIcon"/> 的 <typeparamref name="T"/> 类型的实例
/// </summary>
/// <typeparam name="T">资源池中对象的类型</typeparam>
public sealed class FontIconPool<T> where T : FontIcon, new()
{
    /// <summary>
    /// 懒加载的资源池实例容器，仅在首次访问时会该创建资源池实例
    /// </summary>
    private static readonly Lazy<FontIconPool<T>> s_lazy = new(() => new());

    /// <summary>
    /// 获取共享的 <see cref="FontIconPool{T}"/> 的实例
    /// </summary>
    public static FontIconPool<T> Shared => s_lazy.Value;

    /// <summary>
    /// 用于实现对资源进行缓存的字典
    /// </summary>
    private readonly Dictionary<string, WeakReference> _fontIconCache;

    /// <summary>
    /// 初始化 <see cref="FontIconPool{T}"/> 的静态成员
    /// </summary>
    private FontIconPool()
    {
        _fontIconCache = new Dictionary<string, WeakReference>();
    }

    /// <summary>
    /// 从当前资源池中获取已缓存的对象或创建一个新的资源对象
    /// </summary>
    /// <param name="glyph">图标符号的字符代码</param>
    /// <returns>返回当前资源池中已缓存的对象，如果未找到缓存项则会创建一个新的缓存项并将其返回</returns>
    public T GetCacheOrCreate(string glyph)
    {
        T? result;

        if (_fontIconCache.TryGetValue(glyph, out var weakFontIcon))
        {
            result = weakFontIcon.Target as T;
            if (result is not null)
            {
                return result;
            }
        }

        result = new T() { Glyph = glyph };
        _fontIconCache[glyph] = new WeakReference(result);

        return result;
    }
}
