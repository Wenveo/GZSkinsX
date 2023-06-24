// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Runtime.InteropServices;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Api.Themes;
using GZSkinsX.Api.WindowManager;

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

using WinRT.Interop;

namespace GZSkinsX.MainApp;

internal sealed class ShellWindow : Window
{
    private ShellWindowSettings WindowSettigns { get; }

    public ShellWindow(MicaKind kind, bool extendsContentIntoTitleBar)
    {
        SystemBackdrop = new MicaBackdrop { Kind = kind };
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = extendsContentIntoTitleBar;
        AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;

        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        WindowSettigns = AppxContext.Resolve<ShellWindowSettings>();
        if (WindowSettigns.RestoreWindowPosition)
        {
            AppWindow.MoveAndResize(new(WindowSettigns.X_Axis, WindowSettigns.Y_Axis,
                WindowSettigns.WindowWidth, WindowSettigns.WindowHeight));
        }
        else
        {
            AppWindow.Resize(new(1200, 600));
        }

        if (WindowSettigns.IsMaximized)
        {
            ((OverlappedPresenter)AppWindow.Presenter).Maximize();
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            var themeService = AppxContext.ThemeService;
            themeService.ThemeChanged += OnThemeChanged;

            UpdateButtonForegroundColor(themeService.ActualTheme);
        });

        AppWindow.Destroying += OnDestroying;
        CompositionTarget.Rendered += OnRendered;
    }

    private void OnDestroying(AppWindow sender, object args)
    {
        if (((OverlappedPresenter)sender.Presenter).State is OverlappedPresenterState.Maximized)
        {
            var windowPlacement = new WINDOWPLACEMENT
            {
                length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
                showCmd = SHOW_WINDOW_CMD.SW_NORMAL
            };

            if (PInvoke.GetWindowPlacement((HWND)WindowNative.GetWindowHandle(this), ref windowPlacement))
            {
                WindowSettigns.X_Axis = windowPlacement.rcNormalPosition.X;
                WindowSettigns.Y_Axis = windowPlacement.rcNormalPosition.Y;
                WindowSettigns.WindowHeight = windowPlacement.rcNormalPosition.Height;
                WindowSettigns.WindowWidth = windowPlacement.rcNormalPosition.Width;

                WindowSettigns.RestoreWindowPosition = true;
            }
            else
            {
                WindowSettigns.RestoreWindowPosition = false;
            }

            WindowSettigns.IsMaximized = true;
        }
        else
        {
            WindowSettigns.X_Axis = sender.Position.X;
            WindowSettigns.Y_Axis = sender.Position.Y;
            WindowSettigns.WindowHeight = sender.Size.Height;
            WindowSettigns.WindowWidth = sender.Size.Width;

            WindowSettigns.IsMaximized = false;
            WindowSettigns.RestoreWindowPosition = true;
        }
    }

    private void OnRendered(object? sender, RenderedEventArgs e)
    {
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
