// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Runtime.CompilerServices;

using CommunityToolkit.HighPerformance.Buffers;

using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;

namespace GZSkinsX.Appx.MyMods;

internal static class MyModsHelper
{
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "wcslen")]
    private static extern unsafe int wcslen(string? _, char* ptr);

    private static StringPool MyStringPool { get; } = new();

    public static string EncryptConfigText(string str)
    {
        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        unsafe
        {
            var buffer = new nint();
            fixed (char* ch = str)
            {
                KernelInterop.EncryptConfigText(ch, ref buffer);
                var value = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(buffer.ToPointer(), wcslen(null, (char*)buffer)));
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
            var buffer = new nint();
            fixed (char* ch = str)
            {
                KernelInterop.DecryptConfigText(ch, ref buffer);
                var value = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(buffer.ToPointer(), wcslen(null, (char*)buffer)));
                KernelInterop.FreeCryptographicBuffer(buffer);
                return value;
            }
        }
    }

    public static void ExtractModImage(string input, string output)
    {
        unsafe
        {
            var length = 0;
            var buffer = new nint();

            fixed (char* ch = input)
            {
                var ret = KernelInterop.ReadLegacySkinImage(ch, ref buffer, ref length);

                if (ret is not 0)
                {
                    throw GetKernelInvalidOperationException(ret, input);
                }

                using var unmanagedStream = new UnmanagedMemoryStream((byte*)buffer.ToPointer(), length);
                using var outputStram = new FileStream(output, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

                outputStram.Seek(0, default);
                unmanagedStream.CopyTo(outputStram);
                KernelInterop.FreeCryptographicBuffer(buffer);
            }
        }
    }

    public static MyModInfo ReadModInfo(string filePath)
    {
        KernelInterop.InitializeGZXKernelModule();

        unsafe
        {
            fixed (char* ch = filePath)
            {
                var rawDataSpan = stackalloc byte[32];
                var cStylePointer = (nint)rawDataSpan;
                var skinInfoPtr = (KernelInterop.LegacySkinInfo*)rawDataSpan;

                var ret = KernelInterop.ReadLegacySkinInfo(ch, ref cStylePointer);
                if (ret is not 0)
                {
                    throw GetKernelInvalidOperationException(ret, filePath);
                }
                else
                {
                    string? name, author, description, datetime;
                    name = author = description = datetime = string.Empty;

                    var ptr = (*skinInfoPtr).name;
                    if (ptr != (char*)nint.Zero)
                    {
                        name = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(ptr, wcslen(null, ptr)));
                    }

                    ptr = (*skinInfoPtr).author;
                    if (ptr != (char*)nint.Zero)
                    {
                        author = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(ptr, wcslen(null, ptr)));
                    }

                    ptr = (*skinInfoPtr).description;
                    if (ptr != (char*)nint.Zero)
                    {
                        description = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(ptr, wcslen(null, ptr)));
                    }

                    ptr = (*skinInfoPtr).datetime;
                    if (ptr != (char*)nint.Zero)
                    {
                        datetime = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(ptr, wcslen(null, ptr)));
                    }

                    KernelInterop.FreeLegacySkinInfo(skinInfoPtr);
                    return new(name, author, description, datetime);
                }
            }
        }
    }

    private static InvalidOperationException GetKernelInvalidOperationException(uint errorCode, string fileName)
    {
        var format = ResourceHelper.GetLocalized(errorCode switch
        {
            0x80002000 => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_CannotToReadContent",
            0x80002001 => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_InvalidFileHeader",
            0x80002002 => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_ItemNotFound",
            0x80002003 => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_UnsupportedFileVersion",
            _ => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_Unknown"
        });

        var message = string.Format(format, fileName);
        return new InvalidOperationException(message);
    }
}
