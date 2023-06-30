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

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Game;
using GZSkinsX.Api.Logging;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.Views.WindowFrames.Preload;

using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace GZSkinsX.Views.WindowFrames.StartUp;

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

    private void InitializeMainWindow()
    {
        var minWindowSize = SizeHelper.FromDimensions(1000, 500);
        var appView = ApplicationView.GetForCurrentView();
        appView.SetPreferredMinSize(minWindowSize);

        if (_startUpSettings.IsInitialize is false)
        {
            // Set Default Language
            /*var cultureId = GetUserDefaultUILanguage();
            var cultureInfo = CultureInfo.GetCultureInfo(cultureId);
            ApplicationLanguages.PrimaryLanguageOverride = cultureInfo.Name;*/

            if (appView.TryResizeView(minWindowSize) is false)
            {
                _loggingService.LogWarning("AppxPreload: Failed to resize the window.");
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

        _loggingService.LogDebug($"AppxPreload: IsInitialize = {_startUpSettings.IsInitialize}");
    }

    private void _appxWindow_Closed(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    //[ContractVersion(typeof(UniversalApiContract), 65536u)]
    //[DllImport("api-ms-win-core-localization-obsolete-l1-2-0.dll", CharSet = CharSet.Auto)]
    //private static extern ushort GetUserDefaultUILanguage();
}

