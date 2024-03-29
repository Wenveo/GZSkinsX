// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MotClient;

using Windows.Foundation;

namespace GZSkinsX.Appx.MotClient;

/// <inheritdoc cref="IMotClientAgentService"/>
[Shared, Export(typeof(IMotClientService))]
internal sealed partial class MotClientAgentService : IMotClientService
{
    /// <summary>
    /// 获取存放在线的包清单配置链接。
    /// </summary>
    private static Uri[] OnlineManifests { get; } =
    [
        new("http://pan.x1.skn.lol/d/%20PanGZSkinsX/MounterV3/PackageManifest.json"),
        new("http://x1.gzskins.com/MounterV3/PackageManifest.json")
    ];

    /// <summary>
    /// 用于计算文件路径哈希的 XxHash3 算法实例。
    /// </summary>
    private readonly XxHash64 _xxHash64 = new();

    /// <summary>
    /// 用于计算文件校验和的 XxHash3 算法实例。
    /// </summary>
    private readonly XxHash3 _xxHash3 = new();

    /// <summary>
    /// 获取用于存放服务组件的根文件夹路径。
    /// </summary>
    private string MotClientRootFolder { get; }

    /// <summary>
    /// 获取用于存放当前服务上下文内容的本地配置。
    /// </summary>
    private MotClientSettings MotClientSettings { get; }

    /// <summary>
    /// 获取用于 HTTP 请求的 <see cref="HttpClient"/> 实例。
    /// </summary>
    private HttpClient MyHttpClient { get; }

    /// <inheritdoc/>
    public bool IsMTRunning
    {
        get => AppxContext.KernelService.EnsureMotClientAlive();
    }

    /// <inheritdoc/>
    public event TypedEventHandler<IMotClientService, bool>? IsRunningChanged;

