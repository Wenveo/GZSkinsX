// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Numerics;

using Windows.UI.Composition;
using Windows.UI.Xaml.Media.Animation;

namespace GZSkinsX.Uwp.Composition;

public static partial class CompositionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <param name="animations"></param>
    /// <returns></returns>
    public static CompositionAnimationGroup CreateAnimationGroup(this Compositor c, params CompositionAnimation[] animations)
    {
        var group = c.CreateAnimationGroup();
        foreach (var a in animations)
        {
            group.Add(a);
        }

        return group;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <returns></returns>
    public static CubicBezierEasingFunction CreateCubicBezierEasingFunction(this Compositor c, float x1, float y1, float x2, float y2)
    {
        return c.CreateCubicBezierEasingFunction(new Vector2(x1, y1), new Vector2(x2, y2));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <param name="spline"></param>
    /// <returns></returns>
    public static CubicBezierEasingFunction CreateCubicBezierEasingFunction(this Compositor c, KeySpline spline)
    {
        return c.CreateCubicBezierEasingFunction(spline);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static CubicBezierEasingFunction CreateEntranceEasingFunction(this Compositor c)
    {
        return c.CreateCubicBezierEasingFunction(new Vector2(.1f, .9f), new Vector2(.2f, 1));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="compositor"></param>
    /// <param name="key"></param>
    /// <param name="create"></param>
    /// <returns></returns>
    public static T GetCached<T>(this Compositor c, string key, Func<T> create) where T : CompositionObject
    {
        return CompositionObjectPool.Shared.GetCached(c, key, create);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static CubicBezierEasingFunction GetCachedEntranceEase(this Compositor c)
    {
        return c.GetCached("EntranceEase", c.CreateEntranceEasingFunction);
    }
}
