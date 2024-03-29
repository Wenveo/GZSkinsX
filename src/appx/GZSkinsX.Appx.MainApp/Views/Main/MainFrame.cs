// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Xaml.Media.Animation;

namespace GZSkinsX.Appx.MainApp.Views;

[System.Composition.Shared, WindowFrameContract(Guid = WindowFrameConstants.Main_Guid, PageType = typeof(MainPage))]
internal sealed class MainFrame : IWindowFrame
{
    /// <inheritdoc/>
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        // Should disable animation ?
        if (args.Parameter is bool b && b)
        {
            args.NavigationTransitionInfo = new SuppressNavigationTransitionInfo();
        }

        return true;
    }
}
