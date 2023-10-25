// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Navigation;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MainApp.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class MainPage : Page
{
    private static System.Lazy<INavigationViewManager> NavManager { get; } = new(() =>
    {
        if (AppxContext.DispatcherQueue.HasThreadAccess)
        {
            return AppxContext.NavigationViewManagerFactory.CreateNavigationViewManager(
                NavigationConstants.MAIN_NAV_GUID, new CustomizedNavView());
        }
        else
        {
            return AppxContext.DispatcherQueue.EnqueueAsync(() =>
            {
                return AppxContext.NavigationViewManagerFactory.CreateNavigationViewManager(
                    NavigationConstants.MAIN_NAV_GUID, new CustomizedNavView());
            }).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    });

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        contentPresenter.Content = NavManager.Value.UIObject;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        contentPresenter.Content = null;
    }
}
