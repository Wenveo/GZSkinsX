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
using GZSkinsX.Api.Shell;

using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Preload;

/// <summary>
/// 
/// </summary>
[Shared, ExportViewElement]
[ViewElementMetadata(Guid = ViewElementConstants.Preload_Guid, PageType = typeof(PreloadPage))]
internal sealed class ExportPreloadPage : IViewElement
{
    /// <summary>
    /// 
    /// </summary>
    private readonly IViewManagerService _viewManagerService;

    /// <summary>
    /// 
    /// </summary>
    private readonly PreloadSettings _preloadSettings;

    /// <summary>
    /// 
    /// </summary>
    [ImportingConstructor]
    public ExportPreloadPage(IViewManagerService viewManagerService, PreloadSettings preloadSettings)
    {
        _viewManagerService = viewManagerService;
        _preloadSettings = preloadSettings;
    }

    /// <inheritdoc/>
    public async Task OnInitializeAsync(Page viewElement)
    {
        var random = new Random();
        var waitingTime = random.Next(1000, 2000);
        Debug.WriteLine($"AppxPreload: Waiting Time = {waitingTime}");
        await Task.Delay(waitingTime);

        _viewManagerService.NavigateTo(ViewElementConstants.StartUp_Guid);
    }

    /// <inheritdoc/>
    public async Task OnNavigatingAsync(WindowFrameNavigateEventArgs args)
    {
        var minWindowSize = SizeHelper.FromDimensions(720, 480);
        var appView = ApplicationView.GetForCurrentView();
        appView.SetPreferredMinSize(minWindowSize);

        if (_preloadSettings.IsInitialize is false)
        {
            appView.TryResizeView(minWindowSize);
            _preloadSettings.IsInitialize = true;
        }
        else
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
        }

        if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")) // PC Family
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
}

