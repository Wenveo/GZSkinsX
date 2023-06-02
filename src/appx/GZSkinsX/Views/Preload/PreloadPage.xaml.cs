// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.MRT;
using GZSkinsX.Api.WindowManager;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Views.Preload;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PreloadPage : Page
{
    private readonly IMRTCoreMap _mrtCoreMap;

    public PreloadPage()
    {
        var mainResourceMap = AppxContext.MRTCoreService.MainResourceMap;
        _mrtCoreMap = mainResourceMap.GetSubtree("GZSkinsX/Strings");

        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var b = await Package.Current.VerifyContentIntegrityAsync();
        if (b)
        {
            if (AppxContext.TryResolve<IWindowManagerService>(out var windowManagerService))
            {
                windowManagerService.NavigateTo(WindowFrameConstants.StartUp_Guid);
            }
            else
            {
                // 不应执行至此，但保留一段代码以显示错误
                var guid = typeof(IWindowManagerService).GUID;

                ShowCrashMessage(string.Format(_mrtCoreMap.GetString("Crash_Failed_To_Find_Component"), guid));
                AppxContext.LoggingService.LogError($"AppxPreload: Failed to find component IID: {{{guid}}}.");
            }
        }
        else
        {
            ShowCrashMessage(_mrtCoreMap.GetString("Crash_Failed_To_Check_Access"));
            AppxContext.LoggingService.LogError($"AppxPreload: Failed to check access.");
        }
    }

    private void ShowCrashMessage(string message)
    {
        DefaultContent.Visibility = Visibility.Collapsed;
        CrashContent.Visibility = Visibility.Visible;

        CrashTextHost.Text = message;
    }
}
