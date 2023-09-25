// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Microsoft.UI;
using Microsoft.UI.Windowing;

using Windows.Foundation;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace GZSkinsX.Appx.MainApp;

internal sealed record class NonClientSourceMouseButtonEventArgs(Point Position);

internal sealed partial class NonClientSourceManager
{
    private static readonly Dictionary<nint, NonClientSourceManager> s_hWndToManager = [];

    private static void OnAppWindowDestroying(AppWindow sender, object args)
    {
        sender.Destroying -= OnAppWindowDestroying;
        var hWnd = Win32Interop.GetWindowFromWindowId(sender.Id);
        if (hWnd != nint.Zero && s_hWndToManager.TryGetValue(hWnd, out var manager))
        {
            manager.UnSubclass();
            s_hWndToManager.Remove(hWnd);
        }
    }

    public static NonClientSourceManager? GetFromWindowHandle(nint hWnd)
    {
        if (s_hWndToManager.TryGetValue(hWnd, out var manager))
        {
            return manager;
        }

        unsafe
        {
            fixed (char* ch = "InputNonClientPointerSource")
            {
                var NonClientSource = PInvoke.FindWindowEx((HWND)hWnd,
                    HWND.Null, new PCWSTR(ch), new((char*)nint.Zero));

                if (NonClientSource.IsNull)
                {
                    return null;
                }

                var appWindowId = Win32Interop.GetWindowIdFromWindow(hWnd);
                var appWindow = AppWindow.GetFromWindowId(appWindowId);
                if (appWindow is not null)
                {
                    appWindow.Destroying += OnAppWindowDestroying;
                }

                manager = new NonClientSourceManager(NonClientSource);
                s_hWndToManager.Add(hWnd, manager);
                return manager;
            }
        }
    }

    public event TypedEventHandler<NonClientSourceManager, NonClientSourceMouseButtonEventArgs>? MouseLeftButtonDown;

    public event TypedEventHandler<NonClientSourceManager, NonClientSourceMouseButtonEventArgs>? MouseRightButtonDown;

    private readonly WNDPROC _priorWindowProc;
    private readonly WNDPROC _windowProc;
    private readonly nint _priorWindowProcHandle;
    private readonly nint _windowProcHandle;
    private readonly HWND _windowHandle;

    private NonClientSourceManager(HWND windowHandle)
    {
        _windowHandle = windowHandle;
        _windowProc = new WNDPROC(Callback);
        _priorWindowProcHandle = PInvoke.SetWindowLong(_windowHandle, -4, Marshal.GetFunctionPointerForDelegate(_windowProc));
        _windowProcHandle = PInvoke.GetWindowLong(_windowHandle, -4);
        // This shouldn't be possible.
        Debug.Assert(_priorWindowProcHandle != _windowProcHandle, "Uh oh! Subclassed ourselves!!!");
        _priorWindowProc = Marshal.GetDelegateForFunctionPointer<WNDPROC>(_priorWindowProcHandle);
    }

    private LRESULT Callback(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam)
    {
        static bool TryGetPoint(HWND hWnd, LPARAM lParam, [NotNullWhen(true)] out Point? point)
        {
            if (PInvoke.GetWindowRect(hWnd, out var rect))
            {
                var xy = lParam.Value.ToInt32();
                var scaling = (double)PInvoke.GetDpiForWindow(hWnd) / 96;
                point = new Point
                {
                    // Position relative to the window
                    X = Math.Round(((short)(xy & 0xFFFF) - rect.X) / scaling),
                    Y = Math.Round(((short)(xy >> 16) - rect.Y) / scaling)
                };

                return true;
            }

            point = null;
            return false;
        }

        switch (uMsg)
        {
            case PInvoke.WM_NCLBUTTONDOWN:
                if (TryGetPoint(hWnd, lParam, out var point))
                {
                    MouseLeftButtonDown?.Invoke(this, new(point.Value));
                }
                break;

            case PInvoke.WM_NCRBUTTONDOWN:
                if (TryGetPoint(hWnd, lParam, out point))
                {
                    MouseRightButtonDown?.Invoke(this, new(point.Value));
                }
                break;
        }

        return PInvoke.CallWindowProc(_priorWindowProc, hWnd, uMsg, wParam, lParam);
    }

    private void UnSubclass()
    {
        var currentWndProc = PInvoke.GetWindowLong(_windowHandle, -4);
        if (_windowProcHandle == currentWndProc)
        {
            PInvoke.SetWindowLong(_windowHandle, -4, _priorWindowProcHandle);
        }
    }
}
