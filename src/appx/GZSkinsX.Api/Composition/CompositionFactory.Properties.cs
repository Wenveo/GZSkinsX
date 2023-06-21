// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Numerics;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

using Windows.Foundation;

namespace GZSkinsX.Api.Composition;

[Bindable]
public partial class CompositionFactory
{
    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.RegisterAttached("Center", typeof(Point),
            typeof(CompositionFactory), new PropertyMetadata(new Point(0, 0), OnCenterChanged));

    public static Point GetCenter(DependencyObject obj)
    => (Point)obj.GetValue(CenterProperty);

    public static void SetCenter(DependencyObject obj, Point value)
    => obj.SetValue(CenterProperty, value);

    private static void OnCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement u && e.NewValue is Point p)
        {
            StartCentering(u.GetElementVisual(), (float)p.X, (float)p.Y);
        }
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.RegisterAttached("CornerRadius", typeof(double),
            typeof(CompositionFactory), new PropertyMetadata(0d, OnCornerRadiusChanged));

    public static double GetCornerRadius(DependencyObject obj)
    => (double)obj.GetValue(CornerRadiusProperty);

    public static void SetCornerRadius(DependencyObject obj, double value)
    => obj.SetValue(CornerRadiusProperty, value);

    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && e.NewValue is double v)
        {
            SetCornerRadius(element, (float)v);
        }
    }

    public static readonly DependencyProperty EnableBounceScaleProperty =
        DependencyProperty.RegisterAttached("EnableBounceScale", typeof(bool),
            typeof(CompositionFactory), new PropertyMetadata(false, OnEnableBounceScaleChanged));

    public static bool GetEnableBounceScale(DependencyObject obj)
    => (bool)obj.GetValue(EnableBounceScaleProperty);

    public static void SetEnableBounceScale(DependencyObject obj, bool value)
    => obj.SetValue(EnableBounceScaleProperty, value);

    private static void OnEnableBounceScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement f)
        {
            var v = f.GetElementVisual();
            if (e.NewValue is bool b && b)
            {
                EnableStandardTranslation(v, 0.15);
            }
            else
            {
                v.Properties.SetImplicitAnimation(TRANSLATION, null);
            }
        }
    }

    public static readonly DependencyProperty OpacityDurationProperty =
        DependencyProperty.RegisterAttached("OpacityDuration", typeof(Duration),
            typeof(CompositionFactory), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0)), OnOpacityDurationChanged));

    public static Duration GetOpacityDuration(DependencyObject obj)
    => (Duration)obj.GetValue(OpacityDurationProperty);

    public static void SetOpacityDuration(DependencyObject obj, Duration value)
    => obj.SetValue(OpacityDurationProperty, value);

    private static void OnOpacityDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && e.NewValue is Duration t)
        {
            SetOpacityTransition(element, t.HasTimeSpan ? t.TimeSpan : TimeSpan.Zero);
        }
    }

    public static readonly DependencyProperty ScaleProperty =
        DependencyProperty.RegisterAttached("Scale", typeof(Point),
            typeof(CompositionFactory), new PropertyMetadata(new Point(1, 1), OnScaleChanged));

    public static Point GetScale(DependencyObject obj)
    => (Point)obj.GetValue(ScaleProperty);

    public static void SetScale(DependencyObject obj, Point value)
    => obj.SetValue(ScaleProperty, value);

    private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement u && e.NewValue is Point p)
        {
            u.GetElementVisual().Scale = new(p.ToVector2(), 1);
        }
    }

    public static readonly DependencyProperty TranslationProperty =
        DependencyProperty.RegisterAttached("Translation", typeof(Point),
            typeof(CompositionFactory), new PropertyMetadata(new Point(0, 0), OnTranslationChanged));

    public static Point GetTranslation(DependencyObject obj)
    => (Point)obj.GetValue(TranslationProperty);

    public static void SetTranslation(DependencyObject obj, Point value)
    => obj.SetValue(TranslationProperty, value);

    private static void OnTranslationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement u && e.NewValue is Point p)
        {
            u.EnableCompositionTranslation()
             .GetElementVisual()
             .SetTranslation(new(p.ToVector2(), 0));
        }
    }

    public static readonly DependencyProperty UseStandardRepositionProperty =
        DependencyProperty.RegisterAttached("UseStandardReposition", typeof(bool),
            typeof(CompositionFactory), new PropertyMetadata(false, OnUseStandardRepositionChanged));
    public static bool GetUseStandardReposition(DependencyObject obj)
    => (bool)obj.GetValue(UseStandardRepositionProperty);

    public static void SetUseStandardReposition(DependencyObject obj, bool value)
    => obj.SetValue(UseStandardRepositionProperty, value);

    private static void OnUseStandardRepositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement f && e.NewValue is bool b)
        {
            if (b)
            {
                SetStandardReposition(f);
            }
            else
            {
                DisableStandardReposition(f);
            }
        }
    }

    public static readonly DependencyProperty UseStandardFadeInOutProperty =
        DependencyProperty.RegisterAttached("UseStandardFadeInOut", typeof(bool),
            typeof(CompositionFactory), new PropertyMetadata(false, OnUseStandardFadeInOutChanged));
    public static bool GetUseStandardFadeInOut(DependencyObject obj)
    => (bool)obj.GetValue(UseStandardFadeInOutProperty);

    public static void SetUseStandardFadeInOut(DependencyObject obj, bool value)
    => obj.SetValue(UseStandardFadeInOutProperty, value);

    private static void OnUseStandardFadeInOutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement f && e.NewValue is bool b)
        {
            if (b)
            {
                SetStandardFadeInOut(f);
            }
            else
            {
                DisableStandardFadeInOut(f);
            }
        }
    }
}
