// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml;

using Windows.Storage.Pickers;

using WinRT.Interop;

namespace GZSkinsX.Api.Appx;

public static class WindowExtensions
{
    public static FileOpenPicker InitializeWindowHandle(this FileOpenPicker picker)
        => InitializeWindowHandle(picker, AppxContext.AppxWindow.MainWindowHandle);

    public static FileOpenPicker InitializeWindowHandle(this FileOpenPicker picker, Window window)
        => InitializeWindowHandle(picker, WindowNative.GetWindowHandle(window));

    public static FileOpenPicker InitializeWindowHandle(this FileOpenPicker picker, nint hwnd)
    {
        InitializeWithWindow.Initialize(picker, hwnd);
        return picker;
    }

    public static FileSavePicker InitializeWindowHandle(this FileSavePicker picker)
        => InitializeWindowHandle(picker, AppxContext.AppxWindow.MainWindowHandle);

    public static FileSavePicker InitializeWindowHandle(this FileSavePicker picker, Window window)
        => InitializeWindowHandle(picker, WindowNative.GetWindowHandle(window));

    public static FileSavePicker InitializeWindowHandle(this FileSavePicker picker, nint hwnd)
    {
        InitializeWithWindow.Initialize(picker, hwnd);
        return picker;
    }

    public static FolderPicker InitializeWindowHandle(this FolderPicker picker)
        => InitializeWindowHandle(picker, AppxContext.AppxWindow.MainWindowHandle);

    public static FolderPicker InitializeWindowHandle(this FolderPicker picker, Window window)
        => InitializeWindowHandle(picker, WindowNative.GetWindowHandle(window));

    public static FolderPicker InitializeWindowHandle(this FolderPicker picker, nint hwnd)
    {
        InitializeWithWindow.Initialize(picker, hwnd);
        return picker;
    }
}
