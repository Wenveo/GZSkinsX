// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.IO;
using System.IO.Hashing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using CommunityToolkit.HighPerformance.Buffers;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Kernel;

using Windows.Win32;

namespace GZSkinsX.Appx.Kernel;

/// <inheritdoc cref="IKernelService"/>
[Shared, Export(typeof(IKernelService))]
internal sealed class KernelService : IKernelService
{
    /// <summary>
    /// 获取在线的模块清单列表。
    /// </summary>
    private readonly Uri[] _moduleManifests =
    [
        new("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/Kernel/ModuleManifest.json"),
        new("http://x1.gzskins.com/MounterV3/Kernel/ModuleManifest.json")
    ];

    /// <summary>
    /// 核心模块根文件夹。
    /// </summary>
    private readonly string _rootFolder = Path.Combine(AppxContext.RoamingFolder, "Kernel");

    /// <summary>
    /// 核心模块文件的存储路径。
    /// </summary>
    private readonly string _modulePath = Path.Combine(AppxContext.RoamingFolder, "Kernel", "GZSkinsX.Kernel.dll");

    /// <summary>
    /// 获取存储在本地的数据配置。
    /// </summary>
    private readonly KernelSettings _settings = AppxContext.Resolve<KernelSettings>();

    /// <summary>
    /// 用于改善创建字符串的开销所用的字符串池。
    /// </summary>
    private readonly StringPool _stringPool = new();

    /// <summary>
    /// 核心模块的互操作包装类。
    /// </summary>
    private KernelInterop? _interop;

    /// <summary>
    /// 尝试获取和加载核心模块，并返回互操作包装类的实例，但如果文件不存在或加载失败则会返回 null。
    /// </summary>
    private KernelInterop? Interop
    {
        get
        {
            if (_interop is not null)
            {
                return _interop;
            }

            var moduleHandle = PInvoke.LoadLibrary(_modulePath);
            if (moduleHandle.IsInvalid is false)
            {
                _interop = new(moduleHandle);
            }

            return _interop;
        }
    }

    /// <inheritdoc/>
    public void InitializeModule()
    {
        Interop?.InitializeGZXKernelModule();
    }

    /// <inheritdoc/>
    public bool EnsureMotClientAlive()
    {
        var interop = Interop;
        if (interop is null)
        {
            return false;
        }

        var ret = interop.EnsureMotClientAlive();
        return Convert.ToBoolean(ret);
    }

