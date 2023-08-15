// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Game;

using GZSkinsX.Contracts.Logging;
using GZSkinsX.Contracts.Settings;
using GZSkinsX.Contracts.WindowManager;

using Windows.Foundation.Metadata;

using Windows.UI.ViewManagement;

using Windows.UI.Xaml;

namespace GZSkinsX.Views;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.Index_Guid, PageType = typeof(IndexPage))]
internal sealed class IndexFrame : IWindowFrame
{
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return true;
    }
}


[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.Main_Guid, PageType = typeof(MainPage))]
internal sealed class MainFrame : IWindowFrame
{
    /// <inheritdoc/>
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return true;
    }
}

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.Preload_Guid, PageType = typeof(PreloadPage))]
internal sealed class PreloadFrame : IWindowFrame
{
    /// <inheritdoc/>
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return true;
    }
}


[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.StartUp_Guid, PageType = typeof(StartUpPage))]
internal sealed class StartUpFrame : IWindowFrame, IWindowFrame2
{
    private readonly IWindowManagerService _windowManagerService;
    private readonly StartUpSettings _startUpSettings;
    private readonly ILoggingService _loggingService;
    private readonly IGameService _gameService;
    private readonly IAppxWindow _appxWindow;

    public StartUpFrame()
    {
        _appxWindow = AppxContext.AppxWindow;
        _gameService = AppxContext.GameService;
        _loggingService = AppxContext.LoggingService;
        _startUpSettings = AppxContext.Resolve<StartUpSettings>();
        _windowManagerService = AppxContext.WindowManagerService;

        if (Debugger.IsAttached)
        {
            Application.Current.DebugSettings.EnableFrameRateCounter = true;
        }

        InitializeMainWindow();
    }

    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return false;
    }

    public async Task<bool> CanNavigateToAsync(WindowFrameNavigatingEvnetArgs args)
    {
        if (await _gameService.TryGetRootFolderAsync() is not null)
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

    private void InitializeMainWindow()
    {
        var minWindowSize = SizeHelper.FromDimensions(1000, 500);
        var appView = ApplicationView.GetForCurrentView();
        appView.SetPreferredMinSize(minWindowSize);

        if (_startUpSettings.IsInitialize is false)
        {
            if (!appView.TryResizeView(minWindowSize))
            {
                _loggingService.LogWarning("AppxStartUp: Failed to resize the window.");
            }

            _appxWindow.Closed += (_, _) => _startUpSettings.IsInitialize = true;
        }
        else
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
        }

        if (!ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")) // PC Family
        {
            // Disable the system view activation policy during the first launch of the app
            // only for PC family devices and not for phone family devices
            try
            {
                ApplicationViewSwitcher.DisableSystemViewActivationPolicy();
            }
            catch (Exception)
            {
                // Log that DisableSystemViewActionPolicy didn't work
            }
        }

        _loggingService.LogDebug($"AppxStartUp: IsInitialize = {_startUpSettings.IsInitialize}");
    }

    [Shared, Export]
    internal sealed class StartUpSettings
    {
        private const string THE_GUID = "09A5FCC5-4B0C-4476-8401-59EEBFB19213";
        private const string ISINITIALIZE_NAME = "IsInitialize";

        private readonly ISettingsSection _settingsSection;

        public bool IsInitialize
        {
            get => _settingsSection.Attribute<bool>(ISINITIALIZE_NAME);
            set => _settingsSection.Attribute(ISINITIALIZE_NAME, value);
        }

        public StartUpSettings()
        {
            _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
        }
    }
}
