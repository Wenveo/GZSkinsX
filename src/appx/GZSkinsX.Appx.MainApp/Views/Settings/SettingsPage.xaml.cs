// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Navigation;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MainApp.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class SettingsPage : Page
{
    public INavigationViewManager NavigationViewManager { get; }

    public SettingsPage()
    {
        InitializeComponent();

        NavigationViewManager = AppxContext.NavigationViewManagerFactory.CreateNavigationViewManager(
            NavigationConstants.SETTINGS_NAV_GUID, new() { Target = SettingsNavigationView });
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (DispatcherQueue.TryEnqueue(OnNavigatedToCore) is false)
        {
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs _)
        {
            Loaded -= OnLoaded;
            OnNavigatedToCore();
        }

        void OnNavigatedToCore()
        {
            var navManager = NavigationViewManager;
            if (e.Parameter is NavigationViewNavigateArgs args && navManager.CanNavigate(args))
            {
                navManager.NavigateTo(args);
            }
            else
            {
                navManager.NavigateToFirstItem();
            }
        }
    }

    private void SettingsNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Main_Guid, true);
    }
}
