// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml;

using Windows.UI.Xaml.Markup;

namespace GZSkinsX.Contracts.Controls;

[ContentProperty(Name = nameof(Content))]
public class MultiStateToggleButtonState : DependencyObject
{
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(UIElement),
            typeof(MultiStateToggleButtonState), new PropertyMetadata(null, OnPropertyChangedCallback));

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register(nameof(IsChecked), typeof(UIElement),
            typeof(MultiStateToggleButtonState), new PropertyMetadata(false, OnPropertyChangedCallback));

    public static readonly DependencyProperty StateNameProperty =
        DependencyProperty.Register(nameof(StateName), typeof(UIElement),
            typeof(MultiStateToggleButtonState), new PropertyMetadata(string.Empty, OnPropertyChangedCallback));

    public UIElement Content
    {
        get => (UIElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string StateName
    {
        get => (string)GetValue(StateNameProperty);
        set => SetValue(StateNameProperty, value);
    }

    private MultiStateToggleButton? _parent;

    public MultiStateToggleButton? Parent
    {
        get => _parent;
        internal set
        {
            _parent = value;
            OnParentChanged(value);
        }
    }

    protected virtual void OnParentChanged(MultiStateToggleButton? newParent)
    {

    }

    private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MultiStateToggleButtonState state)
        {
            state.OnPropertyChanged(e);
        }
    }

    private void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Parent?.UpdateContentAndVisualStates();
    }
}
