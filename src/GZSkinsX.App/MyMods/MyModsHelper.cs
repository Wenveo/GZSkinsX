// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.IO;
using System.Runtime.CompilerServices;

using CommunityToolkit.HighPerformance.Buffers;

using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;
using GZSkinsX.Kernel;

using Windows.Storage;

namespace GZSkinsX.MyMods;

internal static class MyModsHelper
{
    public static string EncryptConfigText(string str)
    {
        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        unsafe
        {
            var buffer = (void*)0;
            fixed (char* ch = str)
            {
                KernelInterop.EncryptConfigText(ch, &buffer);
                var value = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(buffer, Count((char*)buffer)));
                KernelInterop.FreeCryptographicBuffer(buffer);
                return value;
            }
        }
    }

    public static string DecryptConfigText(string str)
    {
        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        unsafe
        {
            var buffer = (void*)0;
            fixed (char* ch = str)
            {
                KernelInterop.DecryptConfigText(ch, &buffer);
                var value = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(buffer, Count((char*)buffer)));
                KernelInterop.FreeCryptographicBuffer(buffer);
                return value;
            }
        }
    }

    public static MemoryOwner<byte> ReadModImage(StorageFile storageFile)
    {
        using var handle = storageFile.CreateSafeFileHandle(FileAccess.Read);

        unsafe
        {
            var buffer = (void*)0;
            var length = 0;

            var ret = KernelInterop.ReadLegacySkinImage(handle.DangerousGetHandle().ToPointer(), &buffer, &length);
            if (ret is not 0)
            {
                ThrowInvalidOperationException(ret, storageFile);
            }

            var memoryOwner = MemoryOwner<byte>.Allocate(length);
            new Span<byte>(buffer, length).CopyTo(memoryOwner.Span);

            KernelInterop.FreeCryptographicBuffer(buffer);
            return memoryOwner;
        }
    }

    public static MyModInfo ReadModInfo(StorageFile storageFile)
    {
        using var handle = storageFile.CreateSafeFileHandle(FileAccess.Read);

        unsafe
        {
            var rawDataSpan = stackalloc byte[32];
            var skinInfoPtr = (KernelInterop.LegacySkinInfo*)rawDataSpan;

            var ret = KernelInterop.ReadLegacySkinInfo(handle.DangerousGetHandle().ToPointer(), (void**)&rawDataSpan);
            if (ret is not 0)
            {
                ThrowInvalidOperationException(ret, storageFile);
            }

            string? name, author, description, datetime;
            name = author = description = datetime = string.Empty;

            int len;
            ReadOnlySpan<char> tmp;

            var ptr = (*skinInfoPtr).name;
            if (ptr != (char*)0)
            {
                len = Count(ptr);
                tmp = new(ptr, len);

                name = MyStringPool.GetOrAdd(tmp);
            }

            ptr = (*skinInfoPtr).author;
            if (ptr != (char*)0)
            {
                len = Count(ptr);
                tmp = new(ptr, len);

                author = MyStringPool.GetOrAdd(tmp);
            }

            ptr = (*skinInfoPtr).description;
            if (ptr != (char*)0)
            {
                len = Count(ptr);
                tmp = new(ptr, len);

                description = MyStringPool.GetOrAdd(tmp);
            }

            ptr = (*skinInfoPtr).datetime;
            if (ptr != (char*)0)
            {
                len = Count(ptr);
                tmp = new(ptr, len);

                datetime = MyStringPool.GetOrAdd(tmp);
            }

            KernelInterop.FreeLegacySkinInfo(skinInfoPtr);
            return new MyModInfo(name, author, description, datetime);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowInvalidOperationException(uint errorCode, StorageFile file)
    {
        var format = ResourceHelper.GetLocalized(errorCode switch
        {
            0x80002000 => "Resources/MyModsHelper_Exception_CannotToReadContent",
            0x80002002 => "Resources/MyModsHelper_Exception_ItemNotFound",
            0x80002001 => "Resources/MyModsHelper_Exception_InvalidFileHeader",
            0x80002003 => "Resources/MyModsHelper_Exception_UnsupportedFileVersion",
            _ => "Resources/MyModsHelper_Exception_Unknown"
        });

        var message = string.Format(format, file.Name);
        throw new InvalidOperationException(message);
    }

    private static StringPool MyStringPool { get; } = new();

    private static unsafe int Count(char* ch)
    {
        var count = 0;
        while (*ch++ != char.MinValue)
        {
            count++;
        }

        return count;
    }
}
