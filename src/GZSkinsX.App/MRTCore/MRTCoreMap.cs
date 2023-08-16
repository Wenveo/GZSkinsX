// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using GZSkinsX.Contracts.MRTCore;

using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace GZSkinsX.MRTCore;

/// <inheritdoc cref="IMRTCoreMap"/>
internal sealed class MRTCoreMap(ResourceMap resourceMap) : IMRTCoreMap
{
    /// <summary>
    /// 用于获取本地化资源内容的资源表
    /// </summary>
    private readonly ResourceMap _resourceMap = resourceMap;

    /// <inheritdoc/>
    public async Task<byte[]> GetBytesAsync(string resourceKey)
    {
        if (string.IsNullOrEmpty(resourceKey))
        {
            throw new ArgumentNullException(nameof(resourceKey));
        }

        var item = await _resourceMap.GetValue(resourceKey).GetValueAsFileAsync();
        var buffer = await FileIO.ReadBufferAsync(item);

        return WindowsRuntimeBufferExtensions.ToArray(buffer);
    }

    /// <inheritdoc/>
    public string GetString(string resourceKey)
    {
        if (string.IsNullOrEmpty(resourceKey))
        {
            throw new ArgumentNullException(nameof(resourceKey));
        }

        return _resourceMap.GetValue(resourceKey).ValueAsString;
    }

    /// <inheritdoc/>
    public IMRTCoreMap GetSubtree(string reference)
    {
        if (string.IsNullOrEmpty(reference))
        {
            throw new ArgumentNullException(nameof(reference));
        }

        var subtree = _resourceMap.GetSubtree(reference);
        return new MRTCoreMap(subtree);
    }
}
