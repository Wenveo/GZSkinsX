// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using GZSkinsX.Composition.Cache;
using GZSkinsX.Contracts.Appx;

using Microsoft.VisualStudio.Composition;

namespace GZSkinsX.Composition;

/// <summary>
/// 用于枚举 MEF (v2) 组件的容器，基于 VS-MEF。
/// </summary>
/// <param name="catalog">组件目录。</param>
internal sealed class CompositionContainerV2(AssemblyCatalogV2 catalog)
{
    /// <summary>
    /// 获取缓存文件的完整路径名称。
    /// </summary>
    private static string CacheFileFullPath { get; } = Path.Combine(AppxContext.LocalCacheFolder, "mef-cacheV2.bin");

    /// <summary>
    /// 从已有的缓存中加载或是创建一个新的 <see cref="ExportProvider"/> 实例。
    /// </summary>
    /// <param name="useCache">是否使用缓存。</param>
    /// <returns>从容器中创建的  <see cref="ExportProvider"/> 实例。</returns>
    public async Task<ExportProvider> CreateExportProviderAsync(bool useCache)
    {
        return (await CreateExportProviderFactoryCoreAsync(useCache)).CreateExportProvider();
    }

    /// <summary>
    /// 从已有的缓存中加载或是创建一个新的 <see cref="IExportProviderFactory"/> 实例。
    /// </summary>
    /// <param name="useCache">是否使用缓存。</param>
    /// <returns>从容器中创建的 <see cref="IExportProviderFactory"/> 实例。</returns>
    private async Task<IExportProviderFactory> CreateExportProviderFactoryCoreAsync(bool useCache)
    {
        if (useCache)
        {
            var factory = await TryGetCachedExportProviderFactoryAsync(Resolver.DefaultInstance);
            if (factory != null)
            {
                return factory;
            }
        }

        return await CreateExportProviderFactoryAsync(Resolver.DefaultInstance);
    }

    /// <summary>
    /// 枚举程序集并创建 <see cref="IExportProviderFactory"/> 类型实例。
    /// </summary>
    /// <param name="resolver">默认解析器。</param>
    /// <returns>从容器中创建的  <see cref="IExportProviderFactory"/> 实例。</returns>
    private async Task<IExportProviderFactory> CreateExportProviderFactoryAsync(Resolver resolver)
    {
        var discovery = new AttributedPartDiscovery(resolver, true);
        var parts = await discovery.CreatePartsAsync(catalog.Parts);
        Debug.Assert(parts.ThrowOnErrors() == parts);

        var composableCatalog = ComposableCatalog.Create(resolver).AddParts(parts);
        var configuragtion = CompositionConfiguration.Create(composableCatalog);
        Debug.Assert(configuragtion.ThrowOnErrors() == configuragtion);

        await SaveMefCacheAsync(configuragtion);

        return configuragtion.CreateExportProviderFactory();
    }

    /// <summary>
    /// 尝试在已缓存的文件中获取 <see cref="IExportProviderFactory"/> 对象。
    /// </summary>
    /// <param name="resolver">默认解析器。</param>
    /// <returns>从缓存中获取到的 <see cref="IExportProviderFactory"/> 对象，如果获取失败则返回 null。</returns>
    private async Task<IExportProviderFactory?> TryGetCachedExportProviderFactoryAsync(Resolver resolver)
    {
        try
        {
            using var reader = new CacheStreamReader(File.OpenRead(CacheFileFullPath));
            var oldCache = await reader.ReadAssemablyCatalogCacheAsync();
            var newCache = catalog.Cache;
            if (newCache.Equals(oldCache))
            {
                return await reader.ReadCompositionCacheAsync(resolver);
            }
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    /// 将当前 <see cref="CompositionConfiguration"/> 对象的缓存以文件形式保存到本地。
    /// </summary>
    /// <param name="configuration">用于缓存的对象。</param>
    private async Task SaveMefCacheAsync(CompositionConfiguration configuration)
    {
        var isCreated = false;
        var canDelete = true;

        try
        {
            using var cachedStream = File.Create(CacheFileFullPath);
            isCreated = true;

            await using var writer = new CacheStreamWriter();
            await writer.WriteAssemablyCatalogCacheAsync(catalog.Cache);
            await writer.WriteCompositionCacheAsync(configuration);
            await writer.SaveAsync(cachedStream);

            canDelete = false;
        }
        catch when (isCreated && canDelete)
        {
            try
            {
                File.Delete(CacheFileFullPath);
            }
            catch
            {
            }
        }
    }
}
