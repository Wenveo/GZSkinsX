// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Logging;
using GZSkinsX.Api.WindowManager;

namespace GZSkinsX.Views.WindowFrames.Preload;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.Preload_Guid, PageType = typeof(PreloadPage))]
internal sealed partial class PreloadFrame : IWindowFrame
{
    private readonly ILoggingService _loggingService;
    private readonly PreloadSettings _preloadSettings;

    public PreloadFrame()
    {
        _loggingService = AppxContext.LoggingService;
        _preloadSettings = AppxContext.Resolve<PreloadSettings>();

        InitializeMainWindow();
    }

    /// <inheritdoc/>
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return true;
    }

    private void InitializeMainWindow()
    {
        AppxContext.AppxWindow.MainWindow.AppWindow.Resize(new(1000, 500));

        if (_preloadSettings.IsInitialize is false)
        {
            _preloadSettings.IsInitialize = true;
        }

        _loggingService.LogDebug($"AppxPreload: IsInitialize = {_preloadSettings.IsInitialize}");
    }
}

