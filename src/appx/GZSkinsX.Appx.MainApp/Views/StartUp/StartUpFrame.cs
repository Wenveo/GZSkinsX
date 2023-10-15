// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Game;
using GZSkinsX.Contracts.WindowManager;

namespace GZSkinsX.Appx.MainApp.Views;

[System.Composition.Shared, WindowFrameContract(Guid = WindowFrameConstants.StartUp_Guid, PageType = typeof(StartUpPage))]
internal sealed class StartUpFrame : IWindowFrame
{
    /// <inheritdoc/>
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        if (AppxContext.GameService.EnsureGameDataIsValid())
        {
            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);
            return false;
        }
        else
        {
            // IsInvalid
            args.Parameter = AppxContext.GameService.CurrentRegion is not GameRegion.Unknown;
            return true;
        }
    }
}
