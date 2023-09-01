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

using GZSkinsX.DesktopExtension.Strings;

using Windows.Storage;
using Windows.Win32;

namespace GZSkinsX.DesktopExtension;

internal sealed partial class DesktopExtensionMethods : IDesktopExtensionMethods
{
    private static StringPool MyStringPool { get; } = new();

    private KernelInterop? _interop;

    private async Task<KernelInterop?> InitializeKernelModule()
    {
        if (_interop is not null)
        {
            return _interop;
        }

        try
        {
            var module = await (await ApplicationData.Current.RoamingFolder
                .CreateFolderAsync("Kernel", CreationCollisionOption.OpenIfExists))
                .CreateFileAsync("GZSkinsX.Kernel.dll", CreationCollisionOption.OpenIfExists);

            _interop = new KernelInterop(module.Path);

            var ret = _interop.InitializeGZXKernelModule();
            Debug.Assert(ret == 0);

            return _interop;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<string> EncryptConfigText(string str)
    {
        var interop = await InitializeKernelModule();
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
                interop.EncryptConfigText(ch, &buffer);
                var value = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(buffer, Count((char*)buffer)));
                interop.FreeCryptographicBuffer(buffer);
                return value;
            }
        }
    }

    public async Task<string> DecryptConfigText(string str)
    {
        var interop = await InitializeKernelModule();
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
                interop.DecryptConfigText(ch, &buffer);
                var value = MyStringPool.GetOrAdd(new ReadOnlySpan<char>(buffer, Count((char*)buffer)));
                interop.FreeCryptographicBuffer(buffer);
                return value;
            }
        }
    }

    public async Task ExtractModImage(string input, string output)
    {
        var interop = await InitializeKernelModule() ?? throw GetModuleNotFoundException();
        using var fileStream = new FileStream(input, FileMode.Open, FileAccess.Read);

        unsafe
        {
            var buffer = (void*)0;
            var length = 0;

            var ret = interop.ReadLegacySkinImage(
                fileStream.SafeFileHandle.DangerousGetHandle().ToPointer(), &buffer, &length);

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

    public async Task<ModInfo> ReadModInfo(string filePath)
    {
        var interop = await InitializeKernelModule() ?? throw GetModuleNotFoundException();
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        unsafe
        {
            var rawDataSpan = stackalloc byte[32];
            var skinInfoPtr = (LegacySkinInfo*)rawDataSpan;

            var ret = interop.ReadLegacySkinInfo(
                fileStream.SafeFileHandle.DangerousGetHandle().ToPointer(), (void**)&rawDataSpan);

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
        var format = errorCode switch
        {
            0x80002000 => Resource.Kernel_Exception_CannotToReadContent,
            0x80002002 => Resource.Kernel_Exception_ItemNotFound,
            0x80002001 => Resource.Kernel_Exception_InvalidFileHeader,
            0x80002003 => Resource.Kernel_Exception_UnsupportedFileVersion,
            _ => Resource.Kernel_Exception_Unknown
        };

        var message = string.Format(format, fileName);
        return new InvalidOperationException(message);
    }

    private static InvalidOperationException GetModuleNotFoundException()
    {
        return new InvalidOperationException(Resource.Interop_Exception_ModuleNotFound);
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

    public Task<bool> IsMTRunning()
    {
        using var handle = PInvoke.OpenFileMapping(0xF001F, false, "Gz_services:execute");
        return Task.FromResult(handle.IsInvalid is false);
    }

    public Task<bool> EnsureEfficiencyMode(int processId)
    {
        var result = false;
        try
        {
            var process = Process.GetProcessById(processId);
            result = EfficiencyManager.EnsureEfficiencyMode(process.Handle);
        }
        catch
        {
        }

        return Task.FromResult(result);
    }

    public Task<bool> ProcessLaunch(string executable, string args, bool runAs)
    {
        var startInfo = new ProcessStartInfo
        {
            Arguments = args,
            FileName = executable,
            UseShellExecute = true,
            Verb = runAs ? "RunAs" : string.Empty,
            WorkingDirectory = Path.GetDirectoryName(executable)
        };

        var taskCompletionSource = new TaskCompletionSource<bool>();

        try
        {
            using var process = Process.Start(startInfo);
            taskCompletionSource.SetResult(process.Handle != IntPtr.Zero);
        }
        catch (Exception excp)
        {
            taskCompletionSource.SetException(excp);
        }

        return taskCompletionSource.Task;
    }

    public Task SetEfficiencyMode(int processId, bool isEnable)
    {
        try
        {
            var process = Process.GetProcessById(processId);
            EfficiencyManager.SetEfficiencyMode(process.Handle, isEnable);
        }
        catch
        {
        }

        return Task.CompletedTask;
    }

    public Task SetOwner(int processId)
    {
        try
        {
            var owner = Process.GetProcessById(processId);
            owner.EnableRaisingEvents = true;
            owner.Exited += (_, _) => SafeExit();
        }
        catch
        {

        }

        return Task.CompletedTask;
    }

    public Task<bool> SetWindowText(long windowHandle, string newTitle)
    {
        if (windowHandle is not 0)
        {
            var ret = PInvoke.SetWindowText(new(new(windowHandle)), newTitle);
            return Task.FromResult(ret == 0);
        }

        return Task.FromResult(false);
    }

    internal void SafeExit()
    {
        _interop?.Dispose();
        Program.Exit(0);
    }
}
