// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

using Windows.Foundation.Metadata;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Core.Direct;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace GZSkinsX.Uwp.Composition;

/// <summary>
/// 
/// </summary>
public partial class CompositionFactory
{
    /// <summary>
    /// 
    /// </summary>
    public const string TRANSLATION = "Translation";

    /// <summary>
    /// 
    /// </summary>
    public const string STARTING_VALUE = "this.StartingValue";

    /// <summary>
    /// 
    /// </summary>
    public const string FINAL_VALUE = "this.FinalValue";

    /// <summary>
    /// 
    /// </summary>
    public const double DefaultOffsetDuration = 0.325;

    /// <summary>
    /// 
    /// </summary>
    public const int DEFAULT_STAGGER_MS = 83;

    /// <summary>
    /// 
    /// </summary>
    private static string CENTRE_EXPRESSION =>
        $"({nameof(Vector3)}(this.Target.{nameof(Visual.Size)}.{nameof(Vector2.X)} * {{0}}f, " +
        $"this.Target.{nameof(Visual.Size)}.{nameof(Vector2.Y)} * {{1}}f, 0f))";

    /// <summary>
    /// 
    /// </summary>
    public static UISettings UISettings { get; }

    /// <summary>
    /// 
    /// </summary>
    static CompositionFactory()
    {
        UISettings = new UISettings();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="from"></param>
    /// <param name="delayMs"></param>
    /// <param name="durationMs"></param>
    /// <returns></returns>
    public static ICompositionAnimationBase CreateEntranceAnimation(UIElement target, Vector3 from, int delayMs, int durationMs = 1000)
    {
        var key = $"CEA{from.X}{from.Y}{delayMs}{durationMs}";
        var c = target.EnableTranslation(true).GetElementVisual().Compositor;

        return c.GetCached(key, () =>
        {
            TimeSpan delay = TimeSpan.FromMilliseconds(delayMs);
            var e = c.GetCachedEntranceEase();
            var t = c.CreateVector3KeyFrameAnimation()
                .SetTarget(TRANSLATION)
                .SetDelayBehavior(AnimationDelayBehavior.SetInitialValueBeforeDelay)
                .SetDelayTime(delay)
                .AddKeyFrame(0, from)
                .AddKeyFrame(1, 0, e)
                .SetDuration(TimeSpan.FromMilliseconds(durationMs));

            var o = CreateFade(c, 1, 0, (int)(durationMs * 0.33), delayMs);
            return c.CreateAnimationGroup(t, o);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <param name="to"></param>
    /// <param name="from"></param>
    /// <param name="durationMs"></param>
    /// <param name="delayMs"></param>
    /// <returns></returns>
    public static CompositionAnimation CreateFade(Compositor c, float to, float? from, int durationMs, int delayMs = 0)
    {
        var key = $"SFade{to}{from}{durationMs}{delayMs}";
        return c.GetCached(key, () =>
        {
            var o = c.CreateScalarKeyFrameAnimation();
            o.Target = nameof(Visual.Opacity);
            if (from.HasValue)
            {
                o.InsertKeyFrame(0, from.Value);
            }

            o.InsertKeyFrame(1, to, c.CreateEntranceEasingFunction());
            o.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            o.DelayTime = TimeSpan.FromMilliseconds(delayMs);
            o.Duration = TimeSpan.FromMilliseconds(durationMs);
            return o;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static ICompositionAnimationBase CreateScaleAnimation(Compositor c)
    {
        return c.GetCached("ScaleAni", () =>
        {
            return c.CreateVector3KeyFrameAnimation()
                    .AddKeyFrame(1f, "this.FinalValue")
                    .SetDuration(DefaultOffsetDuration)
                    .SetTarget(nameof(Visual.Scale));
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation CreateSlideIn(UIElement e)
    {
        var v = e.EnableTranslation(true).GetElementVisual();
        return v.GetCached("_SLDI", () =>
        {
            return v.CreateVector3KeyFrameAnimation(TRANSLATION)
                    .AddKeyFrame(1, Vector3.Zero)
                    .SetDuration(DefaultOffsetDuration);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation CreateSlideOut(UIElement e, float x, float y)
    {
        var v = e.EnableTranslation(true).GetElementVisual();
        return v.GetCached("_SLDO", () =>
        {
            return v.CreateVector3KeyFrameAnimation(TRANSLATION)
                    .AddKeyFrame(0, STARTING_VALUE)
                    .AddKeyFrame(1, x, y, 0)
                    .SetDuration(DefaultOffsetDuration);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation CreateSlideOutX(UIElement e)
    {
        var v = e.EnableTranslation(true).GetElementVisual();
        return v.GetCached("SOX", () =>
        {
            return v.CreateVector3KeyFrameAnimation(TRANSLATION)
                    .AddKeyFrame(0, STARTING_VALUE)
                    .AddKeyFrame(1, "Vector3(this.Target.Size.X, 0, 0)")
                    .SetDuration(DefaultOffsetDuration);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static Vector3KeyFrameAnimation CreateSlideOutY(UIElement e)
    {
        var v = e.EnableTranslation(true).GetElementVisual();
        return v.GetCached("SOY", () =>
        {
            return v.CreateVector3KeyFrameAnimation(TRANSLATION)
                    .AddKeyFrame(0, STARTING_VALUE)
                    .AddKeyFrame(1, "Vector3(0, this.Target.Size.Y, 0)")
                    .SetDuration(DefaultOffsetDuration);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="f"></param>
    public static void DisableStandardReposition(FrameworkElement f)
    {
        f.GetElementVisual().ImplicitAnimations?.Remove(nameof(Visual.Offset));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static Visual EnableStandardTranslation(Visual v, double? duration = null)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return v;
        }

        var o = v.GetCached($"__ST{duration ?? DefaultOffsetDuration}", () =>
        {
            return v.CreateVector3KeyFrameAnimation(TRANSLATION)
                    .AddKeyFrame(0, STARTING_VALUE)
                    .AddKeyFrame(1, FINAL_VALUE)
                    .SetDuration(duration ?? DefaultOffsetDuration);
        });

        v.Properties.SetImplicitAnimation(TRANSLATION, o);
        return v;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static ImplicitAnimationCollection GetRepositionCollection(Compositor c)
    {
        return c.GetCached("RepoColl", () =>
        {
            var g = c.CreateAnimationGroup();
            g.Add(c.CreateVector3KeyFrameAnimation()
                    .SetTarget(nameof(Visual.Offset))
                    .AddKeyFrame(1f, "this.FinalValue")
                    .SetDuration(DefaultOffsetDuration));

            var s = c.CreateImplicitAnimationCollection();
            s.Add(nameof(Visual.Offset), g);
            return s;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="delayMs"></param>
    /// <param name="fromOffsetY"></param>
    /// <param name="fromOffsetX"></param>
    /// <param name="durationMs"></param>
    public static void PlayEntrance(UIElement target, int delayMs = 0, int fromOffsetY = 40, int fromOffsetX = 0, int durationMs = 1000)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        var animation = CreateEntranceAnimation(target, new Vector3(fromOffsetX, fromOffsetY, 0), delayMs, durationMs);
        target.GetElementVisual().StartAnimationGroup(animation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="delayMs"></param>
    /// <param name="fromOffsetY"></param>
    /// <param name="fromOffsetX"></param>
    /// <param name="durationMs"></param>
    /// <param name="staggerMs"></param>
    public static void PlayEntrance(List<UIElement> targets, int delayMs = 0, int fromOffsetY = 40, int fromOffsetX = 0, int durationMs = 1000, int staggerMs = 83)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        var start = delayMs;

        foreach (var target in targets)
        {
            var animation = CreateEntranceAnimation(target, new Vector3(fromOffsetX, fromOffsetY, 0), start, durationMs);
            target.GetElementVisual().StartAnimationGroup(animation);
            start += staggerMs;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    public static void PlayFullHeightSlideUpEntrance(FrameworkElement target)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        var v = target.EnableTranslation(true).GetElementVisual();
        var t = v.GetCached("_FHSU", () =>
        {
            return v.CreateVector3KeyFrameAnimation(TRANSLATION)
                    .AddKeyFrame(0, "Vector3(0, this.Target.Size.Y, 0)")
                    .AddKeyFrame(1, "Vector3(0, 0, 0)")
                    .SetDuration(DefaultOffsetDuration);
        });

        v.StartAnimationGroup(t);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public static void PlayScaleEntrance(FrameworkElement target, float from, float to)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        var v = target.GetElementVisual();
        if (target.Tag is null)
        {
            StartCentering(v);
            target.Tag = target;
        }

        var e = v.Compositor.CreateEntranceEasingFunction();
        var t = v.CreateVector3KeyFrameAnimation(nameof(Visual.Scale))
            .AddKeyFrame(0, new Vector3(from, from, 0))
            .AddKeyFrame(1, new Vector3(to, to, 0), e)
            .SetDuration(0.6);

        var o = CreateFade(v.Compositor, 1, 0, 200);
        var g = v.Compositor.CreateAnimationGroup(t, o);
        v.StartAnimationGroup(g);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    public static void PlayStandardEntrance(FrameworkElement element)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        element.GetElementVisual().StartAnimationGroup(
            CreateEntranceAnimation(element, new Vector3(100, 0, 0), 200));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="xamlDirect"></param>
    public static void PokeUIElementZIndex(UIElement e, XamlDirect? xamlDirect = null)
    {
        if (xamlDirect != null)
        {
            var o = xamlDirect.GetXamlDirectObject(e);
            var i = xamlDirect.GetInt32Property(o, XamlPropertyIndex.Canvas_ZIndex);
            xamlDirect.SetInt32Property(o, XamlPropertyIndex.Canvas_ZIndex, i + 1);
            xamlDirect.SetInt32Property(o, XamlPropertyIndex.Canvas_ZIndex, i);
        }
        else
        {
            var index = Canvas.GetZIndex(e);
            Canvas.SetZIndex(e, index + 1);
            Canvas.SetZIndex(e, index);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="size"></param>
    public static void SetCornerRadius(UIElement target, float size)
    {
        var vis = target.GetElementVisual();
        var rec = vis.Compositor.CreateRoundedRectangleGeometry();
        rec.CornerRadius = new(size);
        rec.LinkShapeSize(vis);
        var clip = vis.Compositor.CreateGeometricClip(rec);
        vis.Clip = clip;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="background"></param>
    /// <param name="children"></param>
    /// <param name="container"></param>
    public static void SetDropInOut(FrameworkElement background, IList<FrameworkElement> children, FrameworkElement? container = null)
    {
        if (background is null || children.Count == 0)
        {
            return;
        }

        if (UISettings.AnimationsEnabled is false)
        {
            background.SetShowAnimation(null);
            background.SetHideAnimation(null);
            foreach (var child in children)
            {
                child.SetShowAnimation(null);
                child.SetHideAnimation(null);
            }

            return;
        }

        var delay = 0.15;

        var bv = background.EnableTranslation(true).GetElementVisual();
        var ease = bv.Compositor.GetCachedEntranceEase();

        var bt = bv.Compositor.CreateVector3KeyFrameAnimation();
        bt.Target = TRANSLATION;
        bt.InsertExpressionKeyFrame(0, "Vector3(0, -this.Target.Size.Y, 0)");
        bt.InsertKeyFrame(1, Vector3.Zero, ease);
        bt.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
        bt.DelayTime = TimeSpan.FromSeconds(delay);
        bt.Duration = TimeSpan.FromSeconds(0.7);
        background.SetShowAnimation(bt);

        delay += 0.15;

        foreach (var child in children)
        {
            var v = child.EnableTranslation(true).GetElementVisual();
            var t = v.Compositor.CreateVector3KeyFrameAnimation();
            t.Target = TRANSLATION;
            t.InsertExpressionKeyFrame(0, "Vector3(0, -this.Target.Size.Y, 0)");
            t.InsertKeyFrame(1, Vector3.Zero, ease);
            t.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            t.DelayTime = TimeSpan.FromSeconds(delay);
            t.Duration = TimeSpan.FromSeconds(0.7);
            child.SetShowAnimation(t);
            delay += 0.075;
        }

        if (container != null)
        {
            var c = container.GetElementVisual();
            var clip = c.Compositor.CreateInsetClip();
            c.Clip = clip;
        }

        // Create hide animation
        var list = new List<FrameworkElement>
        {
            background
        };
        list.AddRange(children);

        var ht = bv.Compositor.CreateVector3KeyFrameAnimation();
        ht.Target = TRANSLATION;
        ht.InsertExpressionKeyFrame(1, "Vector3(0, -this.Target.Size.Y, 0)", ease);
        ht.Duration = TimeSpan.FromSeconds(0.5);

        foreach (var child in list)
        {
            child.SetHideAnimation(ht);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="durationMs"></param>
    private static void SetFadeInOut(FrameworkElement e, int durationMs)
    {
        var v = e.GetElementVisual();
        e.SetHideAnimation(CreateFade(v.Compositor, 0, null, durationMs));
        e.SetShowAnimation(CreateFade(v.Compositor, 1, null, durationMs));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="t"></param>
    private static void SetOpacityTransition(FrameworkElement e, TimeSpan t)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        if (t.TotalMilliseconds > 0)
        {
            var c = e.GetElementVisual().Compositor;
            var ani = c.CreateScalarKeyFrameAnimation();
            ani.Target = nameof(Visual.Opacity);
            ani.InsertExpressionKeyFrame(1, FINAL_VALUE, c.CreateLinearEasingFunction());
            ani.Duration = t;

            e.SetImplicitAnimation(nameof(Visual.Opacity), ani);
        }
        else
        {
            e.SetImplicitAnimation(nameof(Visual.Opacity), null);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public static void SetStandardEntrance(FrameworkElement sender, object args)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        if (sender is FrameworkElement e)
            e.SetShowAnimation(CreateEntranceAnimation(e, new Vector3(100, 0, 0), 200));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public static void SetStandardFadeInOut(object sender, RoutedEventArgs args)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        if (sender is FrameworkElement e)
        {
            SetFadeInOut(e, 200);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public static void SetStandardReposition(UIElement e)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        var v = e.GetElementVisual();
        var value = v.GetCached("DefaultOffsetAnimation", () =>
        {
            return v.CreateVector3KeyFrameAnimation(nameof(Visual.Offset))
                    .AddKeyFrame(0, STARTING_VALUE)
                    .AddKeyFrame(1, FINAL_VALUE)
                    .SetDuration(DefaultOffsetDuration);
        });

        v.SetImplicitAnimation(nameof(Visual.Offset), value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="depth"></param>
    /// <param name="recievers"></param>
    public static void SetThemeShadow(UIElement target, float depth, params UIElement[] recievers)
    {
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8) is false)
        {
            return;
        }

        // Temporarily, we'll also disable shadows if Windows Animations are disabled
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        try
        {
            if (!CompositionCapabilities.GetForCurrentView().AreEffectsFast())
            {
                return;
            }

            target.Translation = new Vector3(0, 0, depth);

            var shadow = new ThemeShadow();
            target.Shadow = shadow;
            foreach (var r in recievers)
            {
                shadow.Receivers.Add(r);
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public static void SetupOverlayPanelAnimation(UIElement e)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        var v = e.EnableTranslation(true).GetElementVisual();
        var g = v.GetCached("OPA", () =>
        {
            var t = v.CreateVector3KeyFrameAnimation(TRANSLATION)
                    .AddKeyFrame(1, 0, 200)
                    .SetDuration(0.375);

            var o = CreateFade(v.Compositor, 0, null, 200);
            return v.Compositor.CreateAnimationGroup(t, o);
        });

        e.SetHideAnimation(g);
        e.SetShowAnimation(CreateEntranceAnimation(e, new Vector3(0, 200, 0), 0, 550));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static ExpressionAnimation StartCentering(Visual v, float x = 0.5f, float y = 0.5f)
    {
        v.StopAnimation(nameof(Visual.CenterPoint));

        var e = v.GetCached($"CP{x}{y}", () =>
        {
            return v.CreateExpressionAnimation(nameof(Visual.CenterPoint))
                    .SetExpression(
                        string.Format(CENTRE_EXPRESSION,
                        x.ToString(CultureInfo.InvariantCulture.NumberFormat),
                        y.ToString(CultureInfo.InvariantCulture.NumberFormat)));
        });

        v.StartAnimationGroup(e);
        return e;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barElements"></param>
    /// <param name="contentElements"></param>
    public static void StartStartUpAnimation(List<FrameworkElement> barElements, List<UIElement> contentElements)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        var duration1 = TimeSpan.FromSeconds(0.7);

        var c = barElements[0].GetElementVisual().Compositor;
        var backOut = c.CreateCubicBezierEasingFunction(new Vector2(0.2f, 0.885f), new Vector2(0.25f, 1.125f));

        var delay = 0.1;
        foreach (var element in barElements)
        {
            var t = c.CreateVector3KeyFrameAnimation()
                .SetTarget(TRANSLATION)
                .AddKeyFrame(0, 0, -100)
                .AddKeyFrame(1, 0, backOut)
                .SetDelayBehavior(AnimationDelayBehavior.SetInitialValueBeforeDelay)
                .SetDelayTime(TimeSpan.FromSeconds(delay))
                .SetDuration(duration1);

            delay += 0.055;

            var v = element.EnableTranslation(true).GetElementVisual();
            v.StartAnimationGroup(t);
        }

        PlayEntrance(contentElements, 200);
    }

    /// <summary>
    /// Creates the detault Forward composition animation
    /// </summary>
    /// <param name="outElement"></param>
    /// <param name="inElement"></param>
    /// <returns></returns>
    public static void StartCompositionExpoZoomForwardTransition(FrameworkElement outElement, FrameworkElement inElement)
    {
        if (UISettings.AnimationsEnabled is false)
        {
            return;
        }

        var compositor = ElementCompositionPreview.GetElementVisual(outElement).Compositor;
        var outVisual = ElementCompositionPreview.GetElementVisual(outElement);
        var inVisual = ElementCompositionPreview.GetElementVisual(inElement);

        var outgroup = compositor.CreateAnimationGroup();
        var ingroup = compositor.CreateAnimationGroup();

        var outDuration = TimeSpan.FromSeconds(0.3);
        var inStart = TimeSpan.FromSeconds(0.25);
        var inDuration = TimeSpan.FromSeconds(0.6);

        var ease = compositor.GetCached("ExpoZoomEase",
            () => compositor.CreateCubicBezierEasingFunction(0.95f, 0.05f, 0.79f, 0.04f));

        var easeOut = compositor.GetCached("ExpoZoomOutEase",
            () => compositor.CreateCubicBezierEasingFunction(0.13f, 1.0f, 0.49f, 1.0f));

        // OUT ELEMENT
        {
            outVisual.CenterPoint = outVisual.Size.X > 0
               ? new Vector3(outVisual.Size / 2f, 0f)
               : new Vector3((float)Window.Current.Bounds.Width / 2f, (float)Window.Current.Bounds.Height / 2f, 0f);

            // SCALE OUT
            var sout = compositor.CreateVector3KeyFrameAnimation();
            sout.InsertKeyFrame(1, new Vector3(1.3f, 1.3f, 1f), ease);
            sout.Duration = outDuration;
            sout.Target = nameof(outVisual.Scale);

            // FADE OUT
            var oout = compositor.CreateScalarKeyFrameAnimation();
            oout.InsertKeyFrame(1, 0f, ease);
            oout.Duration = outDuration;
            oout.Target = nameof(outVisual.Opacity);
        }

        // IN ELEMENT
        {
            inVisual.CenterPoint = inVisual.Size.X > 0
                  ? new Vector3(inVisual.Size / 2f, 0f)
                  : new Vector3(outVisual.Size / 2f, 0f);

            // SCALE IN
            var sO = inVisual.Compositor.CreateVector3KeyFrameAnimation();
            sO.Duration = inDuration;
            sO.Target = nameof(inVisual.Scale);
            sO.InsertKeyFrame(0, new Vector3(0.7f, 0.7f, 1.0f), easeOut);
            sO.InsertKeyFrame(1, new Vector3(1.0f, 1.0f, 1.0f), easeOut);
            sO.DelayTime = inStart;
            ingroup.Add(sO);

            // FADE IN
            inVisual.Opacity = 0f;
            var op = inVisual.Compositor.CreateScalarKeyFrameAnimation();
            op.DelayTime = inStart;
            op.Duration = inDuration;
            op.Target = nameof(outVisual.Opacity);
            op.InsertKeyFrame(1, 0f, easeOut);
            op.InsertKeyFrame(1, 1f, easeOut);
            ingroup.Add(op);

        }

        outVisual.StartAnimationGroup(outgroup);
        inVisual.StartAnimationGroup(ingroup);
    }
}
