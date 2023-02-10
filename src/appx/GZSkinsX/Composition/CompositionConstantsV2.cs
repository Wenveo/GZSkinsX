// Licensed to the GZSkins, Inc. under one or more agreements.
// The GZSkins, Inc. licenses this file to you under the MS-PL license.

using GZSkinsX.Contracts.App;

namespace GZSkinsX.Composition;

/// <summary>
/// 存放静态成员或常量，通常会在 <see cref="CompositionContainerV2"/> 中被使用
/// </summary>
internal static class CompositionConstantsV2
{
    /// <summary>
    /// 缓存文件的完整名称（包含路径）
    /// </summary>
    public static readonly string MefCacheFileName;

    /// <summary>
    /// 初始化 <see cref="CompositionConstantsV2"/> 的静态成员
    /// </summary>
    static CompositionConstantsV2()
    {
        MefCacheFileName = Path.Combine(AppxContext.AppxDirectory, "mef-cacheV2.bin");
    }
}
