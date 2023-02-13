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

namespace GZSkinsX.Composition.Cache;

/// <summary>
/// 组件目录缓存，用于缓存类型 <see cref="AssemblyCatalogV2"/> 中的程序集 <see cref="Guid"/> 列表
/// </summary>
[MessagePackObject]
internal sealed class AssemblyCatalogV2Cache : IEquatable<AssemblyCatalogV2Cache>
{
    /// <summary>
    /// 使用 <see cref="HashSet{T}"/> 存储程序集的 <see cref="Guid"/>，该 <see cref="Guid"/> 具有唯一性
    /// </summary>
    [Key(0)]
    private readonly HashSet<Guid> _guids;

    /// <summary>
    /// 初始化 <see cref="AssemblyCatalogV2Cache"/> 的新实例
    /// </summary>
    public AssemblyCatalogV2Cache()
    {
        _guids = new HashSet<Guid>();
    }

    /// <summary>
    /// 从指定的对象中加载并生成缓存
    /// </summary>
    /// <param name="assemblyCatalog">需要缓存的组件目录</param>
    public void LoadFrom(AssemblyCatalogV2 assemblyCatalog)
    {
        _guids.Clear();

        foreach (var asm in assemblyCatalog.Parts)
        {
            _guids.Add(asm.ManifestModule.ModuleVersionId);
        }
    }

    /// <inheritdoc/>
    public bool Equals(AssemblyCatalogV2Cache? other)
    {
        if (other == null)
        {
            return false;
        }

        if (other._guids.Count != _guids.Count)
        {
            return false;
        }

        foreach (var guid in other._guids)
        {
            if (!_guids.Contains(guid))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as AssemblyCatalogV2Cache);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return _guids.GetHashCode();
    }
}
