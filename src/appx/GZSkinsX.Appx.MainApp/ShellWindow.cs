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
using GZSkinsX.Contracts.ContextMenu;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Themes;

using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

using Windows.Foundation;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;

using WinRT.Interop;

namespace GZSkinsX.Appx.MainApp;

internal sealed partial class ShellWindow : Window
{
    private NonClientSourceManager? NCSourceManager { get; set; }

    private FlyoutBase? SystemMenuFlyout { get; set; }

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

        var scaling = (double)PInvoke.GetDpiForWindow((HWND)WindowHandle) / 96;
        WindowSettings = AppxContext.Resolve<ShellWindowSettings>();
        if (WindowSettings.NeedToRestoreWindowState)
        {
            AppWindow.MoveAndResize(new(
                (int)Math.Round(WindowSettings.WindowLeft * scaling), (int)Math.Round(WindowSettings.WindowTop * scaling),
                (int)Math.Round(WindowSettings.WindowWidth * scaling), (int)Math.Round(WindowSettings.WindowHeight * scaling)));
        }
        else
        {
            AppWindow.Resize(new((int)(1004 * scaling), (int)(814 * scaling)));
        }

        var manager = NonClientSourceManager.GetFromWindowHandle(WindowHandle);
        if (manager is not null)
        {
            NCSourceManager = manager;
            manager.MouseLeftButtonDown += NCMouseLeftButtonDown;
            manager.MouseRightButtonDown += NCMouseRightButtonDown;

            SystemMenuFlyout = AppxContext.ContextMenuService.CreateContextMenu(
                ContextMenuConstants.SHELL_SYSTEMMENU_CTX_GUID, new ContextMenuOptions { Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft },
                (s, e) => new ShellSystemMenuUIContext { AppWindowId = AppWindow.Id, WindowHandle = WindowHandle, IsMaximized = PInvoke.IsZoomed((HWND)WindowHandle) });
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

    private void NCMouseLeftButtonDown(NonClientSourceManager sender, NonClientSourceMouseButtonEventArgs args)
    {
        if (Content is { XamlRoot: { } xamlRoot })
        {
            foreach (var popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(xamlRoot))
            {
                // 判断是否有已打开的内容框
                if (popup.Child as ContentDialog is not null)
                {
                    continue;
                }

                // 判断是否为内容框背后的背景层
                if (popup.Child is Rectangle { Name: "SmokeLayerBackground" })
                {
                    continue;
                }

                popup.IsOpen = false;
            }
        }
    }

    private void NCMouseRightButtonDown(NonClientSourceManager sender, NonClientSourceMouseButtonEventArgs args)
    {
        ShowCustomSystemMenu(args.Position);
    }

    private void ShowCustomSystemMenu(Point position)
    {
        if (Content is not { } windowContent)
        {
            return;
        }

        SystemMenuFlyout?.ShowAt(windowContent, new FlyoutShowOptions()
        {
            Position = position,
            Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft,
            ShowMode = FlyoutShowMode.Standard
        });
    }

    private unsafe void UpdateWindowStyle(HWND hWnd)
    {
        var newWindowStyle = WINDOW_STYLE.WS_CAPTION | WINDOW_STYLE.WS_THICKFRAME | WINDOW_STYLE.WS_CLIPSIBLINGS |
            WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_OVERLAPPED | WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_MAXIMIZEBOX;

        // Hide the default titlebar buttons.
        // See also: https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window
        var dwNewWindowStyleLong = new nint((int)newWindowStyle & ~0x80000);
        var result = PInvoke.SetWindowLong(hWnd, -16, dwNewWindowStyleLong);
        Debug.Assert(result is not 0);

        // To show the custom system menu ( [MOD_ALT] Alt + Space [0x20] ).
        PInvoke.RegisterHotKey(hWnd, 132, HOT_KEY_MODIFIERS.MOD_ALT, 0x20);

        // 将系统主题色填充至窗口背景，避免第一次显示时出现白色背景闪烁。
        // Fill the (fallback) accent color to the win32 window background,
        // to avoid flickering on startup (avoid white or black background...).
        if (PInvoke.GetClientRect(hWnd, out var rect))
        {
            var hdc = PInvoke.GetDC(hWnd);
            if (hdc.Value != nint.Zero)
            {
                var accentColor = AppxContext.ThemeService.UISettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
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
        var scaling = (double)PInvoke.GetDpiForWindow((HWND)WindowHandle) / 96;
        if (sender.Presenter is OverlappedPresenter { State: OverlappedPresenterState.Maximized })
        {
            var windowPlacement = new WINDOWPLACEMENT
            {
                length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
                showCmd = SHOW_WINDOW_CMD.SW_SHOWNORMAL
            };

            if (PInvoke.GetWindowPlacement((HWND)WindowHandle, ref windowPlacement))
            {
                WindowSettings.WindowLeft = (int)Math.Round(windowPlacement.rcNormalPosition.X / scaling);
                WindowSettings.WindowTop = (int)Math.Round(windowPlacement.rcNormalPosition.Y / scaling);
                WindowSettings.WindowHeight = (int)Math.Round(windowPlacement.rcNormalPosition.Height / scaling);
                WindowSettings.WindowWidth = (int)Math.Round(windowPlacement.rcNormalPosition.Width / scaling);

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
            WindowSettings.WindowLeft = (int)Math.Round(sender.Position.X / scaling);
            WindowSettings.WindowTop = (int)Math.Round(sender.Position.Y / scaling);
            WindowSettings.WindowHeight = (int)Math.Round(sender.Size.Height / scaling);
            WindowSettings.WindowWidth = (int)Math.Round(sender.Size.Width / scaling);

            WindowSettings.IsMaximized = false;
            WindowSettings.NeedToRestoreWindowState = true;
        }

        var manager = NCSourceManager;
        if (manager is not null)
        {
            NCSourceManager = null;
            manager.MouseLeftButtonDown -= NCMouseLeftButtonDown;
            manager.MouseRightButtonDown -= NCMouseRightButtonDown;
        }

        var systemMenuFlyout = SystemMenuFlyout;
        if (systemMenuFlyout is not null)
        {
            SystemMenuFlyout = null;
        }

        var subClassDelegate = SubClassDelegate;
        if (subClassDelegate is not null)
        {
            SubClassDelegate = null;
            PInvoke.UnregisterHotKey((HWND)WindowHandle, 132);
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

            case PInvoke.WM_HOTKEY:
                WmHotKey(wParam);
                break;
        }

        return PInvoke.DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    private static void WmGetMinMaxInfo(HWND hWnd, LPARAM lParam)
    {
        var scaling = (double)PInvoke.GetDpiForWindow(hWnd) / 96;
        var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
        minMaxInfo.ptMinTrackSize.X = (int)(812 * scaling);
        minMaxInfo.ptMinTrackSize.Y = (int)(582 * scaling);
        Marshal.StructureToPtr(minMaxInfo, lParam, false);
    }

    private void WmHotKey(WPARAM wParam)
    {
        if (wParam.Value.Equals(132))
        {
            ShowCustomSystemMenu(new(10, 38));
        }
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
