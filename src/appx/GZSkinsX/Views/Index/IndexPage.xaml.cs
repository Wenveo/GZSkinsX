// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO.Hashing;
using System.Runtime.InteropServices.WindowsRuntime;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.WindowManager;
using GZSkinsX.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Windows.Storage;


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
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                var destFile = await (await ApplicationData.Current.RoamingFolder
                    .CreateFolderAsync("Kernel", CreationCollisionOption.OpenIfExists))
                    .CreateFileAsync("GZSkinsX.Kernel.dll", CreationCollisionOption.OpenIfExists);

                var buffer = await FileIO.ReadBufferAsync(destFile);
                var checksum = XxHash64.HashToUInt64(buffer.ToArray());
                if (checksum == 1812918375494270614)
                {
                    AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.StartUp_Guid);
                    return;
                }
            }
            catch
            {
            }

            ContentGrid.Visibility = Visibility.Visible;
            ViewModel.StartDownload();
        });
    }
}
