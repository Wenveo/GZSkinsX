// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Game;
using GZSkinsX.Api.WindowManager;

namespace GZSkinsX.Views.WindowFrames.StartUp;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.StartUp_Guid, PageType = typeof(StartUpPage))]
internal sealed class StartUpFrame : IWindowFrame, IWindowFrame2
{
    private readonly IWindowManagerService _windowManagerService;
    private readonly IGameService _gameService;

    public StartUpFrame()
    {
        _windowManagerService = AppxContext.WindowManagerService;
        _gameService = AppxContext.GameService;
    }

    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return false;
    }

    public async Task<bool> CanNavigateToAsync(WindowFrameNavigatingEvnetArgs args)
    {
        if (await _gameService.TryUpdateAsync(_gameService.RootFolder, _gameService.CurrentRegion))
        {
            _windowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);
            return false;
        }
        else
        {
            // IsInvalid
            args.Parameter = _gameService.CurrentRegion is not GameRegion.Unknown;

            return true;
        }
    }
}

