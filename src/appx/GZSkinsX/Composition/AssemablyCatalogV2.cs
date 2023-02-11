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

using System.Reflection;

namespace GZSkinsX.Composition;

/// <summary>
/// 一个轻量级的类，用于枚举 <see cref="Assembly"/>
/// </summary>
internal sealed class AssemablyCatalogV2
{
    /// <summary>
    /// 用于存放程序集的集合。使用字典并以 <see cref="HashCode"/> 作为键对象
    /// </summary>
    private readonly Dictionary<int, Assembly> _hashCodeAsm;

    /// <summary>
    /// 获取已枚举的 <see cref="Assembly"/>
    /// </summary>
    public IEnumerable<Assembly> Parts => _hashCodeAsm.Values;

    /// <summary>
    /// 初始化 <see cref="AssemablyCatalogV2"/> 的新实例
    /// </summary>
    public AssemablyCatalogV2()
    {
        _hashCodeAsm = new Dictionary<int, Assembly>();
    }

    /// <summary>
    /// 将程序集添加到集合并返回当前对象 <see cref="AssemablyCatalogV2"/>
    /// </summary>
    /// <param name="assemblys">将要被添加的程序集</param>
    /// <returns>当前容器对象 <see cref="AssemablyCatalogV2"/></returns>
    public AssemablyCatalogV2 AddParts(IEnumerable<Assembly> assemblys)
    {
        foreach (var asm in assemblys)
        {
            _hashCodeAsm[asm.GetHashCode()] = asm;
        }

        return this;
    }

    /// <summary>
    /// 将程序集添加到集合并返回当前对象 <see cref="AssemablyCatalogV2"/>
    /// </summary>
    /// <param name="assemblys">将要被添加的程序集</param>
    /// <returns>当前容器对象 <see cref="AssemablyCatalogV2"/></returns>
    public AssemablyCatalogV2 AddParts(params Assembly[] assemblys)
    {
        foreach (var asm in assemblys)
        {
            _hashCodeAsm[asm.GetHashCode()] = asm;
        }

        return this;
    }

    /// <summary>
    /// 将类型所处的程序集添加到集合并返回当前对象 <see cref="AssemablyCatalogV2"/>
    /// </summary>
    /// <param name="types">将要被枚举的类型</param>
    /// <returns>当前容器对象 <see cref="AssemablyCatalogV2"/></returns>
    public AssemablyCatalogV2 AddParts(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            _hashCodeAsm[type.Assembly.GetHashCode()] = type.Assembly;
        }

        return this;
    }

    /// <summary>
    /// 将类型所处的程序集添加到集合并返回当前对象 <see cref="AssemablyCatalogV2"/>
    /// </summary>
    /// <param name="types">将要被枚举的类型</param>
    /// <returns>当前容器对象 <see cref="AssemablyCatalogV2"/></returns>
    public AssemablyCatalogV2 AddParts(params Type[] types)
    {
        foreach (var type in types)
        {
            _hashCodeAsm[type.Assembly.GetHashCode()] = type.Assembly;
        }

        return this;
    }
}