    /// <inheritdoc/>
    public unsafe string EncryptConfigText(string str)
    {
        var interop = Interop;
        if (interop is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        var buffer = new nint();
        fixed (char* ch = str)
        {
            interop.EncryptConfigText(ch, ref buffer);
            var value = _stringPool.GetOrAdd(MemoryMarshal.CreateReadOnlySpanFromNullTerminated((char*)buffer));
            interop.FreeCryptographicBuffer(buffer);
            return value;
        }
    }

    /// <inheritdoc/>
    public unsafe string DecryptConfigText(string str)
    {
        var interop = Interop;
        if (interop is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        var buffer = new nint();
        fixed (char* ch = str)
        {
            interop.DecryptConfigText(ch, ref buffer);
            var value = _stringPool.GetOrAdd(MemoryMarshal.CreateReadOnlySpanFromNullTerminated((char*)buffer));
            interop.FreeCryptographicBuffer(buffer);
            return value;
        }
    }

    /// <inheritdoc/>
    public unsafe void ExtractWGZModImage(string input, string output)
    {
        var interop = Interop ?? throw GetModuleNotFoundException();

        var length = 0;
        var buffer = new nint();
        fixed (char* ch = input)
        {
            var ret = interop.ReadLegacySkinImage(ch, ref buffer, ref length);
            if (ret is not 0)
            {
                throw GetKernelInvalidOperationException(ret, input);
            }

            using var unmanagedStream = new UnmanagedMemoryStream((byte*)buffer.ToPointer(), length);
            using var outputStram = new FileStream(output, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            outputStram.Seek(0, default);
            unmanagedStream.CopyTo(outputStram);
            interop.FreeCryptographicBuffer(buffer);
        }
    }

    /// <inheritdoc/>
    public unsafe WGZModInfo ReadWGZModInfo(string filePath)
    {
        var interop = Interop ?? throw GetModuleNotFoundException();
        interop.InitializeGZXKernelModule();

        fixed (char* ch = filePath)
        {
            // 32 = sizeof(NativeModInfo)
            var rawDataSpan = stackalloc byte[32];
            var rawDataPointer = new nint(rawDataSpan);
            var modInfoPtr = (NativeModInfo*)rawDataSpan;

            var ret = interop.ReadLegacySkinInfo(ch, ref rawDataPointer);
            if (ret is not 0)
            {
                throw GetKernelInvalidOperationException(ret, filePath);
            }
            else
            {
                string? name, author, description, datetime;
                name = author = description = datetime = string.Empty;

                var ptr = (*modInfoPtr).name;
                if (ptr != (char*)nint.Zero)
                {
                    name = _stringPool.GetOrAdd(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr));
                }

                ptr = (*modInfoPtr).author;
                if (ptr != (char*)nint.Zero)
                {
                    author = _stringPool.GetOrAdd(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr));
                }

                ptr = (*modInfoPtr).description;
                if (ptr != (char*)nint.Zero)
                {
                    description = _stringPool.GetOrAdd(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr));
                }

                ptr = (*modInfoPtr).datetime;
                if (ptr != (char*)nint.Zero)
                {
                    datetime = _stringPool.GetOrAdd(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr));
                }

                interop.FreeLegacySkinInfo(modInfoPtr);
                return new(name, author, description, datetime);
            }
        }
    }

    /// <summary>
    /// 通过指定的错误代码和文件名称获取具有本地化的错误消息的异常。
    /// </summary>
    private static InvalidOperationException GetKernelInvalidOperationException(uint errorCode, string fileName)
    {
        var format = ResourceHelper.GetLocalized(errorCode switch
        {
            0x80002000 => "GZSkinsX.Appx.Kernel/Resources/Kernel_Exception_CannotToReadContent",
            0x80002001 => "GZSkinsX.Appx.Kernel/Resources/Kernel_Exception_InvalidFileHeader",
            0x80002002 => "GZSkinsX.Appx.Kernel/Resources/Kernel_Exception_ItemNotFound",
            0x80002003 => "GZSkinsX.Appx.Kernel/Resources/Kernel_Exception_UnsupportedFileVersion",
            _ => "GZSkinsX.Appx.Kernel/Resources/Kernel_Exception_Unknown"
        });

        var message = string.Format(format, fileName);
        return new InvalidOperationException(message);
    }

    /// <summary>
    /// 获取具有本地化错误消息的 "模块不存在" 的异常。
    /// </summary>
    private static InvalidOperationException GetModuleNotFoundException()
    {
        return new InvalidOperationException(ResourceHelper.GetLocalized(
            "GZSkinsX.Appx.Kernel/Resources/Interop_Exception_ModuleNotFound"));
    }

    /// <summary>
    /// 验证指定的文件的哈希校验和。
    /// </summary>
    /// <param name="modulePath">输入的文件。</param>
    /// <param name="checksumStr">校验和的字符串值。</param>
    /// <returns>当成功校验目标文件并匹配校验和时返回 true，否则将返回 false。</returns>
    private static bool ValidationChecksum(string modulePath, string checksumStr)
    {
        if (ulong.TryParse(checksumStr, out var checksum))
        {
            try
            {
                var bytes = File.ReadAllBytes(modulePath);
                var result = XxHash64.HashToUInt64(bytes);
                return result == checksum;
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.Kernel.KernelService.ValidationChecksum",
                    $"""
                    Failed to calculate file checksum "{modulePath}".
                    {excp}: "{excp.Message}" {Environment.NewLine}{excp.StackTrace}".
                    """);
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public bool VerifyModuleIntegrity()
    {
        var modulePath = _modulePath;
        if (File.Exists(modulePath) is false)
        {
            return false;
        }

        var moduleChecksum = _settings.ModuleChecksum;
        if (string.IsNullOrWhiteSpace(moduleChecksum))
        {
            return false;
        }

        return ValidationChecksum(modulePath, moduleChecksum);
    }

    /// <inheritdoc/>
    public async Task UpdateManifestAsync()
    {
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
        });

        var manifest = await DownloadModuleManifestAsync(httpClient);
        _settings.ModuleChecksum = manifest.Checksum;
    }

    /// <inheritdoc/>
    public async Task UpdateModuleAsync(IProgress<double>? progress = null)
    {
        using var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
        });

        var tempName = Guid.NewGuid().ToString();
        var tempFile = Path.Combine(AppxContext.TemporaryFolder, tempName);

        try
        {
            var manifest = await DownloadModuleManifestAsync(httpClient);
            using (var outputStream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite))
            {
                await httpClient.DownloadAsync(new(manifest.Path), outputStream, progress);
            }

            if (_interop is not null)
            {
                _interop.ModuleHandle.Dispose();
            }
            else
            {
                if (Directory.Exists(_rootFolder) is false)
                {
                    Directory.CreateDirectory(_rootFolder);
                }
            }

            File.Copy(tempFile, _modulePath, true);
            _settings.ModuleChecksum = manifest.Checksum;
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.Kernel.KernelService.UpdateModuleAsync",
                $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

            throw;
        }
        finally
        {
            try
            {
                File.Delete(tempFile);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// 获取在线的模块清单数据。
    /// </summary>
    private async ValueTask<ModuleManifest> DownloadModuleManifestAsync(HttpClient httpClient)
    {
        foreach (var uri in _moduleManifests)
        {
            var result = await httpClient.GetFromJsonAsync<ModuleManifest>(uri);
            if (ModuleManifest.IsEmpty(result) is false)
            {
                return result;
            }
        }

        throw new IndexOutOfRangeException();
    }

    private record struct ModuleManifest(string Path, string Checksum)
    {
        public static readonly ModuleManifest Empty = new();

        public static bool IsEmpty(in ModuleManifest moduleManifest)
        {
            if (moduleManifest == Empty) return true;
            if (string.IsNullOrWhiteSpace(moduleManifest.Path) is false) return false;
            if (string.IsNullOrWhiteSpace(moduleManifest.Checksum) is false) return false;

            return true;
        }
    }
}
