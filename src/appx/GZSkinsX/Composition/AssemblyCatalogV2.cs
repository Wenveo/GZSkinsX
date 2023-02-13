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

using System.Reflection;

using GZSkinsX.Composition.Cache;

namespace GZSkinsX.Composition;

/// <summary>
/// 一个轻量级的类，用于枚举 <see cref="Assembly"/> 列表
/// </summary>
internal sealed class AssemblyCatalogV2 : IEquatable<AssemblyCatalogV2>
{
    /// <summary>
    /// 用于存放程序集的集合。使用字典并以 <see cref="Guid"/> 作为键对象
    /// </summary>
    private readonly Dictionary<Guid, Assembly> _guidToAsm;

    /// <summary>
    /// 用于存放当前组件目录的缓存，只有在被获取的时候才会生成
    /// </summary>
    private AssemblyCatalogV2Cache? _cache;

    /// <summary>
    /// 获取当前组件目录的缓存
    /// </summary>
    public AssemblyCatalogV2Cache Cache
    {
        get
        {
            if (_cache == null)
            {
                _cache = new AssemblyCatalogV2Cache();
                _cache.LoadFrom(this);
            }

            return _cache;
        }
    }

    /// <summary>
    /// 获取已枚举的 <see cref="Assembly"/>
    /// </summary>
    public IEnumerable<Assembly> Parts => _guidToAsm.Values;

    /// <summary>
    /// 初始化 <see cref="AssemblyCatalogV2"/> 的新实例
    /// </summary>
    public AssemblyCatalogV2()
    {
        _guidToAsm = new Dictionary<Guid, Assembly>();
    }

    /// <summary>
    /// 将程序集添加到集合并返回当前对象 <see cref="AssemblyCatalogV2"/>
    /// </summary>
    /// <param name="assemblys">将要被添加的程序集</param>
    /// <returns>当前容器对象 <see cref="AssemblyCatalogV2"/></returns>
    public AssemblyCatalogV2 AddParts(IEnumerable<Assembly> assemblys)
    {
        foreach (var asm in assemblys)
        {
            _guidToAsm[asm.ManifestModule.ModuleVersionId] = asm;
        }

        return this;
    }

    /// <summary>
    /// 将程序集添加到集合并返回当前对象 <see cref="AssemblyCatalogV2"/>
    /// </summary>
    /// <param name="assemblys">将要被添加的程序集</param>
    /// <returns>当前容器对象 <see cref="AssemblyCatalogV2"/></returns>
    public AssemblyCatalogV2 AddParts(params Assembly[] assemblys)
    {
        foreach (var asm in assemblys)
        {
            _guidToAsm[asm.ManifestModule.ModuleVersionId] = asm;
        }

        return this;
    }

    /// <summary>
    /// 将类型所处的程序集添加到集合并返回当前对象 <see cref="AssemblyCatalogV2"/>
    /// </summary>
    /// <param name="types">将要被枚举的类型</param>
    /// <returns>当前容器对象 <see cref="AssemblyCatalogV2"/></returns>
    public AssemblyCatalogV2 AddParts(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            _guidToAsm[type.Assembly.ManifestModule.ModuleVersionId] = type.Assembly;
        }

        return this;
    }

    /// <summary>
    /// 将类型所处的程序集添加到集合并返回当前对象 <see cref="AssemblyCatalogV2"/>
    /// </summary>
    /// <param name="types">将要被枚举的类型</param>
    /// <returns>当前容器对象 <see cref="AssemblyCatalogV2"/></returns>
    public AssemblyCatalogV2 AddParts(params Type[] types)
    {
        foreach (var type in types)
        {
            _guidToAsm[type.Assembly.ManifestModule.ModuleVersionId] = type.Assembly;
        }

        return this;
    }

    /// <inheritdoc/>
    public bool Equals(AssemblyCatalogV2? other)
    {
        if (other == null)
        {
            return false;
        }

        if (other._guidToAsm.Count != _guidToAsm.Count)
        {
            return false;
        }

        foreach (var guid in other._guidToAsm.Keys)
        {
            if (!_guidToAsm.ContainsKey(guid))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as AssemblyCatalogV2);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(
            _guidToAsm,
            _guidToAsm.Keys,
            _guidToAsm.Values);
    }
}
