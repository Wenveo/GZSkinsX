// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Extensions.CreatorStudio.Views.NewDocument;

public sealed partial class NewFileView : UserControl
{
    private readonly ContentDialog _contentDialog;

    public NewFileView(ContentDialog contentDialog)
    {
        _contentDialog = contentDialog;
        InitializeComponent();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        _contentDialog.Hide();
    }

    private void OnOkay(object sender, RoutedEventArgs e)
    {
        _contentDialog.Hide();
    }
}
