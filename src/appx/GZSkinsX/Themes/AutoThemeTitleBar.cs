﻿// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Api.Themes;

using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;

namespace GZSkinsX.Themes;

[Shared, ExportAutoLoaded]
[AutoLoadedMetadata(LoadType = AutoLoadedType.AppLoaded)]
internal sealed class AutoThemeTitleBar : IAutoLoaded
{
    private readonly IAppxTitleBarButton _appxTitleBarButton;
    private readonly IThemeService _themeService;

    [ImportingConstructor]
    public AutoThemeTitleBar(IAppxTitleBar appxTitleBar, IAppxTitleBarButton appxTitleBarButton, IThemeService themeService)
    {
        appxTitleBar.ExtendViewIntoTitleBar = true;

        _appxTitleBarButton = appxTitleBarButton;
        _appxTitleBarButton.ButtonBackgroundColor = Colors.Transparent;
        _appxTitleBarButton.ButtonInactiveBackgroundColor = Colors.Transparent;

        _themeService = themeService;
        _themeService.ThemeChanged += OnThemeChanged;

        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        if (dispatcherQueue.HasThreadAccess)
            OnInitialize();
        else
            dispatcherQueue.TryEnqueue(OnInitialize);
    }

    private void OnInitialize()
    {
        OnThemeChangedImpl(_themeService.ActualTheme);
    }

    private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
    {
        OnThemeChangedImpl(e.ActualTheme);
    }

    private void OnThemeChangedImpl(ElementTheme newTheme)
    {
        _appxTitleBarButton.ButtonForegroundColor = newTheme == ElementTheme.Light ? Colors.Black : Colors.White;
    }
}