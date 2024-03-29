// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using MessagePack;
using MessagePack.Resolvers;

using Microsoft.VisualStudio.Composition;

namespace GZSkinsX.Composition.Cache;

/// <summary>
/// 组件缓存读取器，可在流读取 <see cref="AssemblyCatalogV2Cache"/> 和 <see cref="IExportProviderFactory"/> 的缓存实例。
/// </summary>
/// <param name="cachedStream">输入的缓存流。</param>
/// <param name="leaveOpen">离开作用域时是否保持流为打开状态。</param>
internal sealed class CacheStreamReader(Stream cachedStream, bool leaveOpen) : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// 用于判断当前类是否调用过 Dispose 或 DisposeAsync。
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// 初始化 <see cref="CacheStreamReader"/> 的新实例。
    /// </summary>
    /// <param name="cachedStream">输入的缓存流。</param>
    public CacheStreamReader(Stream cachedStream) : this(cachedStream, false) { }

    /// <summary>
    /// 查找特定类型的缓存并设置当前流的偏移量。
    /// </summary>
    /// <param name="isAssmblyCatalogCache">需要查找的缓存类型；当为 true 时表示 <see cref="AssemblyCatalogV2Cache"/>，否则为 <see cref="CachedComposition"/>。</param>
    [SkipLocalsInit]
    private void SeekCache(bool isAssmblyCatalogCache)
    {
        // 设置 AssmblyCatalogCache 的默认偏移量
        // 如果非 AssmblyCatalogCache 则另外计算
        var offset = 4;
        if (!isAssmblyCatalogCache)
        {
            // 跳转到文件头并读取第二段数据的偏移量
            cachedStream.Seek(0, SeekOrigin.Begin);

            unsafe
            {
                Span<byte> buffer = stackalloc byte[4];
                var count = cachedStream.Read(buffer);
                Debug.Assert(count == buffer.Length);

                offset = MemoryMarshal.Read<int>(buffer);
            }
        }

        // 跳转到目标缓存数据的起始位置
        cachedStream.Seek(offset, SeekOrigin.Begin);
    }

    /// <summary>
    /// 在缓存流中读取 <see cref="AssemblyCatalogV2Cache"/> 的缓存。
    /// </summary>
    /// <returns>从缓存流中读取到的 <see cref="AssemblyCatalogV2Cache"/> 实例。</returns>
    public async Task<AssemblyCatalogV2Cache> ReadAssemablyCatalogCacheAsync()
    {
        SeekCache(isAssmblyCatalogCache: true);
        return await MessagePackSerializer.DeserializeAsync<AssemblyCatalogV2Cache>(
            stream: cachedStream, options: ContractlessStandardResolverAllowPrivate.Options);
    }

    /// <summary>
    /// 在缓存流中读取缓存的 <see cref="CachedComposition"/> 组件。
    /// </summary>
    /// <param name="resolver">默认解析器。</param>
    /// <returns>从缓存流中读取到的 <see cref="IExportProviderFactory"/> 实例。</returns>
    public async Task<IExportProviderFactory> ReadCompositionCacheAsync(Resolver resolver)
    {
        SeekCache(isAssmblyCatalogCache: false);
        return await new CachedComposition().LoadExportProviderFactoryAsync(
            cacheStream: cachedStream, resolver: resolver);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            if (!leaveOpen)
            {
                cachedStream.Dispose();
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (!leaveOpen)
            {
                await cachedStream.DisposeAsync();
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
