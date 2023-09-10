// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;

using Windows.Win32;

namespace GZSkinsX.DesktopExtension;

internal sealed unsafe class KernelInterop : IDisposable
{
#pragma warning disable format
    private readonly InitializeGZXKernelModuleFunc   _initializeGZXKernelModuleFunc;
    private readonly EncryptConfigTextFunc           _encryptConfigTextFunc;
    private readonly DecryptConfigTextFunc           _decryptConfigTextFunc;
    private readonly FreeCryptographicBufferFunc     _freeCryptographicBufferFunc;
    private readonly FreeLegacySkinInfoFunc          _freeLegacySkinInfoFunc;
    private readonly ReadLegacySkinImageFunc         _readLegacySkinImageFunc;
    private readonly ReadLegacySkinInfoFunc          _readLegacySkinInfoFunc;
#pragma warning restore format

    private readonly FreeLibrarySafeHandle _freeLibrary;
    private bool _disposed;

    public KernelInterop(string libraryPath)
    {
        _freeLibrary = PInvoke.LoadLibrary(libraryPath);

        _initializeGZXKernelModuleFunc =
            PInvoke.GetProcAddress(_freeLibrary, "InitializeGZXKernelModule")
            .CreateDelegate<InitializeGZXKernelModuleFunc>();

        _encryptConfigTextFunc =
            PInvoke.GetProcAddress(_freeLibrary, "EncryptConfigText")
            .CreateDelegate<EncryptConfigTextFunc>();

        _decryptConfigTextFunc =
            PInvoke.GetProcAddress(_freeLibrary, "DecryptConfigText")
            .CreateDelegate<DecryptConfigTextFunc>();

        _freeCryptographicBufferFunc =
            PInvoke.GetProcAddress(_freeLibrary, "FreeCryptographicBuffer")
            .CreateDelegate<FreeCryptographicBufferFunc>();

        _freeLegacySkinInfoFunc =
            PInvoke.GetProcAddress(_freeLibrary, "FreeLegacySkinInfo")
            .CreateDelegate<FreeLegacySkinInfoFunc>();

        _readLegacySkinImageFunc =
            PInvoke.GetProcAddress(_freeLibrary, "ReadLegacySkinImage")
            .CreateDelegate<ReadLegacySkinImageFunc>();

        _readLegacySkinInfoFunc =
            PInvoke.GetProcAddress(_freeLibrary, "ReadLegacySkinInfo")
            .CreateDelegate<ReadLegacySkinInfoFunc>();
    }

    public uint InitializeGZXKernelModule()
    {
        return _initializeGZXKernelModuleFunc();
    }

    public void EncryptConfigText(void* text, ref void* dst)
    {
        _encryptConfigTextFunc(text, ref dst);
    }

    public void DecryptConfigText(void* text, ref void* dst)
    {
        _decryptConfigTextFunc(text, ref dst);
    }

    public void FreeCryptographicBuffer(void* buffer)
    {
        _freeCryptographicBufferFunc(buffer);
    }

    public void FreeLegacySkinInfo(void* skinInfoPtr)
    {
        _freeLegacySkinInfoFunc(skinInfoPtr);
    }

    public uint ReadLegacySkinImage(void* handle, ref void* buffer, ref int length)
    {
        return _readLegacySkinImageFunc(handle, ref buffer, ref length);
    }

    public uint ReadLegacySkinInfo(void* handle, ref void* skinInfoPtr)
    {
        return _readLegacySkinInfoFunc(handle, ref skinInfoPtr);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _freeLibrary.Dispose();
    }

    private unsafe delegate uint InitializeGZXKernelModuleFunc();

    private unsafe delegate void EncryptConfigTextFunc(void* text, ref void* dst);

    private unsafe delegate void DecryptConfigTextFunc(void* text, ref void* dst);

    private unsafe delegate void FreeCryptographicBufferFunc(void* buffer);

    private unsafe delegate void FreeLegacySkinInfoFunc(void* skinInfoPtr);

    private unsafe delegate uint ReadLegacySkinImageFunc(void* handle, ref void* buffer, ref int length);

    private unsafe delegate uint ReadLegacySkinInfoFunc(void* handle, ref void* skinInfoPtr);
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct LegacySkinInfo
{
    public char* name;
    public char* author;
    public char* description;
    public char* datetime;
}
