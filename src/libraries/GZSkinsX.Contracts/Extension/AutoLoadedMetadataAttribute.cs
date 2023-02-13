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

using System.Composition;

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 表示自动加载的扩展的元数据
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AutoLoadedMetadataAttribute : Attribute
{
    /// <summary>
    /// 扩展的加载顺序
    /// </summary>
    public double Order { get; set; }

    /// <summary>
    /// 扩展的触发类型
    /// </summary>
    public AutoLoadedType LoadType { get; set; }
}
