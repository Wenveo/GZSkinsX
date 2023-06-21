// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Numerics;

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media.Animation;

namespace GZSkinsX.Api.Composition;

public static partial class CompositionExtensions
{
    public static T AddKeyFrame<T>(this T animation, float normalizedProgressKey, string expression, KeySpline spline) where T : KeyFrameAnimation
    {
        animation.InsertExpressionKeyFrame(normalizedProgressKey, expression, animation.Compositor.CreateCubicBezierEasingFunction(spline));
        return animation;
    }

    public static T AddKeyFrame<T>(this T animation, float normalizedProgressKey, string expression, CompositionEasingFunction? ease = null) where T : KeyFrameAnimation
    {
        animation.InsertExpressionKeyFrame(normalizedProgressKey, expression, ease);
        return animation;
    }

    public static ScalarKeyFrameAnimation AddKeyFrame(this ScalarKeyFrameAnimation animation, float normalizedProgressKey, float value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    public static ScalarKeyFrameAnimation AddKeyFrame(this ScalarKeyFrameAnimation animation, float normalizedProgressKey, float value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    public static ColorKeyFrameAnimation AddKeyFrame(this ColorKeyFrameAnimation animation, float normalizedProgressKey, Windows.UI.Color value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    public static Vector2KeyFrameAnimation AddKeyFrame(this Vector2KeyFrameAnimation animation, float normalizedProgressKey, Vector2 value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    public static Vector2KeyFrameAnimation AddKeyFrame(this Vector2KeyFrameAnimation animation, float normalizedProgressKey, Vector2 value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, Vector3 value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, Vector3 value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(value, value, 0f), ease);
        return animation;
    }

    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(value, value, 0f), animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    public static Vector3KeyFrameAnimation AddScaleKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(value, value, 1f), ease);
        return animation;
    }

    public static Vector3KeyFrameAnimation AddScaleKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(value, value, 1f), animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float x, float y, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(x, y, 0f), ease);
        return animation;
    }

    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float x, float y, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(x, y, 0f), animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    public static Vector3KeyFrameAnimation AddKeyFrame(this Vector3KeyFrameAnimation animation, float normalizedProgressKey, float x, float y, float z, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, new Vector3(x, y, z), ease);
        return animation;
    }

    public static Vector4KeyFrameAnimation AddKeyFrame(this Vector4KeyFrameAnimation animation, float normalizedProgressKey, Vector4 value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    public static QuaternionKeyFrameAnimation AddKeyFrame(this QuaternionKeyFrameAnimation animation, float normalizedProgressKey, Quaternion value, CompositionEasingFunction? ease = null)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, ease);
        return animation;
    }

    public static QuaternionKeyFrameAnimation AddKeyFrame(this QuaternionKeyFrameAnimation animation, float normalizedProgressKey, Quaternion value, KeySpline ease)
    {
        animation.InsertKeyFrame(normalizedProgressKey, value, animation.Compositor.CreateCubicBezierEasingFunction(ease));
        return animation;
    }

    public static CompositionAnimationGroup CreateAnimationGroup(this Compositor c, params CompositionAnimation[] animations)
    {
        var group = c.CreateAnimationGroup();
        foreach (var a in animations)
        {
            group.Add(a);
        }

        return group;
    }

    public static CubicBezierEasingFunction CreateCubicBezierEasingFunction(this Compositor c, float x1, float y1, float x2, float y2)
    {
        return c.CreateCubicBezierEasingFunction(new Vector2(x1, y1), new Vector2(x2, y2));
    }

    public static CubicBezierEasingFunction CreateCubicBezierEasingFunction(this Compositor c, KeySpline spline)
    {
        return c.CreateCubicBezierEasingFunction(spline);
    }

    public static CubicBezierEasingFunction CreateEntranceEasingFunction(this Compositor c)
    {
        return c.CreateCubicBezierEasingFunction(new Vector2(.1f, .9f), new Vector2(.2f, 1));
    }

    public static ColorKeyFrameAnimation CreateColorKeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateColorKeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    public static ExpressionAnimation CreateExpressionAnimation(this CompositionObject obj)
    {
        return obj.TryAddGroup(obj.Compositor.CreateExpressionAnimation());
    }

    public static ExpressionAnimation CreateExpressionAnimation(this CompositionObject obj, string targetProperty)
    {
        return obj.TryAddGroup(obj.Compositor.CreateExpressionAnimation().SetTarget(targetProperty));
    }

    public static QuaternionKeyFrameAnimation CreateQuaternionKeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateQuaternionKeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    public static SpringVector3NaturalMotionAnimation CreateSpringVector3Animation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateSpringVector3Animation().SetSafeTarget(targetProperty));
    }

    public static ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateScalarKeyFrameAnimation().SetSafeTarget(targetProperty));
    }
    public static Vector2KeyFrameAnimation CreateVector2KeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateVector2KeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    public static Vector3KeyFrameAnimation CreateVector3KeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateVector3KeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    public static Vector4KeyFrameAnimation CreateVector4KeyFrameAnimation(this CompositionObject obj, string? targetProperty = null)
    {
        return obj.TryAddGroup(obj.Compositor.CreateVector4KeyFrameAnimation().SetSafeTarget(targetProperty));
    }

    private static T TryAddGroup<T>(this CompositionObject obj, T animation) where T : CompositionAnimation
    {
        if (obj is CompositionAnimationGroup group)
        {
            group.Add(animation);
        }

        return animation;
    }

    public static UIElement EnableCompositionTranslation(this UIElement element)
    {
        return EnableCompositionTranslation(element, null);
    }

    public static UIElement EnableCompositionTranslation(this UIElement element, float x, float y, float z)
    {
        return EnableCompositionTranslation(element, new Vector3(x, y, z));
    }

    public static UIElement EnableCompositionTranslation(this UIElement element, Vector3? defaultTranslation)
    {
        var visual = GetElementVisual(element);
        if (visual.Properties.TryGetVector3(CompositionFactory.TRANSLATION, out _) == CompositionGetValueStatus.NotFound)
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            if (defaultTranslation.HasValue)
            {
                visual.Properties.InsertVector3(CompositionFactory.TRANSLATION, defaultTranslation.Value);
            }
            else
            {
                visual.Properties.InsertVector3(CompositionFactory.TRANSLATION, new Vector3());
            }
        }

        return element;
    }

    public static UIElement EnableTranslation(this UIElement element, bool enable)
    {
        ElementCompositionPreview.SetIsTranslationEnabled(element, enable);
        return element;
    }

    public static T GetCached<T>(this CompositionObject c, string key, Func<T> create) where T : CompositionObject
    {
        return CompositionObjectPool.Shared.GetCached(c, key, create);
    }

    public static T GetCached<T>(this Compositor c, string key, Func<T> create) where T : CompositionObject
    {
        return CompositionObjectPool.Shared.GetCached(c, key, create);
    }

    public static CubicBezierEasingFunction GetCachedEntranceEase(this Compositor c)
    {
        return c.GetCached("EntranceEase", c.CreateEntranceEasingFunction);
    }

    private static string LINKED_SIZE_EXPRESSION { get; } = $"{nameof(Visual)}.{nameof(Visual.Size)}";

    public static ExpressionAnimation CreateLinkedSizeExpression(Visual sourceVisual)
    {
        return sourceVisual.CreateExpressionAnimation(nameof(Visual.Size))
                           .SetParameter(nameof(Visual), sourceVisual)
                           .SetExpression(LINKED_SIZE_EXPRESSION);
    }

    public static Vector3 GetTranslation(this Visual visual)
    {
        visual.Properties.TryGetVector3(CompositionFactory.TRANSLATION, out var translation);
        return translation;
    }

    public static T LinkSize<T>(this T targetVisual, Visual sourceVisual) where T : Visual
    {
        targetVisual.StartAnimation(CreateLinkedSizeExpression(sourceVisual));
        return targetVisual;
    }

    public static T LinkSize<T>(this T targetVisual, FrameworkElement element) where T : Visual
    {
        targetVisual.StartAnimation(CreateLinkedSizeExpression(element.GetElementVisual()));
        return targetVisual;
    }

    public static T LinkShapeSize<T>(this T targetVisual, Visual sourceVisual) where T : CompositionGeometry
    {
        targetVisual.StartAnimation(CreateLinkedSizeExpression(sourceVisual));
        return targetVisual;
    }

    public static T SetComment<T>(this T obj, string comment) where T : CompositionObject
    {
        obj.Comment = comment;
        return obj;
    }

    public static Visual SetCenterPoint(this Visual visual, float x, float y, float z)
    {
        return SetCenterPoint(visual, new Vector3(x, y, z));
    }

    public static Visual SetCenterPoint(this Visual visual, Vector3 vector)
    {
        visual.CenterPoint = vector;
        return visual;
    }

    public static Visual SetCenterPoint(this Visual visual)
    {
        return SetCenterPoint(visual, new Vector3(visual.Size / 2f, 0f));
    }

    public static T SetDelay<T>(this T animation, double delayTime, AnimationDelayBehavior behavior) where T : KeyFrameAnimation
    {
        animation.DelayTime = TimeSpan.FromSeconds(delayTime);
        animation.DelayBehavior = behavior;
        return animation;
    }

    public static T SetDelay<T>(this T animation, TimeSpan delayTime, AnimationDelayBehavior behavior) where T : KeyFrameAnimation
    {
        animation.DelayBehavior = behavior;
        animation.DelayTime = delayTime;
        return animation;
    }

    public static T SetDelayBehavior<T>(this T animation, AnimationDelayBehavior behavior) where T : KeyFrameAnimation
    {
        animation.DelayBehavior = behavior;
        return animation;
    }

    public static T SetDelayTime<T>(this T animation, double delayTime) where T : KeyFrameAnimation
    {
        animation.DelayTime = TimeSpan.FromSeconds(delayTime);
        return animation;
    }

    public static T SetDelayTime<T>(this T animation, TimeSpan delayTime) where T : KeyFrameAnimation
    {
        animation.DelayTime = delayTime;
        return animation;
    }

    public static T SetDirection<T>(this T animation, AnimationDirection direction) where T : KeyFrameAnimation
    {
        animation.Direction = direction;
        return animation;
    }

    public static T SetDuration<T>(this T animation, double duration) where T : KeyFrameAnimation
    {
        if (duration >= 0)
            return SetDuration(animation, TimeSpan.FromSeconds(duration));
        else
            return animation;
    }

    public static T SetDuration<T>(this T animation, TimeSpan duration) where T : KeyFrameAnimation
    {
        animation.Duration = duration;
        return animation;
    }

    public static SpringVector3NaturalMotionAnimation SetDampingRatio(this SpringVector3NaturalMotionAnimation animation, float dampingRatio)
    {
        animation.DampingRatio = dampingRatio;
        return animation;
    }

    public static ExpressionAnimation SetExpression(this ExpressionAnimation animation, string expression)
    {
        animation.Expression = expression;
        return animation;
    }

    public static T SetFinalValue<T>(this T animation, Vector3 finalValue) where T : Vector3NaturalMotionAnimation
    {
        animation.FinalValue = finalValue;
        return animation;
    }

    public static T SetFinalValue<T>(this T animation, Vector3? value) where T : Vector3NaturalMotionAnimation
    {
        animation.FinalValue = value;
        return animation;
    }
    public static T SetFinalValue<T>(this T animation, float x, float y, float z) where T : Vector3NaturalMotionAnimation
    {
        animation.FinalValue = new Vector3(x, y, z);
        return animation;
    }

    public static T SetParameter<T>(this T animation, string key, UIElement parameter) where T : CompositionAnimation
    {
        if (parameter is not null)
        {
            animation.SetReferenceParameter(key, GetElementVisual(parameter));
        }

        return animation;
    }

    public static T SetParameter<T>(this T animation, string key, CompositionObject parameter) where T : CompositionAnimation
    {
        animation.SetReferenceParameter(key, parameter);
        return animation;
    }

    public static T SetParameter<T>(this T animation, string key, float parameter) where T : CompositionAnimation
    {
        animation.SetScalarParameter(key, parameter);
        return animation;
    }

    public static T SetParameter<T>(this T animation, string key, double parameter) where T : CompositionAnimation
    {
        animation.SetScalarParameter(key, (float)parameter);
        return animation;
    }

    public static T SetParameter<T>(this T animation, string key, Vector2 parameter) where T : CompositionAnimation
    {
        animation.SetVector2Parameter(key, parameter);
        return animation;
    }

    public static T SetParameter<T>(this T animation, string key, Vector3 parameter) where T : CompositionAnimation
    {
        animation.SetVector3Parameter(key, parameter);
        return animation;
    }

    public static T SetParameter<T>(this T animation, string key, Vector4 parameter) where T : CompositionAnimation
    {
        animation.SetVector4Parameter(key, parameter);
        return animation;
    }

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

    public static SpringVector3NaturalMotionAnimation SetPeriod(this SpringVector3NaturalMotionAnimation animation, TimeSpan duration)
    {
        animation.Period = duration;
        return animation;
    }

    public static Visual SetRotationAxis(this Visual visual, Vector3 axis)
    {
        visual.RotationAxis = axis;
        return visual;
    }

    public static Visual SetRotationAxis(this Visual visual, float x, float y, float z)
    {
        visual.RotationAxis = new Vector3(x, y, z);
        return visual;
    }

    public static Visual SetTranslation(this Visual visual, float x, float y, float z)
    {
        return SetTranslation(visual, new Vector3(x, y, z));
    }

    public static Visual SetTranslation(this Visual visual, Vector3 translation)
    {
        visual.Properties.InsertVector3(CompositionFactory.TRANSLATION, translation);
        return visual;
    }

    public static T SetTarget<T>(this T animation, string target) where T : CompositionAnimation
    {
        animation.Target = target;
        return animation;
    }

    private static T SetSafeTarget<T>(this T animation, string? target) where T : CompositionAnimation
    {
        if (string.IsNullOrEmpty(target) is false)
        {
            animation.Target = target;
        }

        return animation;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, float value)
    {
        set.InsertScalar(name, value);
        return set;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, bool value)
    {
        set.InsertBoolean(name, value);
        return set;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Vector2 value)
    {
        set.InsertVector2(name, value);
        return set;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Vector3 value)
    {
        set.InsertVector3(name, value);
        return set;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Vector4 value)
    {
        set.InsertVector4(name, value);
        return set;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Windows.UI.Color value)
    {
        set.InsertColor(name, value);
        return set;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Matrix3x2 value)
    {
        set.InsertMatrix3x2(name, value);
        return set;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Matrix4x4 value)
    {
        set.InsertMatrix4x4(name, value);
        return set;
    }

    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Quaternion value)
    {
        set.InsertQuaternion(name, value);
        return set;
    }

    public static FrameworkElement SetImplicitAnimation(this FrameworkElement element, string path, ICompositionAnimationBase? animation)
    {
        SetImplicitAnimation(GetElementVisual(element), path, animation);
        return element;
    }

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

    public static void StartAnimation(this CompositionObject obj, CompositionAnimation animation)
    {
        if (string.IsNullOrWhiteSpace(animation.Target))
        {
            throw new ArgumentNullException(nameof(animation), "Animation has no target");
        }

        obj.StartAnimation(animation.Target, animation);
    }

    public static void StartAnimation(this CompositionObject obj, CompositionAnimationGroup animation)
    {
        obj.StartAnimationGroup(animation);
    }

    public static void StopAnimation(this CompositionObject obj, CompositionAnimation animation)
    {
        if (string.IsNullOrWhiteSpace(animation.Target))
        {
            throw new ArgumentNullException(nameof(animation), "Animation has no target.");
        }

        obj.StopAnimation(animation.Target);
    }

    public static T SetIterationBehavior<T>(this T animation, AnimationIterationBehavior iterationBehavior) where T : KeyFrameAnimation
    {
        animation.IterationBehavior = iterationBehavior;
        return animation;
    }

    public static T SetStopBehavior<T>(this T animation, AnimationStopBehavior stopBehavior) where T : KeyFrameAnimation
    {
        animation.StopBehavior = stopBehavior;
        return animation;
    }

    public static Visual WithStandardTranslation(this Visual visual)
    {
        return CompositionFactory.EnableStandardTranslation(visual);
    }
}
