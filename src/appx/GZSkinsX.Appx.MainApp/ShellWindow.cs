// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Themes;

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

using WinRT.Interop;

namespace GZSkinsX.Appx.MainApp;

internal sealed partial class ShellWindow : Window
{
    private ShellWindowSettings WindowSettings { get; }

    private SUBCLASSPROC? SubClassDelegate { get; set; }

    internal nint WindowHandle { get; }

    public ShellWindow(MicaKind kind, bool extendsContentIntoTitleBar)
    {
        WindowHandle = WindowNative.GetWindowHandle(this);
        SystemBackdrop = new MicaBackdrop { Kind = kind };
        AppWindow.Title = ResourceHelper.GetLocalized("Resources/AppDisplayName");
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = extendsContentIntoTitleBar;
        AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;

        AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        WindowSettings = AppxContext.Resolve<ShellWindowSettings>();
        if (WindowSettings.NeedToRestoreWindowState)
        {
            AppWindow.MoveAndResize(new(WindowSettings.WindowLeft, WindowSettings.WindowTop,
                WindowSettings.WindowWidth, WindowSettings.WindowHeight));
        }
        else
        {
            var dpiScale = (double)PInvoke.GetDpiForWindow((HWND)WindowHandle) / 96;
            AppWindow.Resize(new((int)(1004 * dpiScale), (int)(578 * dpiScale)));
        }

        // Update the win32 window style.
        UpdateWindowStyle((HWND)WindowHandle);

        // Apply acrylic for the win32 host window.
        DispatcherQueue.TryEnqueue(ApplyAcrylicForWin32Window);

        // Make sure to maximize the window.
        if (WindowSettings.IsMaximized)
        {
            // Maximize the window after UpdateWindowStyle...
            // Because this will be change the win32 window style.
            (AppWindow.Presenter as OverlappedPresenter)?.Maximize();
        }

        var themeService = AppxContext.ThemeService;
        themeService.ThemeChanged += OnThemeChanged;
        UpdateButtonForegroundColor(themeService.ActualTheme);

        AppWindow.Destroying += OnDestroying;
        SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
        PInvoke.SetWindowSubclass((HWND)WindowHandle, SubClassDelegate, 107, 0);
    }

    private unsafe void UpdateWindowStyle(HWND hWnd)
    {
        var newWindowStyle = WINDOW_STYLE.WS_CAPTION | WINDOW_STYLE.WS_THICKFRAME | WINDOW_STYLE.WS_CLIPSIBLINGS |
            WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_OVERLAPPED | WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_MAXIMIZEBOX;

        // Hide the default titlebar buttons.
        // See also: https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window
        var dwNewWindowStyleLong = (int)newWindowStyle & ~0x80000;
        var result = PInvoke.SetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, dwNewWindowStyleLong);
        Debug.Assert(result is not 0);

        // 将系统主题色填充至窗口背景，避免第一次显示时出现白色背景闪烁。
        // Fill the accent (fallback) color of the win32 window,
        // to avoid flickering on startup (avoid white or black background...).
        if (PInvoke.GetClientRect(hWnd, out var rect))
        {
            var hdc = PInvoke.GetDC(hWnd);
            if (hdc.Value != nint.Zero)
            {
                var accentColor = new Windows.UI.ViewManagement.UISettings().GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
                var fallbackBrush = PInvoke.CreateSolidBrush(new((uint)(accentColor.R + (accentColor.G << 8) + (accentColor.B << 16))));

                result = PInvoke.FillRect(hdc, &rect, fallbackBrush);
                Debug.Assert(result is not 0);

                result = PInvoke.DeleteObject(fallbackBrush).Value;
                Debug.Assert(result is not 0);

                result = PInvoke.ReleaseDC(hWnd, hdc);
                Debug.Assert(result is not 0);
            }
        }
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        UpdateButtonForegroundColor(e.ActualTheme);
    }

    private void UpdateButtonForegroundColor(ElementTheme newTheme)
    {
        AppWindow.TitleBar.ButtonForegroundColor = newTheme is ElementTheme.Light ? Colors.Black : Colors.White;
    }

    private void OnDestroying(AppWindow sender, object args)
    {
        if (sender.Presenter is OverlappedPresenter { State: OverlappedPresenterState.Maximized })
        {
            var windowPlacement = new WINDOWPLACEMENT
            {
                length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
                showCmd = SHOW_WINDOW_CMD.SW_SHOWNORMAL
            };

            if (PInvoke.GetWindowPlacement((HWND)WindowHandle, ref windowPlacement))
            {
                WindowSettings.WindowLeft = windowPlacement.rcNormalPosition.X;
                WindowSettings.WindowTop = windowPlacement.rcNormalPosition.Y;
                WindowSettings.WindowHeight = windowPlacement.rcNormalPosition.Height;
                WindowSettings.WindowWidth = windowPlacement.rcNormalPosition.Width;

                WindowSettings.NeedToRestoreWindowState = true;
            }
            else
            {
                WindowSettings.NeedToRestoreWindowState = false;
            }

            WindowSettings.IsMaximized = true;
        }
        else
        {
            WindowSettings.WindowLeft = sender.Position.X;
            WindowSettings.WindowTop = sender.Position.Y;
            WindowSettings.WindowHeight = sender.Size.Height;
            WindowSettings.WindowWidth = sender.Size.Width;

            WindowSettings.IsMaximized = false;
            WindowSettings.NeedToRestoreWindowState = true;
        }

        var subClassDelegate = SubClassDelegate;
        if (subClassDelegate is not null)
        {
            SubClassDelegate = null;
            PInvoke.RemoveWindowSubclass((HWND)WindowHandle, subClassDelegate, 107);
        }
    }

    private unsafe LRESULT WindowSubClass(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case PInvoke.WM_GETMINMAXINFO:
                WmGetMinMaxInfo(hWnd, lParam);
                break;
        }

        return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    private static void WmGetMinMaxInfo(HWND hWnd, LPARAM lParam)
    {
        var dpiScale = (double)PInvoke.GetDpiForWindow(hWnd) / 96;
        var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
        minMaxInfo.ptMinTrackSize.X = (int)(498 * dpiScale);
        minMaxInfo.ptMinTrackSize.Y = (int)(578 * dpiScale);
        Marshal.StructureToPtr(minMaxInfo, lParam, false);
    }

    private void ApplyAcrylicForWin32Window()
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

            SetWindowCompositionAttribute(WindowHandle, ref data);
        }
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
        public nint Data;
        public int SizeOfData;
    }

    /// <summary>
    /// Sets various information regarding DWM window attributes.
    /// </summary>
    [LibraryImport("user32.dll")]
    private static partial int SetWindowCompositionAttribute(nint hWnd, ref WINCOMPATTRDATA data);
}
