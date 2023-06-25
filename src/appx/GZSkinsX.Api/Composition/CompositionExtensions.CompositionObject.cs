// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using Windows.UI.Composition;

namespace GZSkinsX.Api.Composition;

public static partial class CompositionExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetProperty"></param>
    /// <returns></returns>
    public static ColorKeyFrameAnimation CreateColorKeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateColorKeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static ExpressionAnimation CreateExpressionAnimation(this CompositionObject obj)
    {
        return obj.TryAddGroup(obj.Compositor.CreateExpressionAnimation());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetProperty"></param>
    /// <returns></returns>
    public static ExpressionAnimation CreateExpressionAnimation(this CompositionObject obj, string targetProperty)
    {
        return obj.TryAddGroup(obj.Compositor.CreateExpressionAnimation().SetTarget(targetProperty));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetProperty"></param>
    /// <returns></returns>
    public static QuaternionKeyFrameAnimation CreateQuaternionKeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateQuaternionKeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetProperty"></param>
    /// <returns></returns>
    public static SpringVector3NaturalMotionAnimation CreateSpringVector3Animation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateSpringVector3Animation().SetSafeTarget(targetProperty));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetProperty"></param>
    /// <returns></returns>
    public static ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateScalarKeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetProperty"></param>
    /// <returns></returns>
    public static Vector2KeyFrameAnimation CreateVector2KeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateVector2KeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetProperty"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateVector3KeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetProperty"></param>
    /// <returns></returns>
    public static Vector4KeyFrameAnimation CreateVector4KeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateVector4KeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c"></param>
    /// <param name="key"></param>
    /// <param name="create"></param>
    /// <returns></returns>
    public static T GetCached<T>(this CompositionObject c, string key, Func<T> create) where T : CompositionObject
    {
        return CompositionObjectPool.Shared.GetCached(c, key, create);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    public static T SetComment<T>(this T obj, string comment) where T : CompositionObject
    {
        obj.Comment = comment;
        return obj;
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="path"></param>
    /// <param name="animation"></param>
    /// <returns></returns>
    public static T SetImplicitAnimation<T>(this T obj, string path, ICompositionAnimationBase? animation) where T : CompositionObject
    {
        obj.ImplicitAnimations ??= obj.Compositor.CreateImplicitAnimationCollection();

        if (animation == null)
        {
            obj.ImplicitAnimations.Remove(path);
        }
        else
        {
            obj.ImplicitAnimations[path] = animation;
        }

        return obj;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="animation"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void StartAnimation(this CompositionObject obj, CompositionAnimation animation)
    {
        if (string.IsNullOrWhiteSpace(animation.Target))
        {
            throw new ArgumentNullException("Animation has no target");
        }

        obj.StartAnimation(animation.Target, animation);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="animation"></param>
    public static void StartAnimation(this CompositionObject obj, CompositionAnimationGroup animation)
    {
        obj.StartAnimationGroup(animation);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="animation"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void StopAnimation(this CompositionObject obj, CompositionAnimation animation)
    {
        if (string.IsNullOrWhiteSpace(animation.Target))
        {
            throw new ArgumentNullException("Animation has no target.");
        }

        obj.StopAnimation(animation.Target);
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="animation"></param>
    /// <returns></returns>
    private static T TryAddGroup<T>(this CompositionObject obj, T animation) where T : CompositionAnimation
    {
        if (obj is CompositionAnimationGroup group)
        {
            group.Add(animation);
        }

        return animation;
    }
}
