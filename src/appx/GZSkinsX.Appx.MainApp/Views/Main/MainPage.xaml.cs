// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Navigation;

using Microsoft.UI.Xaml;
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
                NavigationConstants.MAIN_NAV_GUID, new() { Target = new CustomizedNavView() });
        }
        else
        {
            return AppxContext.DispatcherQueue.EnqueueAsync(() =>
            {
                return AppxContext.NavigationViewManagerFactory.CreateNavigationViewManager(
                    NavigationConstants.MAIN_NAV_GUID, new() { Target = new CustomizedNavView() });
            }).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    });

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (NavManager.IsValueCreated is false)
        {
            var navManager = NavManager.Value;
            var navView = navManager.UIObject as CustomizedNavView;
            Debug.Assert(navView is not null);

            navView.Loaded += OnNavVewLoaded;
            contentPresenter.Content = navView;
        }
        else
        {
            contentPresenter.Content = NavManager.Value.UIObject;
        }

        static void OnNavVewLoaded(object sender, RoutedEventArgs e)
        {
            var navView = sender as CustomizedNavView;
            Debug.Assert(navView is not null);

            navView.Loaded -= OnNavVewLoaded;
            navView.InitializeAsync(NavManager.Value).FireAndForget();
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        contentPresenter.Content = null;
    }
}
