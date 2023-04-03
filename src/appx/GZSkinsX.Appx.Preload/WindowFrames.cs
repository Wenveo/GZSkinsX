// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Logging;
using GZSkinsX.Api.WindowManager;

using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace GZSkinsX.Appx.Preload;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.Preload_Guid, PageType = typeof(PreloadPage))]
internal sealed class ExportPreloadFrame : IWindowFrame
{
    private readonly ILoggingService _loggingService;
    private readonly PreloadSettings _preloadSettings;

    public ExportPreloadFrame()
    {
        var resolver = AppxContext.ServiceLocator;
        _loggingService = AppxContext.LoggingService;
        _preloadSettings = resolver.Resolve<PreloadSettings>();

        AppxContext.AppxTitleBar.ExtendViewIntoTitleBar = true;
        AppxContext.AppxTitleBarButton.ButtonBackgroundColor = Colors.Transparent;
        AppxContext.AppxTitleBarButton.ButtonInactiveBackgroundColor = Colors.Transparent;

        if (Debugger.IsAttached)
        {
            Application.Current.DebugSettings.EnableFrameRateCounter = true;
        }

        InitializeMainWindow();
    }

    /// <inheritdoc/>
    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return true;
    }

    private void InitializeMainWindow()
    {
        var minWindowSize = SizeHelper.FromDimensions(720, 480);
        var appView = ApplicationView.GetForCurrentView();
        appView.SetPreferredMinSize(minWindowSize);

        if (_preloadSettings.IsInitialize is false)
        {
            // Set Default Language
            var cultureId = GetUserDefaultUILanguage();
            var cultureInfo = CultureInfo.GetCultureInfo(cultureId);
            ApplicationLanguages.PrimaryLanguageOverride = cultureInfo.Name;

            if (appView.TryResizeView(minWindowSize))
            {
                _loggingService.LogWarning("AppxPreload: Failed to resize the window.");
            }
            _preloadSettings.IsInitialize = true;
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

        _loggingService.LogDebug($"AppxPreload: IsInitialize = {_preloadSettings.IsInitialize}");
    }

    [ContractVersion(typeof(UniversalApiContract), 65536u)]
    [DllImport("api-ms-win-core-localization-obsolete-l1-2-0.dll", CharSet = CharSet.Auto)]
    private static extern ushort GetUserDefaultUILanguage();
}

