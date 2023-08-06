// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Helpers;
using GZSkinsX.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class StartUpPage : Page
{
    public StartUpViewModel ViewModel { get; } = new();

    public StartUpPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var isInvalid = e.Parameter is bool b && b;

        // 设置标题
        var localizedTitle = ResourceHelper.GetLocalized(isInvalid
            ? "Resources/StartUp_Initialize_Invalid_Title"
            : "Resources/StartUp_Initialize_Default_Title");
        StartUp_Initialize_Title.Text = localizedTitle;
    }
}
