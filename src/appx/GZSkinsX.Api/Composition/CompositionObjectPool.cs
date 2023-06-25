// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;

using Windows.UI.Composition;

namespace GZSkinsX.Api.Composition;

/// <summary>
///
/// </summary>
public sealed class CompositionObjectPool
{
    /// <summary>
    ///
    /// </summary>
    private static readonly Lazy<CompositionObjectPool> s_lazy = new(() => new CompositionObjectPool());

    /// <summary>
    ///
    /// </summary>
    public static CompositionObjectPool Shared => s_lazy.Value;

    /// <summary>
    ///
    /// </summary>
    private readonly Dictionary<Compositor, Dictionary<string, WeakReference<CompositionObject>>> _objCache;

    /// <summary>
    ///
    /// </summary>
    private readonly object _lockObj;

    /// <summary>
    ///
    /// </summary>
    private CompositionObjectPool()
    {
        _lockObj = new();
        _objCache = new();
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="compositor"></param>
    /// <param name="key"></param>
    /// <param name="create"></param>
    /// <returns></returns>
    public T GetCached<T>(Compositor c, string key, Func<T> create) where T : CompositionObject
    {
        CompositionObject value;
        lock (_lockObj)
        {
            if (_objCache.TryGetValue(c, out var dic) is false)
            {
                _objCache[c] = dic = new();
            }

            if (dic.TryGetValue(key, out var weakRef))
            {
                if (weakRef.TryGetTarget(out value) is false)
                {
                    value = create();
                    weakRef.SetTarget(value);
                }
            }
            else
            {
                value = create();
                dic[key] = new(value);
            }
        }

        return (T)value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c"></param>
    /// <param name="key"></param>
    /// <param name="create"></param>
    /// <returns></returns>
    public T GetCached<T>(CompositionObject c, string key, Func<T> create) where T : CompositionObject
    {
        return GetCached(c.Compositor, key, create);
    }
}
