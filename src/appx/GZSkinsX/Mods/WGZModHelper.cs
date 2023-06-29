// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using CommunityToolkit.HighPerformance.Buffers;

using GZSkinsX.Api.Mods;
using GZSkinsX.Kernel;

using Windows.Storage;

namespace GZSkinsX.Mods;

internal static class WGZModHelper
{
    public static MemoryOwner<byte> ReadModImage(StorageFile storageFile)
    {
        using var handle = storageFile.CreateSafeFileHandle(FileAccess.Read);

        var buffer = new IntPtr();
        var length = 0;

        unsafe
        {
            var ret = KernelInterop.ReadLegacySkinImage(handle.DangerousGetHandle(), &buffer, &length);
            if (ret is not 0)
            {
                ThrowInvalidOperationExceptionForErrorCode(ret);
            }

            var memoryOwner = MemoryOwner<byte>.Allocate(length);
            new Span<byte>((void*)buffer, length).CopyTo(memoryOwner.Span);

            Marshal.FreeHGlobal(buffer);
            return memoryOwner;
        }
    }

    public static WGZModInfo ReadModInfo(StorageFile storageFile)
    {
        using var handle = storageFile.CreateSafeFileHandle(FileAccess.Read);

        unsafe
        {
            var rawDataSpan = stackalloc byte[32];
            var skinInfoPtr = (KernelInterop.LegacySkinInfo*)rawDataSpan;

            var ret = KernelInterop.ReadLegacySkinInfo(handle.DangerousGetHandle(), &skinInfoPtr);
            if (ret is not 0)
            {
                ThrowInvalidOperationExceptionForErrorCode(ret);
            }

            string? name, author, description, datetime;
            name = author = description = datetime = string.Empty;

            int len;
            ReadOnlySpan<char> tmp;

            var namePtr = (*skinInfoPtr).name;
            if (namePtr != s_nullptr)
            {
                len = wsclen(namePtr);
                tmp = new(namePtr, len);

                name = s_stringPool.GetOrAdd(tmp);
                Marshal.FreeHGlobal((nint)namePtr);
            }

            var authorPtr = (*skinInfoPtr).author;
            if (authorPtr != s_nullptr)
            {
                len = wsclen(authorPtr);
                tmp = new(authorPtr, len);

                author = s_stringPool.GetOrAdd(tmp);
                Marshal.FreeHGlobal((nint)authorPtr);
            }

            var descriptionPtr = (*skinInfoPtr).description;
            if (descriptionPtr != s_nullptr)
            {
                len = wsclen(descriptionPtr);
                tmp = new(descriptionPtr, len);

                description = s_stringPool.GetOrAdd(tmp);
                Marshal.FreeHGlobal((nint)descriptionPtr);
            }

            var datetimePtr = (*skinInfoPtr).datetime;
            if (datetimePtr != s_nullptr)
            {
                len = wsclen(datetimePtr);
                tmp = new(datetimePtr, len);

                datetime = s_stringPool.GetOrAdd(tmp);
                Marshal.FreeHGlobal((nint)datetimePtr);
            }

            return new WGZModInfo(name, author, description, datetime);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowInvalidOperationExceptionForErrorCode(int errorCode)
    {
        var message = errorCode switch
        {
            -1 => "未在模组文件中找到有效的模组信息！",
            -2 => "在读取文件时出现了未知的错误。",
            -3 => "无效的文件头标识。",
            -4 => "该版本的模组文件不受支持。",
            _ => "在读取文件时发生了意料之外的错误。"
        };

        throw new InvalidOperationException(message);
    }

    private static readonly unsafe char* s_nullptr = (char*)0UL;

    private static readonly StringPool s_stringPool = new();

    private static readonly MethodInfo s_wcslen = typeof(string).GetMethod("wcslen",
        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);

    private static readonly object[] s_objects = new object[1];

    private static unsafe int wsclen(char* ptr)
    {
        s_objects[0] = (IntPtr)ptr;
        return (int)s_wcslen.Invoke(null, s_objects);
    }
}
