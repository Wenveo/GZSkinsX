// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Threading.Tasks;

using MessagePack;
using MessagePack.Resolvers;

using Microsoft.VisualStudio.Composition;

namespace GZSkinsX.Composition.Cache;

/// <summary>
/// 组件缓存写入器，可将 <see cref="AssemblyCatalogV2Cache"/> 和 <see cref="IExportProviderFactory"/> 的实例写出到缓存流。
/// </summary>
internal sealed class CacheStreamWriter : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// 当前的 <see cref="AssemblyCatalogV2Cache"/> 缓存流。
    /// </summary>
    private readonly FileStream _assemblyCatalogCacheStream;

    /// <summary>
    /// 当前的 <see cref="CompositionConfiguration"/> 缓存流。
    /// </summary>
    private readonly FileStream _compositionCacheStream;

    /// <summary>
    /// 用于判断当前类是否调用过 Dispose 或 DisposeAsync。
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// 初始化 <see cref="CacheStreamWriter"/> 的新实例。
    /// </summary>
    public CacheStreamWriter()
    {
        _assemblyCatalogCacheStream = new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite);
        _compositionCacheStream = new FileStream(Path.GetTempFileName(), FileMode.Open, FileAccess.ReadWrite);
    }

    /// <summary>
    /// 将传入的 <see cref="AssemblyCatalogV2Cache"/> 实例写入到缓存。
    /// </summary>
    /// <param name="value">需要写入的缓存对象。</param>
    public async Task WriteAssemablyCatalogCacheAsync(AssemblyCatalogV2Cache value)
    {
        _assemblyCatalogCacheStream.SetLength(0);

        await MessagePackSerializer.SerializeAsync(
            _assemblyCatalogCacheStream, value,
            ContractlessStandardResolverAllowPrivate.Options);

        await _assemblyCatalogCacheStream.FlushAsync();
    }

    /// <summary>
    /// 将传入的 <see cref="CompositionConfiguration"/> 实例写入到缓存。
    /// </summary>
    /// <param name="value">需要写入的缓存对象。</param>
    public async Task WriteCompositionCacheAsync(CompositionConfiguration value)
    {
        _compositionCacheStream.SetLength(0);

        await new CachedComposition().SaveAsync(
            value, _compositionCacheStream);

        await _compositionCacheStream.FlushAsync();
    }

    /// <summary>
    /// 将当前的 <see cref="AssemblyCatalogV2Cache"/> 以及 <see cref="CompositionConfiguration"/> 的缓存一同写出到目标缓存流。
    /// </summary>
    /// <param name="cacheStream">目标输出流。</param>
    public async Task SaveAsync(Stream cacheStream)
    {
        // 这里写入第二个数据段的偏移量
        // 第一段数据的偏移量大小始终为 4
        // Second Data Offset + First Data + Second Data
        var offset = 4 + (int)_assemblyCatalogCacheStream.Length;
        await cacheStream.WriteAsync(BitConverter.GetBytes(offset));

        _assemblyCatalogCacheStream.Seek(0, SeekOrigin.Begin);
        await _assemblyCatalogCacheStream.CopyToAsync(cacheStream);

        _compositionCacheStream.Seek(0, SeekOrigin.Begin);
        await _compositionCacheStream.CopyToAsync(cacheStream);

        await cacheStream.FlushAsync();
    }

    /// <summary>
    /// 尝试清理写出缓存内容时所使用的临时文件。
    /// </summary>
    private void TryCleanup()
    {
        try
        {
            File.Delete(_assemblyCatalogCacheStream.Name);
        }
        catch
        {
        }

        try
        {
            File.Delete(_compositionCacheStream.Name);
        }
        catch
        {
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _assemblyCatalogCacheStream.Dispose();
            _compositionCacheStream.Dispose();

            TryCleanup();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _assemblyCatalogCacheStream.DisposeAsync();
            await _compositionCacheStream.DisposeAsync();

            TryCleanup();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
