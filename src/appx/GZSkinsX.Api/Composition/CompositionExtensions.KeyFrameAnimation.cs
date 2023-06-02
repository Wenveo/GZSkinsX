// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Numerics;

using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media.Animation;

namespace GZSkinsX.SDK.Composition;

public static partial class CompositionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="expression"></param>
    /// <param name="spline"></param>
    /// <returns></returns>
    public static T AddKeyFrame<T>(this T animation, float normalizedProgressKey, string expression, KeySpline spline) where T : KeyFrameAnimation
    {
        animation.InsertExpressionKeyFrame(normalizedProgressKey, expression, animation.Compositor.CreateCubicBezierEasingFunction(spline));
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="expression"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static T AddKeyFrame<T>(this T animation, float normalizedProgressKey, string expression, CompositionEasingFunction? ease = null) where T : KeyFrameAnimation
    {
        animation.InsertExpressionKeyFrame(normalizedProgressKey, expression, ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static ScalarKeyFrameAnimation AddKeyFrame(this ScalarKeyFrameAnimation animation, float normalizedProgressKey, float value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static ScalarKeyFrameAnimation AddKeyFrame(this ScalarKeyFrameAnimation animation, float normalizedProgressKey, float value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static ColorKeyFrameAnimation AddKeyFrame(this ColorKeyFrameAnimation animation, float normalizedProgressKey, Color value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector2KeyFrameAnimation AddKeyFrame(this Vector2KeyFrameAnimation animation, float normalizedProgressKey, Vector2 value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector2KeyFrameAnimation AddKeyFrame(this Vector2KeyFrameAnimation animation, float normalizedProgressKey, Vector2 value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, Vector3 value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, Vector3 value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(value, value, 0f), ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(value, value, 0f), animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddScaleKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(value, value, 1f), ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddScaleKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(value, value, 1f), animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float x, float y, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(x, y, 0f), ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float x, float y, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(x, y, 0f), animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float x, float y, float z, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(x, y, z), ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static Vector4KeyFrameAnimation AddKeyFrame(this Vector4KeyFrameAnimation animation, float normalizedProgressKey, Vector4 value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static QuaternionKeyFrameAnimation AddKeyFrame(this QuaternionKeyFrameAnimation animation, float normalizedProgressKey, Quaternion value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="normalizedProgressKey"></param>
    /// <param name="value"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    public static QuaternionKeyFrameAnimation AddKeyFrame(this QuaternionKeyFrameAnimation animation, float normalizedProgressKey, Quaternion value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="delayTime"></param>
    /// <param name="behavior"></param>
    /// <returns></returns>
    public static T SetDelay<T>(this T animation, double delayTime, AnimationDelayBehavior behavior) where T : KeyFrameAnimation
    {
        animation.DelayTime = TimeSpan.FromSeconds(delayTime);
        animation.DelayBehavior = behavior;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="delayTime"></param>
    /// <param name="behavior"></param>
    /// <returns></returns>
    public static T SetDelay<T>(this T animation, TimeSpan delayTime, AnimationDelayBehavior behavior) where T : KeyFrameAnimation
    {
        animation.DelayBehavior = behavior;
        animation.DelayTime = delayTime;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="behavior"></param>
    /// <returns></returns>
    public static T SetDelayBehavior<T>(this T animation, AnimationDelayBehavior behavior) where T : KeyFrameAnimation
    {
        animation.DelayBehavior = behavior;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    public static T SetDelayTime<T>(this T animation, double delayTime) where T : KeyFrameAnimation
    {
        animation.DelayTime = TimeSpan.FromSeconds(delayTime);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    public static T SetDelayTime<T>(this T animation, TimeSpan delayTime) where T : KeyFrameAnimation
    {
        animation.DelayTime = delayTime;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static T SetDirection<T>(this T animation, AnimationDirection direction) where T : KeyFrameAnimation
    {
        animation.Direction = direction;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static T SetDuration<T>(this T animation, double duration) where T : KeyFrameAnimation
    {
        if (duration >= 0)
            return SetDuration(animation, TimeSpan.FromSeconds(duration));
        else
            return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static T SetDuration<T>(this T animation, TimeSpan duration) where T : KeyFrameAnimation
    {
        animation.Duration = duration;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="iterationBehavior"></param>
    /// <returns></returns>
    public static T SetIterationBehavior<T>(this T animation, AnimationIterationBehavior iterationBehavior) where T : KeyFrameAnimation
    {
        animation.IterationBehavior = iterationBehavior;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="stopBehavior"></param>
    /// <returns></returns>
    public static T SetStopBehavior<T>(this T animation, AnimationStopBehavior stopBehavior) where T : KeyFrameAnimation
    {
        animation.StopBehavior = stopBehavior;
        return animation;
    }
}
