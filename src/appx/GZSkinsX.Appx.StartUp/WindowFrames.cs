// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api.Game;
using GZSkinsX.Api.Scripting;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.DotNet.Diagnostics;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Appx.StartUp;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.StartUp_Guid, PageType = typeof(StartUpPage))]
internal sealed class ExportStartUpFrame : IWindowFrame
{
    private readonly IServiceLocator _serviceLocator;
    private readonly IGameService _gameService;
    private readonly IWindowManagerService _windowManagerService;

    private bool _isInvalid;

    [ImportingConstructor]
    public ExportStartUpFrame(IServiceLocator serviceLocator)
    {
        _serviceLocator = serviceLocator;
        _gameService = serviceLocator.Resolve<IGameService>();
        _windowManagerService = serviceLocator.Resolve<IWindowManagerService>();
    }

    /// <inheritdoc/>
    public async Task OnInitializeAsync(Page viewElement)
    {
        Debug2.Assert(viewElement is StartUpPage);
        if (viewElement is StartUpPage startUpPage)
        {
            startUpPage.InitializeContext(_serviceLocator, _isInvalid);
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task OnNavigatingAsync(WindowFrameNavigateEventArgs args)
    {
        if (_gameService.TryUpdate(_gameService.RootDirectory, _gameService.CurrentRegion))
        {
            _windowManagerService.NavigateTo(WindowFrameConstants.Main_Guid);
            args.Handled = true;
        }
        else
        {
            _isInvalid = _gameService.CurrentRegion != GameRegion.Unknown ||
                string.IsNullOrEmpty(_gameService.RootDirectory) is false;
        }

        await Task.CompletedTask;
    }
}

