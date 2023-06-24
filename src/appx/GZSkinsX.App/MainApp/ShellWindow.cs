// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Api.Themes;
using GZSkinsX.Api.WindowManager;

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace GZSkinsX.MainApp;

internal sealed class ShellWindow : Window
{
    public ShellWindow(MicaKind kind, bool extendsContentIntoTitleBar)
    {
        SystemBackdrop = new MicaBackdrop { Kind = kind };
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = extendsContentIntoTitleBar;

        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        DispatcherQueue.TryEnqueue(() =>
        {
            var themeService = AppxContext.ThemeService;
            themeService.ThemeChanged += OnThemeChanged;

            UpdateButtonForegroundColor(themeService.ActualTheme);
        });

        CompositionTarget.Rendered += OnRendered;
    }

    private int _renderCount;

    private void OnRendered(object? sender, RenderedEventArgs e)
    {
        /// 渲染到第 3 帧才显示出 UI 元素，3 帧之前还是黑屏
        /// 但未在其它版本的操作系统中测试过，因此该内容仅为参考
        if (++_renderCount < 2)
        {
            return;
        }

        CompositionTarget.Rendered -= OnRendered;

        var extensionService = StartUpClass.ExtensionService;
        extensionService.LoadAdvanceExtensions(AdvanceExtensionTrigger.AppLoaded);
        extensionService.NotifyUniversalExtensions(UniversalExtensionEvent.AppLoaded);

        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Preload_Guid);

        if (Content is FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = AppxContext.ThemeService.CurrentTheme;
        }
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        var themeService = (IThemeService?)sender;
        Debug.Assert(themeService is not null);

        UpdateButtonForegroundColor(e.ActualTheme);
    }

    private void UpdateButtonForegroundColor(ElementTheme newTheme)
    {
        AppWindow.TitleBar.ButtonForegroundColor = newTheme == ElementTheme.Light ? Colors.Black : Colors.White;
    }
}
