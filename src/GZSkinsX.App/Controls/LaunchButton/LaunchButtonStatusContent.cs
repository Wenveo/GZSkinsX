// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml;

namespace GZSkinsX.Controls;

internal sealed class LaunchButtonStatusContent : DependencyObject
{
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(UIElement),
            typeof(LaunchButtonStatusContent), new PropertyMetadata(null));

    public static readonly DependencyProperty RunningContentProperty =
        DependencyProperty.Register(nameof(RunningContent), typeof(UIElement),
            typeof(LaunchButtonStatusContent), new PropertyMetadata(null));

    public static readonly DependencyProperty UpdatingContentProperty =
        DependencyProperty.Register(nameof(UpdatingContent), typeof(UIElement),
            typeof(LaunchButtonStatusContent), new PropertyMetadata(null));

    public static readonly DependencyProperty CheckUpdatesContentProperty =
        DependencyProperty.Register(nameof(CheckUpdatesContent), typeof(UIElement),
            typeof(LaunchButtonStatusContent), new PropertyMetadata(null));

    public static readonly DependencyProperty UpdateFailedContentProperty =
        DependencyProperty.Register(nameof(UpdateFailedContent), typeof(UIElement),
            typeof(LaunchButtonStatusContent), new PropertyMetadata(null));

    public UIElement DefaultContent
    {
        get => (UIElement)GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    public UIElement RunningContent
    {
        get => (UIElement)GetValue(RunningContentProperty);
        set => SetValue(RunningContentProperty, value);
    }

    public UIElement CheckUpdatesContent
    {
        get => (UIElement)GetValue(CheckUpdatesContentProperty);
        set => SetValue(CheckUpdatesContentProperty, value);
    }

    public UIElement UpdatingContent
    {
        get => (UIElement)GetValue(UpdatingContentProperty);
        set => SetValue(UpdatingContentProperty, value);
    }

    public UIElement UpdateFailedContent
    {
        get => (UIElement)GetValue(UpdateFailedContentProperty);
        set => SetValue(UpdateFailedContentProperty, value);
    }

    internal UIElement GetUIContent(LaunchButtonStatus state) => state switch
    {
        LaunchButtonStatus.Running => RunningContent,
        LaunchButtonStatus.CheckUpdates => CheckUpdatesContent,
        LaunchButtonStatus.UpdateFailed => UpdateFailedContent,
        LaunchButtonStatus.Updating => UpdatingContent,
        _ => DefaultContent
    };
}
