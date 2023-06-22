// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

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
}
