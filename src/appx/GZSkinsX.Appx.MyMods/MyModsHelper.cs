// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using CommunityToolkit.HighPerformance.Buffers;

using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;
using GZSkinsX.DesktopExtension;

using Windows.Storage;

namespace GZSkinsX.Appx.MyMods;

internal static class MyModsHelper
{
    private static StringPool MyStringPool { get; } = new();

    private static KernelInterop? Interop { get; set; }

    private static async Task<KernelInterop?> InitializeKernelModuleAsync()
    {
        if (Interop is not null)
        {
            return Interop;
        }

        try
        {
            var module = await (await ApplicationData.Current.RoamingFolder
                .GetFolderAsync("Kernel")).GetFileAsync("GZSkinsX.Kernel.dll");

            Interop = new KernelInterop(module.Path);

            var ret = Interop.InitializeGZXKernelModule();
            Debug.Assert(ret == 0);

            return Interop;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task<string> EncryptConfigTextAsync(string str)
    {
        var interop = await InitializeKernelModuleAsync();
        if (interop is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        unsafe
        {
            var buffer = (void*)0;
            fixed (char* ch = str)
            {
                interop.EncryptConfigText(ch, ref buffer);
                var value = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(buffer, Count((char*)buffer)));
                interop.FreeCryptographicBuffer(buffer);
                return value;
            }
        }
    }

    public static async Task<string> DecryptConfigTextAsync(string str)
    {
        var interop = await InitializeKernelModuleAsync();
        if (interop is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        unsafe
        {
            var buffer = (void*)0;
            fixed (char* ch = str)
            {
                interop.DecryptConfigText(ch, ref buffer);
                var value = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(buffer, Count((char*)buffer)));
                interop.FreeCryptographicBuffer(buffer);
                return value;
            }
        }
    }

    public static async Task ExtractModImageAsync(string input, string output)
    {
        var interop = await InitializeKernelModuleAsync() ?? throw GetModuleNotFoundException();
        using var fileStream = new FileStream(input, FileMode.Open, FileAccess.Read);

        unsafe
        {
            var buffer = (void*)0;
            var length = 0;

            var ret = interop.ReadLegacySkinImage(
                fileStream.SafeFileHandle.DangerousGetHandle().ToPointer(), ref buffer, ref length);

            if (ret is not 0)
            {
                throw GetKernelInvalidOperationException(ret, input);
            }

            using var unmanagedStream = new UnmanagedMemoryStream((byte*)buffer, length);
            using var outputStram = new FileStream(output, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            outputStram.Seek(0, default);
            unmanagedStream.CopyTo(outputStram);
            interop.FreeCryptographicBuffer(buffer);
        }
    }

    public static async Task<MyModInfo> ReadModInfoAsync(string filePath)
    {
        var interop = await InitializeKernelModuleAsync() ?? throw GetModuleNotFoundException();
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        unsafe
        {
            var rawDataSpan = stackalloc byte[32];
            var cStylePointer = (void*)rawDataSpan;
            var skinInfoPtr = (LegacySkinInfo*)rawDataSpan;

            var ret = interop.ReadLegacySkinInfo(
                fileStream.SafeFileHandle.DangerousGetHandle().ToPointer(), ref cStylePointer);

            if (ret is not 0)
            {
                throw GetKernelInvalidOperationException(ret, filePath);
            }
            else
            {
                string? name, author, description, datetime;
                name = author = description = datetime = string.Empty;

                var ptr = (*skinInfoPtr).name;
                if (ptr != (char*)0)
                {
                    name = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(ptr, Count(ptr)));
                }

                ptr = (*skinInfoPtr).author;
                if (ptr != (char*)0)
                {
                    author = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(ptr, Count(ptr)));
                }

                ptr = (*skinInfoPtr).description;
                if (ptr != (char*)0)
                {
                    description = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(ptr, Count(ptr)));
                }

                ptr = (*skinInfoPtr).datetime;
                if (ptr != (char*)0)
                {
                    datetime = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(ptr, Count(ptr)));
                }

                interop.FreeLegacySkinInfo(skinInfoPtr);
                return new(name, author, description, datetime);
            }
        }
    }

    private static InvalidOperationException GetKernelInvalidOperationException(uint errorCode, string fileName)
    {
        var format = ResourceHelper.GetLocalized(errorCode switch
        {
            0x80002000 => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_CannotToReadContent",
            0x80002002 => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_ItemNotFound",
            0x80002001 => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_InvalidFileHeader",
            0x80002003 => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_UnsupportedFileVersion",
            _ => "GZSkinsX.Appx.MyMods/Resources/Kernel_Exception_Unknown"
        });

        var message = string.Format(format, fileName);
        return new InvalidOperationException(message);
    }

    private static InvalidOperationException GetModuleNotFoundException()
    {
        return new InvalidOperationException(ResourceHelper.GetLocalized(
            "GZSkinsX.Appx.MyMods/Resources/Interop_Exception_ModuleNotFound"));
    }

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
