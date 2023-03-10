// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api;
using GZSkinsX.Api.Game;
using GZSkinsX.Api.Shell;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Appx.StartUp;

/// <summary>
/// 
/// </summary>
[Shared, ExportViewElement]
[ViewElementMetadata(Guid = ViewElementConstants.StartUp_Guid, PageType = typeof(StartUpPage))]
internal sealed class ExportStartUpPage : IViewElement
{
    /// <summary>
    /// 
    /// </summary>
    private readonly IViewManagerService _viewManagerService;

    /// <summary>
    /// 
    /// </summary>
    private readonly IGameService _gameService;

    /// <summary>
    /// 
    /// </summary>
    [ImportingConstructor]
    public ExportStartUpPage(IViewManagerService viewManagerService, IGameService gameService)
    {
        _viewManagerService = viewManagerService;
        _gameService = gameService;
    }

    /// <inheritdoc/>
    public async Task OnInitializeAsync(Page viewElement)
    {
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task OnNavigatingAsync(WindowFrameNavigateEventArgs args)
    {
        if (_gameService.TryUpdate(_gameService.RootDirectory, _gameService.CurrentRegion))
        {
            _viewManagerService.NavigateTo(ViewElementConstants.Main_Guid);
            args.Handled = true;
        }

        await Task.CompletedTask;
    }
}

