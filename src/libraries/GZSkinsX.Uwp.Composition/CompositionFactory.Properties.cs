// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GZSkinsX.Uwp.Composition;

[Bindable]
public partial class CompositionFactory
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.RegisterAttached("Center", typeof(Point),
            typeof(CompositionFactory), new PropertyMetadata(new Point(0, 0), OnCenterChanged));

    public static Point GetCenter(DependencyObject obj)
    => (Point)obj.GetValue(CenterProperty);

    public static void SetCenter(DependencyObject obj, Point value)
    => obj.SetValue(CenterProperty, value);

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.RegisterAttached("CornerRadius", typeof(double),
            typeof(CompositionFactory), new PropertyMetadata(0d, OnCornerRadiusChanged));

    public static double GetCornerRadius(DependencyObject obj)
    => (double)obj.GetValue(CornerRadiusProperty);

    public static void SetCornerRadius(DependencyObject obj, double value)
    => obj.SetValue(CornerRadiusProperty, value);

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty EnableBounceScaleProperty =
        DependencyProperty.RegisterAttached("EnableBounceScale", typeof(bool),
            typeof(CompositionFactory), new PropertyMetadata(false, OnEnableBounceScaleChanged));

    public static bool GetEnableBounceScale(DependencyObject obj)
    => (bool)obj.GetValue(EnableBounceScaleProperty);

    public static void SetEnableBounceScale(DependencyObject obj, bool value)
    => obj.SetValue(EnableBounceScaleProperty, value);

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty OpacityDurationProperty =
        DependencyProperty.RegisterAttached("OpacityDuration", typeof(Duration),
            typeof(CompositionFactory), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0)), OnOpacityDurationChanged));

    public static Duration GetOpacityDuration(DependencyObject obj)
    => (Duration)obj.GetValue(OpacityDurationProperty);

    public static void SetOpacityDuration(DependencyObject obj, Duration value)
    => obj.SetValue(OpacityDurationProperty, value);

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty ScaleProperty =
        DependencyProperty.RegisterAttached("Scale", typeof(Point),
            typeof(CompositionFactory), new PropertyMetadata(new Point(1, 1), OnScaleChanged));

    public static Point GetScale(DependencyObject obj)
    => (Point)obj.GetValue(ScaleProperty);

    public static void SetScale(DependencyObject obj, Point value)
    => obj.SetValue(ScaleProperty, value);

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty TranslationProperty =
        DependencyProperty.RegisterAttached("Translation", typeof(Point),
            typeof(CompositionFactory), new PropertyMetadata(new Point(0, 0), OnTranslationChanged));

    public static Point GetTranslation(DependencyObject obj)
    => (Point)obj.GetValue(TranslationProperty);

    public static void SetTranslation(DependencyObject obj, Point value)
    => obj.SetValue(TranslationProperty, value);

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty UseStandardRepositionProperty =
        DependencyProperty.RegisterAttached("UseStandardReposition", typeof(bool),
            typeof(CompositionFactory), new PropertyMetadata(false, OnUseStandardRepositionChanged));
    public static bool GetUseStandardReposition(DependencyObject obj)
    => (bool)obj.GetValue(UseStandardRepositionProperty);

    public static void SetUseStandardReposition(DependencyObject obj, bool value)
    => obj.SetValue(UseStandardRepositionProperty, value);

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty UseStandardFadeInOutProperty =
        DependencyProperty.RegisterAttached("UseStandardFadeInOut", typeof(bool),
            typeof(CompositionFactory), new PropertyMetadata(false, OnUseStandardFadeInOutChanged));
    public static bool GetUseStandardFadeInOut(DependencyObject obj)
    => (bool)obj.GetValue(UseStandardFadeInOutProperty);

    public static void SetUseStandardFadeInOut(DependencyObject obj, bool value)
    => obj.SetValue(UseStandardFadeInOutProperty, value);
}
