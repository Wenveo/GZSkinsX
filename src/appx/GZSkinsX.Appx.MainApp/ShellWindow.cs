// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
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
    private ShellWindowSettings WindowSettigns { get; }

    private SUBCLASSPROC? SubClassDelegate { get; set; }

    internal nint WindowHandle { get; }

    public int MinHeight { get; set; } = 572;

    public int MinWidth { get; set; } = 498;

    public int Height { get; set; } = 1004;

    public int Width { get; set; } = 572;

    public ShellWindow(MicaKind kind, bool extendsContentIntoTitleBar)
    {
        WindowHandle = WindowNative.GetWindowHandle(this);
        SystemBackdrop = new MicaBackdrop { Kind = kind };
        AppWindow.Title = ResourceHelper.GetLocalized("Resources/AppDisplayName");
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
            var dpiScale = (double)PInvoke.GetDpiForWindow((HWND)WindowHandle) / 96;
            AppWindow.Resize(new((int)(Width * dpiScale), (int)(Height * dpiScale)));
        }

        if (WindowSettigns.IsMaximized)
        {
            (AppWindow.Presenter as OverlappedPresenter)?.Maximize();
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            TryApplyAero();

            var themeService = AppxContext.ThemeService;
            themeService.ThemeChanged += OnThemeChanged;

            UpdateButtonForegroundColor(themeService.ActualTheme);
        });

        SubscribeWindowSubClass();
        AppWindow.Destroying += OnDestroying;
    }

    private void OnDestroying(AppWindow sender, object args)
    {
        if (sender.Presenter is OverlappedPresenter { State: OverlappedPresenterState.Maximized })
        {
            var windowPlacement = new WINDOWPLACEMENT
            {
                length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
                showCmd = SHOW_WINDOW_CMD.SW_NORMAL
            };

            if (PInvoke.GetWindowPlacement((HWND)WindowHandle, ref windowPlacement))
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

        var subClassDelegate = SubClassDelegate;
        if (subClassDelegate is not null)
        {
            SubClassDelegate = null;
            PInvoke.RemoveWindowSubclass((HWND)WindowHandle, subClassDelegate, 107);
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

            return SetWindowCompositionAttribute(WindowHandle, ref data) == 0;
        }
    }

    private void SubscribeWindowSubClass()
    {
        LRESULT WindowSubClass(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
        {
            switch (uMsg)
            {
                case PInvoke.WM_GETMINMAXINFO:
                    var dpiScale = (double)PInvoke.GetDpiForWindow(hWnd) / 96;
                    var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                    minMaxInfo.ptMinTrackSize.X = (int)(MinWidth * dpiScale);
                    minMaxInfo.ptMinTrackSize.Y = (int)(MinHeight * dpiScale);
                    Marshal.StructureToPtr(minMaxInfo, lParam, false);
                    break;
            }

            return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
        PInvoke.SetWindowSubclass((HWND)WindowHandle, SubClassDelegate, 107, 0);
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
}
