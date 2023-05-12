// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Navigation;

using GZSkinsX.Appx.NavigationRoot.Controls;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.NavigationRoot;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NavigationRootPage : Page
{
    private static readonly INavigationViewManager s_navigationViewManager;

    static NavigationRootPage()
    {
        var serviceLocator = AppxContext.ServiceLocator;
        var navigationViewFactory = serviceLocator.Resolve<INavigationViewFactory>();
        s_navigationViewManager = navigationViewFactory.CreateNavigationViewManager(
            NavigationConstants.NAVIGATIONROOT_NV_GUID,
            new CustomNavigationView());
    }

    public NavigationRootPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        contentPresenter.Content = s_navigationViewManager.UIObject;
        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        contentPresenter.Content = null;
        base.OnNavigatedFrom(e);
    }
}
