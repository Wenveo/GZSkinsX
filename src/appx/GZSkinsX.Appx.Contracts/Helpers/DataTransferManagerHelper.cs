// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;

using GZSkinsX.Contracts.Appx;

using Windows.ApplicationModel.DataTransfer;

namespace GZSkinsX.Contracts.Helpers;

public partial class DataTransferManagerHelper
{
    public static readonly Guid DataTransferManagerInteropIID =
        new(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);

    public static DataTransferManager GetDataTransferManager()
    {
        return GetDataTransferManager(AppxContext.AppxWindow.MainWindowHandle);
    }

    public static DataTransferManager GetDataTransferManager(nint windowHandle)
    {
        var interop = DataTransferManager.As<IDataTransferManagerInterop>();
        return DataTransferManager.FromAbi(interop.GetForWindow(windowHandle, DataTransferManagerInteropIID));
    }

    public static IDataTransferManagerInterop GetDataTransferManagerInterop()
    {
        return DataTransferManager.As<IDataTransferManagerInterop>();
    }

    [ComImport]
    [Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataTransferManagerInterop
    {
        nint GetForWindow([In] nint appWindow, [In] ref Guid riid);

        void ShowShareUIForWindow(nint appWindow);
    }
}
