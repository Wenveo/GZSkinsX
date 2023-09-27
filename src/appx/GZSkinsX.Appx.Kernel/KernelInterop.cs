// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;

using Windows.Win32;

namespace GZSkinsX.Appx.Kernel;

internal sealed unsafe class KernelInterop(FreeLibrarySafeHandle moduleHandle)
{
    private readonly InitializeGZXKernelModuleFunc _initializeGZXKernelModuleFunc =
        PInvoke.GetProcAddress(moduleHandle, "InitializeGZXKernelModule")
            .CreateDelegate<InitializeGZXKernelModuleFunc>();

    private readonly EnsureMotClientAliveFunc _ensureMotClientAliveFunc =
        PInvoke.GetProcAddress(moduleHandle, "EnsureMotClientAlive")
            .CreateDelegate<EnsureMotClientAliveFunc>();

    private readonly EncryptConfigTextFunc _encryptConfigTextFunc =
        PInvoke.GetProcAddress(moduleHandle, "EncryptConfigText")
            .CreateDelegate<EncryptConfigTextFunc>();

    private readonly DecryptConfigTextFunc _decryptConfigTextFunc =
        PInvoke.GetProcAddress(moduleHandle, "DecryptConfigText")
            .CreateDelegate<DecryptConfigTextFunc>();

    private readonly FreeCryptographicBufferFunc _freeCryptographicBufferFunc =
        PInvoke.GetProcAddress(moduleHandle, "FreeCryptographicBuffer")
            .CreateDelegate<FreeCryptographicBufferFunc>();

    private readonly FreeLegacySkinInfoFunc _freeLegacySkinInfoFunc =
        PInvoke.GetProcAddress(moduleHandle, "FreeLegacySkinInfo")
            .CreateDelegate<FreeLegacySkinInfoFunc>();

    private readonly ReadLegacySkinImageFunc _readLegacySkinImageFunc =
        PInvoke.GetProcAddress(moduleHandle, "ReadLegacySkinImage")
            .CreateDelegate<ReadLegacySkinImageFunc>();

    private readonly ReadLegacySkinInfoFunc _readLegacySkinInfoFunc =
        PInvoke.GetProcAddress(moduleHandle, "ReadLegacySkinInfo")
            .CreateDelegate<ReadLegacySkinInfoFunc>();

    public FreeLibrarySafeHandle ModuleHandle = moduleHandle;

    public uint InitializeGZXKernelModule()
    {
        return _initializeGZXKernelModuleFunc();
    }

    public int EnsureMotClientAlive()
    {
        return _ensureMotClientAliveFunc();
    }

    public void EncryptConfigText(void* text, ref nint dst)
    {
        _encryptConfigTextFunc(text, ref dst);
    }

    public void DecryptConfigText(void* text, ref nint dst)
    {
        _decryptConfigTextFunc(text, ref dst);
    }

    public void FreeCryptographicBuffer(nint buffer)
    {
        _freeCryptographicBufferFunc(buffer);
    }

    public void FreeLegacySkinInfo(void* modInfoPtr)
    {
        _freeLegacySkinInfoFunc(modInfoPtr);
    }

    public uint ReadLegacySkinImage(char* filename, ref nint buffer, ref int length)
    {
        return _readLegacySkinImageFunc(filename, ref buffer, ref length);
    }

    public uint ReadLegacySkinInfo(char* filename, ref nint skinInfoPtr)
    {
        return _readLegacySkinInfoFunc(filename, ref skinInfoPtr);
    }

    private delegate uint InitializeGZXKernelModuleFunc();

    private delegate int EnsureMotClientAliveFunc();

    private delegate void EncryptConfigTextFunc(void* text, ref nint dst);

    private delegate void DecryptConfigTextFunc(void* text, ref nint dst);

    private delegate void FreeCryptographicBufferFunc(nint buffer);

    private delegate void FreeLegacySkinInfoFunc(void* modInfoPtr);

    private delegate uint ReadLegacySkinImageFunc(char* filename, ref nint buffer, ref int length);

    private delegate uint ReadLegacySkinInfoFunc(char* filename, ref nint skinInfoPtr);
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct NativeModInfo
{
    public char* name;
    public char* author;
    public char* description;
    public char* datetime;
}
