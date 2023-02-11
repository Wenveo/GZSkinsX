// Copyright 2022 - 2023 GZSkins, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")；
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

using System.Diagnostics;

using Microsoft.VisualStudio.Composition;

namespace GZSkinsX.Composition;

/// <summary>
/// 用于枚举 MEF (v2) 组件的容器，基于 VS-MEF
/// </summary>
internal sealed class CompositionContainerV2
{
    /// <summary>
    /// 已枚举的程序集目录
    /// </summary>
    private readonly AssemablyCatalogV2 _assemablyCatalog;

    /// <summary>
    /// 初始化 <see cref="CompositionContainerV2"/> 的新实例
    /// </summary>
    /// <param name="catalog"></param>
    public CompositionContainerV2(AssemablyCatalogV2 catalog)
    {
        _assemablyCatalog = catalog;
    }

    /// <summary>
    /// 从已有的缓存中加载或是创建一个新的 <see cref="ExportProvider"/> 实例
    /// </summary>
    /// <param name="useCache">是否使用缓存</param>
    /// <returns>从容器中创建的  <see cref="ExportProvider"/> 实例</returns>
    public async Task<ExportProvider> CreateExportProviderAsync(bool useCache)
    {
        return (await CreateExportProviderFactoryCoreAsync(useCache)).CreateExportProvider();
    }

    /// <summary>
    /// 从已有的缓存中加载或是创建一个新的 <see cref="IExportProviderFactory"/> 实例
    /// </summary>
    /// <param name="useCache">是否使用缓存</param>
    /// <returns>从容器中创建的 <see cref="IExportProviderFactory"/> 实例</returns>
    private async Task<IExportProviderFactory> CreateExportProviderFactoryCoreAsync(bool useCache)
    {
        var resolver = Resolver.DefaultInstance;
        if (useCache)
        {
            var factory = await TryGetCachedExportProviderFactoryAsync(resolver);
            if (factory != null)
            {
                return factory;
            }
        }

        return await CreateExportProviderFactoryAsync(resolver);
    }

    /// <summary>
    /// 枚举程序集并创建 <see cref="IExportProviderFactory"/>
    /// </summary>
    /// <param name="resolver">默认解析器</param>
    /// <returns>从容器中创建的  <see cref="IExportProviderFactory"/> 实例</returns>
    private async Task<IExportProviderFactory> CreateExportProviderFactoryAsync(Resolver resolver)
    {
        var discovery = new AttributedPartDiscovery(resolver, true);
        var parts = await discovery.CreatePartsAsync(_assemablyCatalog.Parts);
        Debug.Assert(parts.ThrowOnErrors() == parts);

        var composableCatalog = ComposableCatalog.Create(resolver).AddParts(parts);
        var configuragtion = CompositionConfiguration.Create(composableCatalog);
        Debug.Assert(configuragtion.ThrowOnErrors() == configuragtion);

        await SaveMefCacheAsync(configuragtion);

        return configuragtion.CreateExportProviderFactory();
    }

    /// <summary>
    /// 尝试在已缓存的文件中获取 <see cref="IExportProviderFactory"/> 对象
    /// </summary>
    /// <param name="resolver">默认解析器</param>
    /// <returns>从缓存中获取到的 <see cref="IExportProviderFactory"/> 对象，如果获取失败则返回 null </returns>
    private static async Task<IExportProviderFactory?> TryGetCachedExportProviderFactoryAsync(Resolver resolver)
    {
        try
        {
            using var cachedStream = File.OpenRead(CompositionConstantsV2.MefCacheFileName);
            return await new CachedComposition().LoadExportProviderFactoryAsync(cachedStream, resolver);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// 将当前 <see cref="CompositionConfiguration"/> 对象的缓存以文件形式保存到本地
    /// </summary>
    /// <param name="configuration">用于缓存的对象</param>
    private static async Task SaveMefCacheAsync(CompositionConfiguration configuration)
    {
        var isCreated = false;
        var canDelete = true;

        try
        {
            using var cachedStream = File.Create(CompositionConstantsV2.MefCacheFileName);
            isCreated = true;

            await new CachedComposition().SaveAsync(configuration, cachedStream);
            canDelete = false;
        }
        catch when (isCreated && canDelete)
        {
            try
            {
                File.Delete(CompositionConstantsV2.MefCacheFileName);
            }
            catch
            {
            }
        }
    }
}
