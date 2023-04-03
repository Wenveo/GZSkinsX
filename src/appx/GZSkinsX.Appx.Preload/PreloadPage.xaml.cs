// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.WindowManager;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.Preload;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PreloadPage : Page
{
    public PreloadPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        var random = new Random();
        var waitingTime = random.Next(1000, 2000);
        Debug.WriteLine($"AppxPreload: Waiting Time = {waitingTime}");
        await Task.Delay(waitingTime);

        if (AppxContext.ServiceLocator.TryResolve<IWindowManagerService>(out var windowManagerService))
        {
            windowManagerService.NavigateTo(WindowFrameConstants.StartUp_Guid);
        }
    }
}
