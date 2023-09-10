// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;
using GZSkinsX.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Windows.System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; } = new();

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        DispatcherQueue.EnqueueAsync(ViewModel.InitializeAsync).FireAndForget();
        AppxContext.AppxTitleBar.SetTitleBar(FakeTitleBar);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        AppxContext.AppxTitleBar.SetTitleBar(null);
    }

    private void GoBack_Click(object sender, RoutedEventArgs e)
    {
        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Main_Guid);
    }
}
