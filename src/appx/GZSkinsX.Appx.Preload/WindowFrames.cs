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
using System.Threading.Tasks;

using GZSkinsX.Api.WindowManager;

using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Preload;

[Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.Preload_Guid, PageType = typeof(PreloadPage))]
internal sealed class ExportPreloadFrame : IWindowFrame
{
    private readonly IWindowManagerService _windowManagerService;
    private readonly PreloadSettings _preloadSettings;

    [ImportingConstructor]
    public ExportPreloadFrame(IWindowManagerService windowManagerService, PreloadSettings preloadSettings)
    {
        _windowManagerService = windowManagerService;
        _preloadSettings = preloadSettings;
    }

    /// <inheritdoc/>
    public async Task OnInitializeAsync(Page viewElement)
    {
        var random = new Random();
        var waitingTime = random.Next(1000, 2000);
        Debug.WriteLine($"AppxPreload: Waiting Time = {waitingTime}");
        await Task.Delay(waitingTime);

        _windowManagerService.NavigateTo(WindowFrameConstants.StartUp_Guid);
    }

    /// <inheritdoc/>
    public async Task OnNavigatingAsync(WindowFrameNavigateEventArgs args)
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

            appView.TryResizeView(minWindowSize);
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

        await Task.CompletedTask;
    }

    [ContractVersion(typeof(UniversalApiContract), 65536u)]
    [DllImport("api-ms-win-core-localization-obsolete-l1-2-0.dll", CharSet = CharSet.Auto)]
    private static extern ushort GetUserDefaultUILanguage();
}

