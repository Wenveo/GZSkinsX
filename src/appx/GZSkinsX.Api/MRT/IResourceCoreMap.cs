// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

namespace GZSkinsX.Api.MRT;

/// <summary>
/// 
/// </summary>
public interface IResourceCoreMap
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="resourceKey"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    Task<byte[]> GetBytesAsync(string resourceKey);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resourceKey"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    string GetString(string resourceKey);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    IResourceCoreMap GetSubtree(string reference);
}
