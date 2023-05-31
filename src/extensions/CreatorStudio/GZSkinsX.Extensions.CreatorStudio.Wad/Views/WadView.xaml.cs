// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Extensions.CreatorStudio.Wad.Parser;
using GZSkinsX.Extensions.CreatorStudio.Wad.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Extensions.CreatorStudio.Wad.Views;

public sealed partial class WadView : UserControl
{
    internal WadViewModel ViewModel { get; }

    internal WadView(WadFile wadFile)
    {
        ViewModel = new(wadFile);
        InitializeComponent();
    }

    private void ListView_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
        {
            if (e.OriginalSource is FrameworkElement element &&
                element.DataContext is WadFolderNodeVM folderNodeVM)
            {
                ViewModel.CurrentViewNode = folderNodeVM;
            }
        }
    }
}
