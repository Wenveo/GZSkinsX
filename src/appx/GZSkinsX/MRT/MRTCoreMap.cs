// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

using GZSkinsX.Api.MRT;

using Microsoft.Windows.ApplicationModel.Resources;

namespace GZSkinsX.MRT;

/// <inheritdoc cref="IMRTCoreMap"/>
internal sealed class MRTCoreMap : IMRTCoreMap
{
    /// <summary>
    /// 用于获取本地化资源内容的资源表
    /// </summary>
    private readonly ResourceMap _resourceMap;

    /// <inheritdoc/>
    public uint ResourceCount => _resourceMap.ResourceCount;

    /// <summary>
    /// 初始化 <see cref="MRTCoreMap"/> 的新实例
    /// </summary>
    public MRTCoreMap(ResourceMap resourceMap)
    {
        _resourceMap = resourceMap;
    }

    /// <inheritdoc/>
    public byte[] GetBytes(string resourceKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(resourceKey);
        return _resourceMap.GetValue(resourceKey).ValueAsBytes;
    }

    /// <inheritdoc/>
    public string GetString(string resourceKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(resourceKey);
        return _resourceMap.GetValue(resourceKey).ValueAsString;
    }

    /// <inheritdoc/>
    public IMRTCoreMap GetSubtree(string reference)
    {
        ArgumentException.ThrowIfNullOrEmpty(reference);
        return new MRTCoreMap(_resourceMap.GetSubtree(reference));
    }

    /// <inheritdoc/>
    public bool TryGetBytes(string resourceKey, [NotNullWhen(true)] out byte[]? bytes)
    {
        var candidate = _resourceMap.TryGetValue(resourceKey);
        if (candidate is not null)
        {
            bytes = candidate.ValueAsBytes;
            return true;
        }

        bytes = null;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetString(string resourceKey, [NotNullWhen(true)] out string? value)
    {
        var candidate = _resourceMap.TryGetValue(resourceKey);
        if (candidate is not null)
        {
            value = candidate.ValueAsString;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetSubtree(string reference, [NotNullWhen(true)] out IMRTCoreMap? mrtCoreMap)
    {
        var subtree = _resourceMap.TryGetSubtree(reference);
        if (subtree is not null)
        {
            mrtCoreMap = new MRTCoreMap(subtree);
            return true;
        }

        mrtCoreMap = null;
        return false;
    }
}
