// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;

using GZSkinsX.Appx.Contracts.App;
using GZSkinsX.Appx.Contracts.Themes;

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

using WinRT.Interop;

namespace GZSkinsX.Appx.MainApp;

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
        if (WindowSettigns.NeedToRestoreWindowState)
        {
            AppWindow.MoveAndResize(new(WindowSettigns.WindowLeft, WindowSettigns.WindowTop,
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
                WindowSettigns.WindowLeft = windowPlacement.rcNormalPosition.X;
                WindowSettigns.WindowTop = windowPlacement.rcNormalPosition.Y;
                WindowSettigns.WindowHeight = windowPlacement.rcNormalPosition.Height;
                WindowSettigns.WindowWidth = windowPlacement.rcNormalPosition.Width;

                WindowSettigns.NeedToRestoreWindowState = true;
            }
            else
            {
                WindowSettigns.NeedToRestoreWindowState = false;
            }

            WindowSettigns.IsMaximized = true;
        }
        else
        {
            WindowSettigns.WindowLeft = sender.Position.X;
            WindowSettigns.WindowTop = sender.Position.Y;
            WindowSettigns.WindowHeight = sender.Size.Height;
            WindowSettigns.WindowWidth = sender.Size.Width;

            WindowSettigns.IsMaximized = false;
            WindowSettigns.NeedToRestoreWindowState = true;
        }
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        UpdateButtonForegroundColor(e.ActualTheme);
    }

    private void UpdateButtonForegroundColor(ElementTheme newTheme)
    {
        AppWindow.TitleBar.ButtonForegroundColor =
            newTheme is ElementTheme.Light ? Colors.Black : Colors.White;
    }
}
