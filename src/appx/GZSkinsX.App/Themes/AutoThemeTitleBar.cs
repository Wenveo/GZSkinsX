// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Api.Themes;

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace GZSkinsX.Themes;

[Shared, ExportAdvanceExtension]
[AdvanceExtensionMetadata(Trigger = AdvanceExtensionTrigger.AppLoaded)]
internal sealed class AutoThemeTitleBar : IAdvanceExtension
{
    private readonly IAppxTitleBar _appxTitleBar;
    private readonly IThemeService _themeService;

    public AutoThemeTitleBar()
    {
        AppxContext.AppxWindow.MainWindow.SystemBackdrop = new MicaBackdrop();

        _appxTitleBar = AppxContext.AppxTitleBar;
        _appxTitleBar.ButtonBackgroundColor = Colors.Transparent;
        _appxTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        _appxTitleBar.ExtendsContentIntoTitleBar = true;

        _themeService = AppxContext.ThemeService;
        _themeService.ThemeChanged += OnThemeChanged;

        DispatcherQueue.GetForCurrentThread().TryEnqueue(OnInitialize);
    }

    private void OnInitialize()
    {
        OnThemeChangedImpl(_themeService.ActualTheme);
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        OnThemeChangedImpl(e.ActualTheme);
    }

    private void OnThemeChangedImpl(ElementTheme newTheme)
    {
        _appxTitleBar.ButtonForegroundColor = newTheme == ElementTheme.Light ? Colors.Black : Colors.White;
    }
}