// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using System;

using GZSkinsX.Api.Navigation;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GZSkinsX.Views.WindowFrames.NavigationRoot;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NavigationRootPage : Page
{
    private static readonly Lazy<INavigationViewManager> s_lazy_navigationViewManager;

    private static INavigationViewManager NavigationViewManager => s_lazy_navigationViewManager.Value;

    static NavigationRootPage()
    {
        s_lazy_navigationViewManager = new(() =>
        {
            return new CustomNavigationView().NavigationViewManager;
        });
    }

    public NavigationRootPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        contentPresenter.Content = NavigationViewManager.UIObject;
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        contentPresenter.Content = null;
        base.OnNavigatedFrom(e);
    }
}
