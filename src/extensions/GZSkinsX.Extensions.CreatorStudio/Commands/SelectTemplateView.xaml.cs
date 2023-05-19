// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Extensions.CreatorStudio.Commands;

public sealed partial class SelectTemplateView : UserControl
{
    public Action? OnCancel { get; set; }

    public Action? OnNext { get; set; }

    public SelectTemplateView()
    {
        InitializeComponent();
    }

    private void TemplateView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        NextButton.IsEnabled = TemplateView.SelectedIndex != -1;
    }

    private void CancelButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        OnCancel?.Invoke();
    }

    private void NextButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        OnNext?.Invoke();
    }
}
