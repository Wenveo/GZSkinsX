// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

using Windows.UI.Composition;

namespace GZSkinsX.Api.Composition;

internal sealed class CompositionObjectPool
{
    private static readonly Lazy<CompositionObjectPool> s_lazy = new(() => new CompositionObjectPool());

    public static CompositionObjectPool Shared => s_lazy.Value;

    private readonly Dictionary<Compositor, Dictionary<string, WeakReference<CompositionObject>>> _objCache;

    private readonly object _lockObj;

    private CompositionObjectPool()
    {
        _lockObj = new();
        _objCache = new();
    }

    public T GetCached<T>(Compositor c, string key, Func<T> create) where T : CompositionObject
    {
        CompositionObject? value;
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

    public T GetCached<T>(CompositionObject c, string key, Func<T> create)
        where T : CompositionObject => GetCached(c.Compositor, key, create);
}
