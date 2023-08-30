// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Game;
using GZSkinsX.Contracts.WindowManager;

namespace GZSkinsX.Views;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.StartUp_Guid, PageType = typeof(StartUpPage))]
internal sealed class StartUpFrame : IWindowFrame, IWindowFrame2
{
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return false;
    }

    public async Task<bool> CanNavigateToAsync(WindowFrameNavigatingEvnetArgs args)
    {
        var gameService = AppxContext.GameService;

        var rootFolder = await gameService.TryGetRootFolderAsync();
        if (await gameService.TryUpdateAsync(rootFolder, gameService.CurrentRegion))
        {
            AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);
            return false;
        }
        else
        {
            // IsInvalid
            args.Parameter = gameService.CurrentRegion is not GameRegion.Unknown;

            return true;
        }
    }
}
