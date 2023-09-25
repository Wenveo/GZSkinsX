// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MainApp;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class ShellSystemMenuFlyout : MenuFlyout
{
    public ShellSystemMenuFlyout()
    {
        InitializeComponent();
    }

    private void OnOpening(object sender, object e)
    {
        if (XamlRoot is not { } xamlRoot)
        {
            return;
        }

        var appWinodwId = xamlRoot.ContentIslandEnvironment.AppWindowId;
        var hWnd = (HWND)Win32Interop.GetWindowFromWindowId(appWinodwId);
        if (PInvoke.IsZoomed(hWnd))
        {
            RestoreMenuItem.IsEnabled = true;
            MoveMenuItem.IsEnabled = false;
            SizeMenuItem.IsEnabled = false;
            MaximizeMenuItem.IsEnabled = false;
        }
        else
        {
            RestoreMenuItem.IsEnabled = false;
            MoveMenuItem.IsEnabled = true;
            SizeMenuItem.IsEnabled = true;
            MaximizeMenuItem.IsEnabled = true;
        }
    }

    private void HideMenuAndDosomething(Action<HWND> action)
    {
        Hide();
        if (XamlRoot is { } xamlRoot)
        {
            var appWinodwId = xamlRoot.ContentIslandEnvironment.AppWindowId;
            action((HWND)Win32Interop.GetWindowFromWindowId(appWinodwId));
        }
    }

    private void OnRestoreMenuItemClick(object sender, RoutedEventArgs e)
    {
        HideMenuAndDosomething((hWnd) =>
        {
            PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_SHOWNORMAL);
        });
    }

    private void OnMoveMenuItemClick(object sender, RoutedEventArgs e)
    {
        HideMenuAndDosomething((hWnd) =>
        {
            PInvoke.SendMessage(hWnd, PInvoke.WM_SYSCOMMAND, new(0xF010), new(nint.Zero));
        });
    }

    private void OnSizeMenuItemClick(object sender, RoutedEventArgs e)
    {
        HideMenuAndDosomething((hWnd) =>
        {
            PInvoke.SendMessage(hWnd, PInvoke.WM_SYSCOMMAND, new(0xF000), new(nint.Zero));
        });
    }

    private void OnMinimizeItemClick(object sender, RoutedEventArgs e)
    {
        HideMenuAndDosomething((hWnd) =>
        {
            PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_MINIMIZE);
        });
    }

    private void OnMaximizeMenuItemClick(object sender, RoutedEventArgs e)
    {
        HideMenuAndDosomething((hWnd) =>
        {
            PInvoke.ShowWindow(hWnd, SHOW_WINDOW_CMD.SW_MAXIMIZE);
        });
    }

    private void OnSettingsMenuItemClick(object sender, RoutedEventArgs e)
    {
        Hide();
        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Settings_Guid);
    }

    private void OnCloseMenuItemClick(object sender, RoutedEventArgs e)
    {
        HideMenuAndDosomething((hWnd) =>
        {
            PInvoke.SendMessage(hWnd, PInvoke.WM_CLOSE, new(nuint.Zero), new(nint.Zero));
        });
    }
}