    /// <summary>
    /// 初始化 <see cref="MotClientAgentService"/> 的新实例。
    /// </summary>
    public MotClientAgentService()
    {
        MotClientSettings = AppxContext.Resolve<MotClientSettings>();
        MyHttpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
        });
        MotClientRootFolder = Path.Combine(AppxContext.RoamingFolder, "MotClient");

        var worker = new BackgroundWorker();
        worker.DoWork += DoSomething;
        worker.RunWorkerAsync();
    }

    // ヾ(•ω•`)o
    private async void DoSomething(object? sender, DoWorkEventArgs e)
    {
        async Task CheckExitAsync()
        {
            while (true)
            {
                await Task.Delay(1000);

                try
                {
                    if (IsMTRunning is false)
                    {
                        IsRunningChanged?.Invoke(this, false);
                        AppxContext.LoggingService.LogAlways("GZSkinsX.Appx.MotClient.MotClientService.CheckExitAsync", "MotClientAgent has exited.");
                        break;
                    }
                }
                catch
                {
                }
            }

            await Task.CompletedTask;
        }

        async Task CheckRunAsync()
        {
            while (true)
            {
                await Task.Delay(1000);

                try
                {
                    if (IsMTRunning)
                    {
                        IsRunningChanged?.Invoke(this, true);
                        AppxContext.LoggingService.LogAlways("GZSkinsX.Appx.MotClient.MotClientService.CheckRunAsync", "MotClientAgent is running.");
                        break;
                    }
                }
                catch
                {
                }
            }

            await Task.CompletedTask;
        }

        while (true)
        {
            try
            {
                if (IsMTRunning)
                {
                    IsRunningChanged?.Invoke(this, true);
                    AppxContext.LoggingService.LogAlways("GZSkinsX.Appx.MotClient.MotClientService.DoSomething", "MotClientAgent is running.");
                    await CheckExitAsync();
                }
                else
                {
                    IsRunningChanged?.Invoke(this, false);
                    AppxContext.LoggingService.LogAlways("GZSkinsX.Appx.MotClient.MotClientService.DoSomething", "MotClientAgent is not running.");
                    await CheckRunAsync();
                }
            }
            catch
            {
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CheckForUpdatesAsync()
    {
        var metadata = await TryGetCurrentPackageMetadataAsync(nameof(MTPackageMetadata.Version));
        if (metadata is not null)
        {
            try
            {
                var onlineManifest = await DownloadPackageManifestAsync();
                if (StringComparer.Ordinal.Equals(metadata.Version, onlineManifest.Version))
                {
                    AppxContext.LoggingService.LogOkay("GZSkinsX.Appx.MotClient.MotClientService.CheckForUpdatesAsync", $"Now is uptodate \"{metadata.Version}\".");
                    return false;
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MotClient.MotClientService.CheckForUpdatesAsync",
                    $"""
                    Failed to check updates.
                    {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}
                    """);

                throw;
            }
        }

        AppxContext.LoggingService.LogAlways("GZSkinsX.Appx.MotClient.MotClientService.CheckForUpdatesAsync", $"Attention needed.");
        return true;
    }

    /// <inheritdoc/>
    public async Task LaunchAsync()
    {
        if (TryGetMotClientAgentWorkingDirectory(out var workingDirectory) is false)
        {
            return;
        }

        var localPackageMetadata =
            await TryGetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile),
                nameof(MTPackageMetadata.ProcStartupArgs));

        if (localPackageMetadata is not null)
        {
            var executableFile = Path.Combine(workingDirectory, localPackageMetadata.ExecutableFile);
            try
            {
                ProcessLaunch(executableFile, localPackageMetadata.ProcStartupArgs);
                AppxContext.LoggingService.LogOkay("GZSkinsX.Appx.MotClient.MotClientService.LaunchAsync", "Launch successfully.");
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MotClient.MotClientService.LaunchAsync",
                    $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

                throw;
            }
        }
    }

    /// <inheritdoc/>
    public async Task LaunchAsync(string args)
    {
        if (TryGetMotClientAgentWorkingDirectory(out var workingDirectory) is false)
        {
            return;
        }

        var localPackageMetadata =
            await TryGetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile));

        if (localPackageMetadata is not null)
        {
            var executableFile = Path.Combine(workingDirectory, localPackageMetadata.ExecutableFile);
            try
            {
                ProcessLaunch(executableFile, args);
                AppxContext.LoggingService.LogOkay("GZSkinsX.Appx.MotClient.MotClientService.LaunchAsync2", "Launch successfully.");
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MotClient.MotClientService.LaunchAsync2",
                    $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

                throw;
            }
        }
    }

    /// <inheritdoc/>
    public async Task TerminateAsync()
    {
        if (TryGetMotClientAgentWorkingDirectory(out var workingDirectory) is false)
        {
            return;
        }

        var localPackageMetadata =
            await TryGetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.ExecutableFile),
                nameof(MTPackageMetadata.ProcTerminateArgs));

        if (localPackageMetadata is not null)
        {
            var executableFile = Path.Combine(workingDirectory, localPackageMetadata.ExecutableFile);
            try
            {
                ProcessLaunch(executableFile, localPackageMetadata.ProcTerminateArgs);
                AppxContext.LoggingService.LogOkay("GZSkinsX.Appx.MotClient.MotClientService.TerminateAsync", "Launch successfully.");
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MotClient.MotClientService.TerminateAsync",
                    $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");

                throw;
            }
        }
    }

    /// <inheritdoc/>
    public async Task<MTPackageMetadata?> TryGetCurrentPackageMetadataAsync(params string[] filter)
    {
        if (TryGetMotClientAgentWorkingDirectory(out var workingDirectory))
        {
            return await TryGetLocalMTPackageMetadataAsync(workingDirectory, filter);
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public bool TryGetMotClientAgentWorkingDirectory([NotNullWhen(true)] out string? result)
    {
        var workingDirectory = MotClientSettings.WorkingDirectory;
        if (Directory.Exists(workingDirectory) is false)
        {
            result = null;
            return false;
        }

        result = workingDirectory;
        return true;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(IProgress<double>? progress = null)
    {
        var checkVersion = false;
        MTPackageMetadata? previousMetadata = null;
        if (TryGetMotClientAgentWorkingDirectory(out var workingDirectory))
        {
            previousMetadata = await TryGetLocalMTPackageMetadataAsync(workingDirectory,
                nameof(MTPackageMetadata.Version), nameof(MTPackageMetadata.SettingsFile));

            if (await VerifyContentIntegrityCoreAsync(workingDirectory))
            {
                // 当校验完包内容后
                // 再次检查组件版本
                checkVersion = true;
            }
        }

        var onlineManifest = await DownloadPackageManifestAsync();
        if (checkVersion && previousMetadata is not null)
        {
            if (StringComparer.Ordinal.Equals(previousMetadata.Version, onlineManifest.Version))
            {
                AppxContext.LoggingService.LogAlways(
                    "GZSkinsX.Appx.MotClient.MotClientService.UpdateAsync",
                    "This component is already up to date.");

                return;
            }
        }

        var destFolder = await DownloadMTPackageAsync(new(onlineManifest.Path!), new((value) => progress?.Report(value * 100)));
        if (workingDirectory is not null && previousMetadata is not null)
        {
            var newMetadata = await TryGetLocalMTPackageMetadataAsync(destFolder, nameof(MTPackageMetadata.SettingsFile));
            if (newMetadata is not null)
            {
                // Copy settings file
                var settingsFilePath = Path.Combine(workingDirectory, previousMetadata.SettingsFile);
                if (File.Exists(settingsFilePath))
                {
                    File.Copy(settingsFilePath, Path.Combine(destFolder, newMetadata.SettingsFile), true);
                }
            }
        }

        // 更新当前服务组件的工作目录
        MotClientSettings.WorkingDirectory = destFolder;

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.Appx.MotClient.MotClientService.UpdateAsync",
            $"The component has been updated to \"{onlineManifest.Version}\".");

        TryCleanupMounterRootFolder();
    }

    /// <inheritdoc/>
    public async Task<bool> VerifyContentIntegrityAsync()
    {
        if (TryGetMotClientAgentWorkingDirectory(out var workingDirectory) is false)
        {
            return false;
        }

        return await VerifyContentIntegrityCoreAsync(workingDirectory);
    }

    /// <inheritdoc cref="VerifyContentIntegrityAsync"/>
    private async Task<bool> VerifyContentIntegrityCoreAsync(string workingDirectory)
    {
        var blockmapFile = Path.Combine(workingDirectory, "_metadata", "blockmap.json");
        if (File.Exists(blockmapFile) is false)
        {
            return false;
        }

        FileStream? fileStream = null;
        MTPackageBlockMap? localBlockMap;
        try
        {
            fileStream = new FileStream(blockmapFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            localBlockMap = await JsonSerializer.DeserializeAsync<MTPackageBlockMap>(fileStream);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MotClient.MotClientService.VerifyContentIntegrityCoreAsync",
                $"""
                    Failed to deserialize block map json "{blockmapFile}".
                    {excp}: "{excp.Message}" {Environment.NewLine}{excp.StackTrace}".
                    """);

            return false;
        }
        finally
        {
            fileStream?.Dispose();
        }

        if (localBlockMap is null || localBlockMap.Blocks is null)
        {
            return false;
        }

        var hashToChecksum = localBlockMap.Blocks.ToFrozenDictionary(
            a => ulong.Parse(a.Hash, CultureInfo.InvariantCulture),
            b => ulong.Parse(b.Checksum, CultureInfo.InvariantCulture));

        var parentFolderPathLength = workingDirectory.Length + 1;
        var hashToPath = Directory.EnumerateFiles(workingDirectory, "*", SearchOption.AllDirectories)
            .ToFrozenDictionary(elementSelector: path => path, keySelector: path =>
            {
                var relativePath = path[parentFolderPathLength..].Replace('\\', '/');

                _xxHash64.Reset();
                _xxHash64.Append(Encoding.UTF8.GetBytes(relativePath));
                return _xxHash64.GetCurrentHashAsUInt64();
            });


        foreach (var item in hashToChecksum)
        {
            if (hashToPath.TryGetValue(item.Key, out var path) is false)
            {
                return false;
            }

            try
            {
                fileStream = new FileStream(path, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite, 4096, true);

                _xxHash3.Reset();
                await _xxHash3.AppendAsync(fileStream);

                if (_xxHash3.GetCurrentHashAsUInt64() != item.Value)
                {
                    return false;
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MotClient.MotClientService.VerifyContentIntegrityAsync",
                    $"""
                    Failed to calculate file checksum "{path}".
                    {excp}: "{excp.Message}" {Environment.NewLine}{excp.StackTrace}".
                    """);

                return false;
            }
            finally
            {
                fileStream?.Dispose();
            }
        }

        return true;
    }

    /// <summary>
    /// 从在线的服务器中检索和下载包清单配置。
    /// </summary>
    /// <param name="progress">用于报告下载进度。</param>
    /// <returns>当成功下载包清单时会将其反序列化并返回，但如果出现诸如网络无法访问等原因，将会抛出异常。</returns>
    /// <exception cref="IndexOutOfRangeException">当尝试从所有可能的在线的服务器中检索和下载无果时将会抛出此异常。</exception>
    private async Task<MTPackageManifest> DownloadPackageManifestAsync(Progress<double>? progress = null)
    {
        using var memoryStream = new MemoryStream();
        foreach (var uri in OnlineManifests)
        {
            try
            {
                await MyHttpClient.DownloadAsync(uri, memoryStream, progress);
                memoryStream.Seek(0, SeekOrigin.Begin);

                if (await JsonSerializer.DeserializeAsync<MTPackageManifest>(memoryStream) is { } result &&
                    !string.IsNullOrWhiteSpace(result.Path) && !string.IsNullOrWhiteSpace(result.Version))
                {
                    return result;
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MotClient.MotClientService.DownloadPackageManifestAsync",
                    $"{excp}: \"{excp.Message}\". {Environment.NewLine}{excp.StackTrace}.");
            }

            Array.Fill<byte>(memoryStream.GetBuffer(), 0x20);
            memoryStream.Seek(0, SeekOrigin.Begin);
        }

        throw new IndexOutOfRangeException();
    }

    /// <summary>
    /// 从指定的配置链接中检索和下载组件包。
    /// </summary>
    /// <param name="requestUri">请求的下载链接。</param>
    /// <param name="progress">用于报告下载进度。</param>
    /// <returns>存放解压后的组件包文件目录。</returns>
    private async Task<string> DownloadMTPackageAsync(Uri requestUri, Progress<double>? progress = null)
    {
        var tempName = Guid.NewGuid().ToString();
        var tempFile = Path.Combine(AppxContext.TemporaryFolder, tempName);

        try
        {
            using (var outputStream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite))
            {
                await MyHttpClient.DownloadAsync(requestUri, outputStream, progress);
            }

            var destFolderPath = Path.Combine(MotClientRootFolder, tempName);
            if (Directory.Exists(destFolderPath) is false)
            {
                Directory.CreateDirectory(destFolderPath);
            }

            ZipFile.ExtractToDirectory(tempFile, destFolderPath);
            return destFolderPath;
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MotClient.MotClientService.DownloadMTPackageAsync",
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
    /// 尝试从指定的组件目录中检索包元数据配置。
    /// </summary>
    /// <param name="workingDirectory">目标组件的工作 (根) 目录。</param>
    /// <param name="filter">筛选并获取指定成员的值，默认将获取所有成员的值。</param>
    /// <returns>如果成功获取到目标组件的包元数据便会将其返回，否则将返回 null。</returns>
    private static async Task<MTPackageMetadata?> TryGetLocalMTPackageMetadataAsync(string workingDirectory, params string[] filter)
    {
        var metadataFile = Path.Combine(workingDirectory, "_metadata", "package.json");
        if (File.Exists(metadataFile) is false)
        {
            return null;
        }

        try
        {
            return ParseMTPackageMetadataFromBytes(await File.ReadAllBytesAsync(metadataFile), filter);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.App.MotClient.MotClientService.TryGetLocalMTPackageMetadata",
                $"""
                Failed to parse mt package data. "{excp.Message}".
                "{excp}: {excp.StackTrace}."
                """);

            return null;
        }
    }

    /// <summary>
    /// 从指定的字节数据中解析包元数据。
    /// </summary>
    /// <param name="jsonData">包元数据的字节数据。</param>
    /// <param name="filter">筛选并获取指定成员的值，默认将获取所有成员的值。</param>
    /// <returns>从字节数据中解析的 <see cref="MTPackageMetadata"/> 数据对象实例。</returns>
    private static MTPackageMetadata ParseMTPackageMetadataFromBytes(ReadOnlySpan<byte> jsonData, params string[] filter)
    {
        bool AllowInclude(string propName)
        {
            if (filter.Length is 0)
            {
                return true;
            }

            return filter.Contains(propName, StringComparer.OrdinalIgnoreCase);
        }

        bool TryGetValue(ref Utf8JsonReader jsonReader, ref string? field, string propName)
        {
            if (jsonReader.ValueTextEquals(propName) && AllowInclude(propName))
            {
                if (jsonReader.Read())
                {
                    field = jsonReader.GetString() ?? string.Empty;
                    return true;
                }
            }

            return false;
        }

        bool TryGetStartUpArgs(ref Utf8JsonReader jsonReader, ref MTPackageMetadataStartUpArgument[] array, string propName)
        {
            if (jsonReader.ValueTextEquals(propName) is false || AllowInclude(propName) is false)
            {
                return false;
            }

            var list = new List<MTPackageMetadataStartUpArgument>();
            if (jsonReader.Read() && jsonReader.TokenType is JsonTokenType.StartArray)
            {
                string? name = null, value = null;
                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType is not JsonTokenType.StartObject)
                    {
                        name = null;
                        value = null;
                        continue;
                    }

                    if (jsonReader.TokenType is JsonTokenType.PropertyName)
                    {
                        if (jsonReader.ValueTextEquals("Name"))
                        {
                            if (jsonReader.Read())
                            {
                                name = jsonReader.GetString();
                            }
                        }
                        else if (jsonReader.ValueTextEquals("Value"))
                        {
                            if (jsonReader.Read())
                            {
                                value = jsonReader.GetString();
                            }
                        }
                    }

                    if (jsonReader.TokenType is not JsonTokenType.EndObject)
                    {
                        if (name is not null && value is not null)
                        {
                            list.Add(new(name, value));
                        }
                    }
                }
            }

            array = [.. list];
            return true;
        }

        var otherStartupArgs = Array.Empty<MTPackageMetadataStartUpArgument>();
        string? author, version, description, aboutTheAuthor, settingsFile;
        string? executableFile, procStartupArgs, procTerminateArgs;

        author = version = description = aboutTheAuthor = settingsFile = null;
        executableFile = procStartupArgs = procTerminateArgs = null;

        var jsonReader = new Utf8JsonReader(jsonData);
        while (jsonReader.Read())
        {
            if (jsonReader.TokenType is not JsonTokenType.PropertyName) { continue; }
            if (TryGetValue(ref jsonReader, ref author, nameof(MTPackageMetadata.Author))) { continue; }
            if (TryGetValue(ref jsonReader, ref version, nameof(MTPackageMetadata.Version))) { continue; }
            if (TryGetValue(ref jsonReader, ref description, nameof(MTPackageMetadata.Description))) { continue; }
            if (TryGetValue(ref jsonReader, ref aboutTheAuthor, nameof(MTPackageMetadata.AboutTheAuthor))) { continue; }
            if (TryGetValue(ref jsonReader, ref settingsFile, nameof(MTPackageMetadata.SettingsFile))) { continue; }
            if (TryGetValue(ref jsonReader, ref executableFile, nameof(MTPackageMetadata.ExecutableFile))) { continue; }
            if (TryGetValue(ref jsonReader, ref procStartupArgs, nameof(MTPackageMetadata.ProcStartupArgs))) { continue; }
            if (TryGetValue(ref jsonReader, ref procTerminateArgs, nameof(MTPackageMetadata.ProcTerminateArgs))) { continue; }
            if (TryGetStartUpArgs(ref jsonReader, ref otherStartupArgs, nameof(MTPackageMetadata.OtherStartupArgs))) { continue; }
        }

        return new MTPackageMetadata(author ?? string.Empty, version ?? string.Empty, description ?? string.Empty,
            aboutTheAuthor ?? string.Empty, settingsFile ?? string.Empty, executableFile ?? string.Empty,
            procStartupArgs ?? string.Empty, procTerminateArgs ?? string.Empty, otherStartupArgs);
    }

    /// <summary>
    /// 通过指定的可执行程序路径和启动参数去启动新的进程。
    /// </summary>
    /// <param name="filePath">可执行程序的完整路径。</param>
    /// <param name="args">程序的启动参数。</param>
    private static void ProcessLaunch(string filePath, string args)
    {
        // Bug in .NET 8 RC1
        // See https://github.com/dotnet/runtime/issues/92046

        Process.Start(new ProcessStartInfo
        {
            Arguments = args,
            FileName = filePath,
            UseShellExecute = true,
            CreateNoWindow = true,
            Verb = "RunAs",
            WorkingDirectory = Path.GetDirectoryName(filePath)
        });
    }

    /// <summary>
    /// 尝试清理在存放组件的根目录中的多余文件/文件夹。
    /// </summary>
    private void TryCleanupMounterRootFolder()
    {
        if (Directory.Exists(MotClientRootFolder) is false)
        {
            return;
        }

        var targetFolderFullName = MotClientSettings.WorkingDirectory;
        foreach (var item in new DirectoryInfo(MotClientRootFolder).EnumerateFileSystemInfos())
        {
            // 当匹配到与工作目录相同的文件夹时，跳过删除操作
            // 除此之外，其余的所有文件/文件夹都将被删除。
            var dirInfo = item as DirectoryInfo;
            if (dirInfo is not null && StringComparer.Ordinal.Equals(dirInfo.FullName, targetFolderFullName))
            {
                continue;
            }

            try
            {
                if (dirInfo is not null)
                {
                    // 强制删除文件夹以及里面的文件
                    dirInfo.Delete(true);
                }
                else
                {
                    item.Delete();
                }
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MotClient.MotClientService.TryCleanupMounterRootFolder",
                    $"Failed to delete the storage item ({item.Name}): \"{excp.Message}\".");
            }
        }
    }

    private sealed record class MTPackageManifest(string? Path, string? Version) { }

    private sealed record class MTPackageBlockMap(IList<MTPackageBlockEntry>? Blocks) { }

    private record struct MTPackageBlockEntry(string Hash, string Checksum) { }
}
