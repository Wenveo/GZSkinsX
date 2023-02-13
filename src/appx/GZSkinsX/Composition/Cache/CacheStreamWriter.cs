// Copyright 2022 - 2023 GZSkins, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MessagePack;
using MessagePack.Resolvers;

using Microsoft.VisualStudio.Composition;

namespace GZSkinsX.Composition.Cache;

/// <summary>
/// 组件缓存写入器，可将 <see cref="AssemblyCatalogV2Cache"/> 和 <see cref="IExportProviderFactory"/> 的实例写出到缓存流
/// </summary>
internal sealed class CacheStreamWriter : IAsyncDisposable, IDisposable
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

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _assemblyCatalogCacheStream.DisposeAsync();
            await _compositionCacheStream.DisposeAsync();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
