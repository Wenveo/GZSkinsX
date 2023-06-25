// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Numerics;

using Windows.Foundation;
using Windows.UI.Xaml;

namespace GZSkinsX.Api.Composition;

public partial class CompositionFactory
{
    private static void OnCenterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement u && e.NewValue is Point p)
        {
            StartCentering(u.GetElementVisual(), (float)p.X, (float)p.Y);
        }
    }

    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && e.NewValue is double v)
        {
            SetCornerRadius(element, (float)v);
        }
    }

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

    private static void OnOpacityDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && e.NewValue is Duration t)
        {
            SetOpacityTransition(element, t.HasTimeSpan ? t.TimeSpan : TimeSpan.Zero);
        }
    }

    private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement u && e.NewValue is Point p)
        {
            u.GetElementVisual().Scale = new(p.ToVector2(), 1);
        }
    }

    private static void OnTranslationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement u && e.NewValue is Point p)
        {
            u.EnableCompositionTranslation()
             .GetElementVisual()
             .SetTranslation(new(p.ToVector2(), 0));
        }
    }

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
