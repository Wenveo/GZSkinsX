// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
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
/// 组件缓存写入器，可将 <see cref="AssemblyCatalogV2Cache"/> 和 <see cref="IExportProviderFactory"/> 的实例写出到缓存流
/// </summary>
public sealed class CacheStreamWriter : IDisposable
{
    /// <summary>
    /// 当前的 <see cref="AssemblyCatalogV2Cache"/> 缓存流
    /// </summary>
    private readonly MemoryStream _assemblyCatalogCacheStream;

    /// <summary>
    /// 当前的 <see cref="CompositionConfiguration"/> 缓存流
    /// </summary>
    private readonly MemoryStream _compositionCacheStream;

    /// <summary>
    /// 用于判断当前类是否调用过 Dispose 或 DisposeAsync
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// 初始化 <see cref="CacheStreamWriter"/> 的新实例
    /// </summary>
    public CacheStreamWriter()
    {
        _assemblyCatalogCacheStream = new MemoryStream();
        _compositionCacheStream = new MemoryStream();
    }

    /// <summary>
    /// 将传入的 <see cref="AssemblyCatalogV2Cache"/> 实例写入到缓存
    /// </summary>
    /// <param name="value">写入的对象</param>
    public async Task WriteAssemablyCatalogCacheAsync(AssemblyCatalogV2Cache value)
    {
        _assemblyCatalogCacheStream.SetLength(0);

        await MessagePackSerializer.SerializeAsync(
            _assemblyCatalogCacheStream, value,
            ContractlessStandardResolverAllowPrivate.Options);

        await _assemblyCatalogCacheStream.FlushAsync();
    }

    /// <summary>
    /// 将传入的 <see cref="CompositionConfiguration"/> 实例写入到缓存
    /// </summary>
    /// <param name="value">写入的对象</param>
    public async Task WriteCompositionCacheAsync(CompositionConfiguration value)
    {
        _compositionCacheStream.SetLength(0);

        await new CachedComposition().SaveAsync(
            value, _compositionCacheStream);

        await _compositionCacheStream.FlushAsync();
    }

    /// <summary>
    /// 将当前的 <see cref="AssemblyCatalogV2Cache"/> 以及 <see cref="CompositionConfiguration"/> 的缓存一同写出到目标缓存流
    /// </summary>
    /// <param name="cacheStream">目标写出流</param>
    public async Task SaveAsync(Stream cacheStream)
    {
        using var bw = new BinaryWriter(cacheStream);

        // 这里写入第二个数据段的偏移量
        // 第一段数据的偏移量始终为 4
        // Second Data Offset + First Data + Second Data
        bw.Write(4 + (int)_assemblyCatalogCacheStream.Length);

        bw.Write(_assemblyCatalogCacheStream.ToArray());
        bw.Write(_compositionCacheStream.ToArray());

        await cacheStream.FlushAsync();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _assemblyCatalogCacheStream.Dispose();
            _compositionCacheStream.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
