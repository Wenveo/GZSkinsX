// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;

namespace GZSkinsX.Appx.MyMods;

internal static unsafe partial class KernelInterop
{
    [LibraryImport("GZSkinsX.Kernel.dll", SetLastError = true)]
    internal static partial uint InitializeGZXKernelModule();

    [LibraryImport("GZSkinsX.Kernel.dll", SetLastError = true)]
    internal static unsafe partial void EncryptConfigText(char* text, ref nint dst);

    [LibraryImport("GZSkinsX.Kernel.dll", SetLastError = true)]
    internal static unsafe partial void DecryptConfigText(char* text, ref nint dst);

    [LibraryImport("GZSkinsX.Kernel.dll", SetLastError = true)]
    internal static unsafe partial void FreeCryptographicBuffer(nint buffer);

    [LibraryImport("GZSkinsX.Kernel.dll", SetLastError = true)]
    internal static unsafe partial void FreeLegacySkinInfo(void* skinInfoPtr);

    [LibraryImport("GZSkinsX.Kernel.dll", SetLastError = true)]
    internal static unsafe partial uint ReadLegacySkinImage(char* filename, ref nint buffer, ref int length);

    [LibraryImport("GZSkinsX.Kernel.dll", SetLastError = true)]
    internal static unsafe partial uint ReadLegacySkinInfo(char* filename, ref nint skinInfoPtr);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct LegacySkinInfo
    {
        public char* name;
        public char* author;
        public char* description;
        public char* datetime;
    }
}
