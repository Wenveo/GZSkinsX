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

using Microsoft.VisualStudio.Composition;

namespace GZSkinsX.Composition.Cache;

/// <summary>
/// 缓存数据的类型
/// </summary>
internal enum CacheDataType
{
    /// <summary>
    /// 表示 <see cref="AssemblyCatalogV2"/> 类型的缓存
    /// </summary>
    AssemablyCatalog,
    /// <summary>
    /// 表示 <see cref="CachedComposition"/> 类型的缓存
    /// </summary>
    Composition
}
