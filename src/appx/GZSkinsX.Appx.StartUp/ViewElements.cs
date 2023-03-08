// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api;
using GZSkinsX.Api.Game;
using GZSkinsX.Api.Shell;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Appx.StartUp;

[Shared, ExportViewElement]
[ViewElementMetadata(Guid = THE_GUID, PageType = typeof(LoadingPage))]
internal sealed class ExportLoadingPage : IViewElement
{
    public const string THE_GUID = "96BAB9A3-08CE-4A16-95E6-25403A49C378";
}

[Shared, ExportViewElement]
[ViewElementMetadata(Guid = ViewElementConstants.StartUpPage_Guid, PageType = typeof(StartUpPage))]
internal sealed class ExportStartUpPage : IViewElementLoader
{
    private readonly IViewManagerService _viewManagerService;
    private readonly IGameService _gameService;

    [ImportingConstructor]
    public ExportStartUpPage(IViewManagerService viewManagerService, IGameService gameService)
    {
        _viewManagerService = viewManagerService;
        _gameService = gameService;
    }

    public void OnNavigating(WindowFrameNavigateEventArgs args)
    {
        _viewManagerService.NavigateTo(ExportLoadingPage.THE_GUID);
        if (_gameService.TryUpdate(_gameService.RootDirectory, _gameService.CurrentRegion))
        {
            _viewManagerService.NavigateTo(ViewElementConstants.MainPage_Guid);
            return;
        }

        args.Handled = true;
    }

    public void OnInitialize(Page viewElement)
    {
    }
}

