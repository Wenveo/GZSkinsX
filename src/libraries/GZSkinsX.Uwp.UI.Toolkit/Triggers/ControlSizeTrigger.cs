// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml;

namespace GZSkinsX.Uwp.UI.Toolkit;

/// <summary>
/// Provide an abstraction around the Toolkit ControlSizeTrigger for both UWP and WinUI 3 in the same namespace (until 8.0).
/// </summary>
public class ControlSizeTrigger : StateTriggerBase
{
    /// <summary>
    /// Gets or sets a value indicating
    /// whether this trigger will be active or not.
    /// </summary>
    public bool CanTrigger
    {
        get => (bool)GetValue(CanTriggerProperty);
        set => SetValue(CanTriggerProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CanTrigger"/> DependencyProperty.
    /// </summary>
    public static readonly DependencyProperty CanTriggerProperty =
        DependencyProperty.Register(nameof(CanTrigger), typeof(bool), typeof(ControlSizeTrigger),
            new PropertyMetadata(true, (d, e) => ((ControlSizeTrigger)d).UpdateTrigger()));

    /// <summary>
    /// Gets or sets the max width at which to trigger.
    /// This value is exclusive, meaning this trigger
    /// could be active if the value is less than MaxWidth.
    /// </summary>
    public double MaxWidth
    {
        get => (double)GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MaxWidth"/> DependencyProperty.
    /// </summary>
    public static readonly DependencyProperty MaxWidthProperty =
        DependencyProperty.Register(nameof(MaxWidth), typeof(double), typeof(ControlSizeTrigger),
            new PropertyMetadata(double.PositiveInfinity, (d, e) => ((ControlSizeTrigger)d).UpdateTrigger()));

    /// <summary>
    /// Gets or sets the min width at which to trigger.
    /// This value is inclusive, meaning this trigger
    /// could be active if the value is >= MinWidth.
    /// </summary>
    public double MinWidth
    {
        get => (double)GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MinWidth"/> DependencyProperty.
    /// </summary>
    public static readonly DependencyProperty MinWidthProperty =
        DependencyProperty.Register(nameof(MinWidth), typeof(double), typeof(ControlSizeTrigger),
            new PropertyMetadata(0.0, (d, e) => ((ControlSizeTrigger)d).UpdateTrigger()));

    /// <summary>
    /// Gets or sets the max height at which to trigger.
    /// This value is exclusive, meaning this trigger
    /// could be active if the value is less than MaxHeight.
    /// </summary>
    public double MaxHeight
    {
        get => (double)GetValue(MaxHeightProperty);
        set => SetValue(MaxHeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MaxHeight"/> DependencyProperty.
    /// </summary>
    public static readonly DependencyProperty MaxHeightProperty =
        DependencyProperty.Register(nameof(MaxHeight), typeof(double), typeof(ControlSizeTrigger),
            new PropertyMetadata(double.PositiveInfinity, (d, e) => ((ControlSizeTrigger)d).UpdateTrigger()));

    /// <summary>
    /// Gets or sets the min height at which to trigger.
    /// This value is inclusive, meaning this trigger
    /// could be active if the value is >= MinHeight.
    /// </summary>
    public double MinHeight
    {
        get => (double)GetValue(MinHeightProperty);
        set => SetValue(MinHeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MinHeight"/> DependencyProperty.
    /// </summary>
    public static readonly DependencyProperty MinHeightProperty =
        DependencyProperty.Register(nameof(MinHeight), typeof(double), typeof(ControlSizeTrigger),
            new PropertyMetadata(0.0, (d, e) => ((ControlSizeTrigger)d).UpdateTrigger()));

    /// <summary>
    /// Gets or sets the element whose width will observed
    /// for the trigger.
    /// </summary>
    public FrameworkElement TargetElement
    {
        get => (FrameworkElement)GetValue(TargetElementProperty);
        set => SetValue(TargetElementProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TargetElement"/> DependencyProperty.
    /// </summary>
    /// <remarks>
    /// Using a DependencyProperty as the backing store for TargetElement. This enables animation, styling, binding, etc.
    /// </remarks>
    public static readonly DependencyProperty TargetElementProperty =
        DependencyProperty.Register(nameof(TargetElement), typeof(FrameworkElement),
            typeof(ControlSizeTrigger), new PropertyMetadata(null, OnTargetElementPropertyChanged));

    /// <summary>
    /// Gets a value indicating whether the trigger is active.
    /// </summary>
    public bool IsActive { get; private set; }

    private static void OnTargetElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ControlSizeTrigger)d).UpdateTargetElement((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
    }

    // Handle event to get current values
    private void OnTargetElementSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateTrigger();
    }

    private void UpdateTargetElement(FrameworkElement oldValue, FrameworkElement newValue)
    {
        if (oldValue != null)
        {
            oldValue.SizeChanged -= OnTargetElementSizeChanged;
        }

        if (newValue != null)
        {
            newValue.SizeChanged += OnTargetElementSizeChanged;
        }

        UpdateTrigger();
    }

    // Logic to evaluate and apply trigger value
    private void UpdateTrigger()
    {
        if (TargetElement == null || !CanTrigger)
        {
            SetActive(false);
            return;
        }

        var activate = MinWidth <= TargetElement.ActualWidth &&
                        TargetElement.ActualWidth < MaxWidth &&
                        MinHeight <= TargetElement.ActualHeight &&
                        TargetElement.ActualHeight < MaxHeight;

        IsActive = activate;
        SetActive(activate);
    }
}