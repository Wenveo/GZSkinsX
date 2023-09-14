// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Themes;

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

using WinRT.Interop;

namespace GZSkinsX.Appx.MainApp;

internal sealed partial class ShellWindow : Window
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
            var defaultWindowSize = new SizeInt32(1004, 572);
            if (TryGetScaleAdjustment(out var dpiScale))
            {
                defaultWindowSize.Width = (int)(defaultWindowSize.Width * dpiScale.Value);
                defaultWindowSize.Height = (int)(defaultWindowSize.Height * dpiScale.Value);
            }

            AppWindow.Resize(defaultWindowSize);
        }

        if (WindowSettigns.IsMaximized)
        {
            ((OverlappedPresenter)AppWindow.Presenter).Maximize();
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            TryApplyAero();

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

    private bool TryApplyAero()
    {
        unsafe
        {
            var accentPolicy = new ACCENT_POLICY
            {
                AccentState = ACCENT_STATE.ACCENT_ENABLE_BLURBEHIND,
            };

            var data = new WINCOMPATTRDATA
            {
                Attribute = WINCOMPATTR.WCA_ACCENT_POLICY,
                SizeOfData = Marshal.SizeOf(accentPolicy),
                Data = (nint)(&accentPolicy)
            };

            return SetWindowCompositionAttribute(WindowNative.GetWindowHandle(this), ref data) == 0;
        }
    }

    private bool TryGetScaleAdjustment([NotNullWhen(true)] out double? dpiScale)
    {
        var wndId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(this));
        var displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
        var hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

        // Get DPI.
        var result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out var dpiX, out var _);
        if (result is 0)
        {
            dpiScale = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96) / 100.0;
            return true;
        }

        dpiScale = null;
        return false;
    }

    /// <summary>
    /// DWM window accent state.
    /// </summary>
    private enum ACCENT_STATE
    {
        ACCENT_ENABLE_BLURBEHIND = 3
    }

    /// <summary>
    /// DWM window attributes.
    /// </summary>
    private enum WINCOMPATTR
    {
        WCA_ACCENT_POLICY = 19
    }

    /// <summary>
    /// DWM window accent policy.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct ACCENT_POLICY
    {
        public ACCENT_STATE AccentState;
        public uint AccentFlags;
        public uint GradientColor;
        public uint AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WINCOMPATTRDATA
    {
        public WINCOMPATTR Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    /// <summary>
    /// Sets various information regarding DWM window attributes.
    /// </summary>
    [LibraryImport("user32.dll")]
    private static partial int SetWindowCompositionAttribute(nint hWnd, ref WINCOMPATTRDATA data);

    private enum Monitor_DPI_Type : int
    {
        MDT_Effective_DPI = 0,
        MDT_Angular_DPI = 1,
        MDT_Raw_DPI = 2,
        MDT_Default = MDT_Effective_DPI
    }

    [LibraryImport("Shcore.dll", SetLastError = true)]
    private static partial int GetDpiForMonitor(nint hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);
}
