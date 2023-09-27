// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;
using GZSkinsX.ViewModels;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class IndexPage : Page
{
    public IndexViewModel ViewModel { get; } = new();

    public IndexPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await DispatcherQueue.EnqueueAsync(async () =>
        {
            try
            {
                if (AppxContext.KernelService.VerifyModuleIntegrity())
                {
                    AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.StartUp_Guid);
                    Task.Run(AppxContext.KernelService.UpdateManifestAsync).FireAndForget();
                    return;
                }
            }
            catch
            {
            }

            ContentGrid.Visibility = Visibility.Visible;
            await ViewModel.TryDownloadAsync();
        }, DispatcherQueuePriority.Normal);
    }
}
