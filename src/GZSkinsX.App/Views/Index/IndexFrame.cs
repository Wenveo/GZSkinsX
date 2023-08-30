// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.WindowManager;

using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace GZSkinsX.Views;

[System.Composition.Shared, ExportWindowFrame]
[WindowFrameMetadata(Guid = WindowFrameConstants.Index_Guid, PageType = typeof(IndexPage))]
internal sealed class IndexFrame : IWindowFrame
{
    private IndexSettings Settings { get; }

    public IndexFrame()
    {
        Settings = AppxContext.Resolve<IndexSettings>();
        InitializeMainWindow();
    }

    public bool CanNavigateTo(WindowFrameNavigatingEvnetArgs args)
    {
        return true;
    }

    private void InitializeMainWindow()
    {
        if (Settings.IsInitialize is false)
        {
            var minWindowSize = SizeHelper.FromDimensions(988, 568);
            var appView = ApplicationView.GetForCurrentView();
            appView.SetPreferredMinSize(minWindowSize);

            if (!appView.TryResizeView(minWindowSize))
            {
                AppxContext.LoggingService.LogWarning("AppxStartUp: Failed to resize the window.");
            }

            AppxContext.AppxWindow.Closed += (_, _) => Settings.IsInitialize = true;
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

        AppxContext.LoggingService.LogDebug($"AppxIndex: IsInitialize = {Settings.IsInitialize}");
    }
}
