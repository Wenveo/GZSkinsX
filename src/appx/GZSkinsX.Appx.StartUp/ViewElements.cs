// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Diagnostics;
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
[ViewElementMetadata(Guid = THE_GUID, PageType = typeof(LoadingPage))]
internal sealed class ExportLoadingPage : IViewElement
{
    /// <summary>
    /// 
    /// </summary>
    public const string THE_GUID = "96BAB9A3-08CE-4A16-95E6-25403A49C378";

    /// <inheritdoc/>
    public async Task OnInitializeAsync(Page viewElement)
    {
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task OnNavigatingAsync(WindowFrameNavigateEventArgs args)
    {
        await Task.CompletedTask;
    }
}

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
        _viewManagerService.NavigateTo(ExportLoadingPage.THE_GUID);
        if (_gameService.TryUpdate(_gameService.RootDirectory, _gameService.CurrentRegion))
        {
            _viewManagerService.NavigateTo(ViewElementConstants.Main_Guid);
            args.Handled = true;
            return;
        }

        var random = new Random();
        var waitingTime = random.Next(1000, 2000);
        Debug.WriteLine($"AppxStartUp: Waiting Time = {waitingTime}");
        await Task.Delay(waitingTime);
    }
}

