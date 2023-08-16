// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Extension;
using GZSkinsX.Contracts.Themes;

using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;

namespace GZSkinsX.Themes;

[AdvanceExtensionMetadata]
[Shared, ExportAdvanceExtension]
internal sealed class AutoThemeTitleBar : IAdvanceExtension
{
    private readonly IAppxTitleBar _appxTitleBar;
    private readonly IThemeService _themeService;

    [method: ImportingConstructor]
    public AutoThemeTitleBar(IAppxTitleBar appxTitleBar, IThemeService themeService)
    {
        _appxTitleBar = appxTitleBar;
        _appxTitleBar.ExtendViewIntoTitleBar = true;
        _appxTitleBar.ButtonBackgroundColor = Colors.Transparent;
        _appxTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

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
        _appxTitleBar.ButtonForegroundColor = newTheme == ElementTheme.Light ? Colors.Black : Colors.White;
    }
}
