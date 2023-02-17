// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;

using Windows.System;

namespace GZSkinsX.Appx.Backdrops;

/// <summary>
/// Copy from: <see href="https://github.com/microsoft/WinUI-Gallery/blob/595afaae7296f5c61e4b5247ea4eb868bc4d35ac/WinUIGallery/SamplePages/SampleSystemBackdropsWindow.xaml.cs#L12-L49"/>
/// </summary>
internal sealed partial class WindowsSystemDispatcherQueueHelper
{
    [StructLayout(LayoutKind.Sequential)]
    private struct DispatcherQueueOptions
    {
        public int dwSize;
        public int threadType;
        public int apartmentType;
    }

    [LibraryImport("CoreMessaging.dll")]
    private static unsafe partial int CreateDispatcherQueueController(
        DispatcherQueueOptions options, IntPtr* instance);

    private IntPtr _dispatcherQueueController = IntPtr.Zero;

    public void EnsureWindowsSystemDispatcherQueueController()
    {
        if (DispatcherQueue.GetForCurrentThread() != null)
        {
            // one already exists, so we'll just use it.
            return;
        }

        if (_dispatcherQueueController == IntPtr.Zero)
        {
            DispatcherQueueOptions options;
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType = 2;    // DQTYPE_THREAD_CURRENT
            options.apartmentType = 2; // DQTAT_COM_STA

            unsafe
            {
                IntPtr dispatcherQueueController;
                CreateDispatcherQueueController(options, &dispatcherQueueController);
                _dispatcherQueueController = dispatcherQueueController;
            }
        }
    }
}
