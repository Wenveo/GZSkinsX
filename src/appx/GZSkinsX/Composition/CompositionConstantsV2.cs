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
