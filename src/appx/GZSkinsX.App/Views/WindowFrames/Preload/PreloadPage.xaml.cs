// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Linq;
using System.Threading.Tasks;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.MainApp;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Windows.ApplicationModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Views.WindowFrames.Preload;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PreloadPage : Page
{
    private sealed class UniversalExtensionResources : ResourceDictionary
    {
        public UniversalExtensionResources()
        {
            foreach (var rsrc in StartUpClass.ExtensionService.GetMergedResourceDictionaries())
            {
                MergedDictionaries.Add(rsrc);
            }
        }
    }

    public PreloadPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            await Task.Delay(200);

            var b = await Package.Current.VerifyContentIntegrityAsync();
            if (b)
            {
                if (Application.Current.Resources.MergedDictionaries.FirstOrDefault(
                    rsrc => rsrc.GetType() == typeof(UniversalExtensionResources)) is null)
                {
                    Application.Current.Resources.MergedDictionaries.Add(new UniversalExtensionResources());
                }

                AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.StartUp_Guid);
            }
            else
            {
                ShowCrashMessage(ResourceHelper.GetLocalized("Resources/Preload_Crash_Failed_To_Verify_Content_Integrity"));
                AppxContext.LoggingService.LogError($"AppxPreload: Failed to verify content integrity.");
            }
        });

        base.OnNavigatedTo(e);
    }

    private void ShowCrashMessage(string message)
    {
        DefaultContent.Visibility = Visibility.Collapsed;
        CrashContent.Visibility = Visibility.Visible;

        CrashTextHost.Text = message;
    }
}
