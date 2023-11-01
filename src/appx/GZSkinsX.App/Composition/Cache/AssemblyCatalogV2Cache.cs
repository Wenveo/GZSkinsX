// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;

using MessagePack;

namespace GZSkinsX.Composition.Cache;

// <summary>
/// 组件目录缓存，用于缓存类型 <see cref="AssemblyCatalogV2"/> 中的程序集 <see cref="Guid"/> 列表。
/// </summary>
[MessagePackObject]
internal sealed class AssemblyCatalogV2Cache : IEquatable<AssemblyCatalogV2Cache>
{
    /// <summary>
    /// 使用 <see cref="HashSet{T}"/> 存储程序集的 <see cref="Guid"/>，该 <see cref="Guid"/> 具有唯一性。
    /// </summary>
    [Key(0)]
    private readonly HashSet<Guid> _guids;

    /// <summary>
    /// 初始化 <see cref="AssemblyCatalogV2Cache"/> 的新实例。
    /// </summary>
    public AssemblyCatalogV2Cache()
    {
        _guids = new HashSet<Guid>();
    }

    /// <summary>
    /// 从指定的对象中加载并生成缓存。
    /// </summary>
    /// <param name="assemblyCatalog">需要缓存的组件目录。</param>
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
