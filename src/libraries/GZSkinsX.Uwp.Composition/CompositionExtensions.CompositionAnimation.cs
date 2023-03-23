// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Numerics;

using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace GZSkinsX.Uwp.Composition;

public static partial class CompositionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="dampingRatio"></param>
    /// <returns></returns>
    public static SpringVector3NaturalMotionAnimation SetDampingRatio(this SpringVector3NaturalMotionAnimation animation, float dampingRatio)
    {
        animation.DampingRatio = dampingRatio;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static ExpressionAnimation SetExpression(this ExpressionAnimation animation, string expression)
    {
        animation.Expression = expression;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="finalValue"></param>
    /// <returns></returns>
    public static T SetFinalValue<T>(this T animation, Vector3 finalValue) where T : Vector3NaturalMotionAnimation
    {
        animation.FinalValue = finalValue;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T SetFinalValue<T>(this T animation, Vector3? value) where T : Vector3NaturalMotionAnimation
    {
        animation.FinalValue = value;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static T SetFinalValue<T>(this T animation, float x, float y, float z) where T : Vector3NaturalMotionAnimation
    {
        animation.FinalValue = new Vector3(x, y, z);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="key"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static T SetParameter<T>(this T animation, string key, UIElement parameter) where T : CompositionAnimation
    {
        if (parameter is not null)
        {
            animation.SetReferenceParameter(key, GetElementVisual(parameter));
        }

        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="key"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static T SetParameter<T>(this T animation, string key, CompositionObject parameter) where T : CompositionAnimation
    {
        animation.SetReferenceParameter(key, parameter);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="key"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static T SetParameter<T>(this T animation, string key, float parameter) where T : CompositionAnimation
    {
        animation.SetScalarParameter(key, parameter);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="key"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static T SetParameter<T>(this T animation, string key, double parameter) where T : CompositionAnimation
    {
        animation.SetScalarParameter(key, (float)parameter);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="key"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static T SetParameter<T>(this T animation, string key, Vector2 parameter) where T : CompositionAnimation
    {
        animation.SetVector2Parameter(key, parameter);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="key"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static T SetParameter<T>(this T animation, string key, Vector3 parameter) where T : CompositionAnimation
    {
        animation.SetVector3Parameter(key, parameter);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="key"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static T SetParameter<T>(this T animation, string key, Vector4 parameter) where T : CompositionAnimation
    {
        animation.SetVector4Parameter(key, parameter);
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static SpringVector3NaturalMotionAnimation SetPeriod(this SpringVector3NaturalMotionAnimation animation, double duration)
    {
        if (duration >= 0)
        {
            return SetPeriod(animation, TimeSpan.FromSeconds(duration));
        }
        else
        {
            return animation;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static SpringVector3NaturalMotionAnimation SetPeriod(this SpringVector3NaturalMotionAnimation animation, TimeSpan duration)
    {
        animation.Period = duration;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static T SetTarget<T>(this T animation, string target) where T : CompositionAnimation
    {
        animation.Target = target;
        return animation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="animation"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private static T SetSafeTarget<T>(this T animation, string? target) where T : CompositionAnimation
    {
        if (string.IsNullOrEmpty(target) is false)
        {
            animation.Target = target;
        }

        return animation;
    }
}
