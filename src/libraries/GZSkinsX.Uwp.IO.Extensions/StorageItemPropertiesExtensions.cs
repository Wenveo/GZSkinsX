// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Storage;

namespace GZSkinsX.Uwp.IO.Extensions;

/// <summary>
/// 
/// </summary>
public static class StorageItemPropertiesExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static async Task<WindowCorePropertiesContainer> RetrievePropertiesAsync(this IStorageItemProperties item, WindowCorePropertiesBuilder builder)
    {
        return new WindowCorePropertiesContainer(await item.Properties.RetrievePropertiesAsync(builder.Build()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="propertyNames"></param>
    /// <returns></returns>
    public static async Task<WindowCorePropertiesContainer> RetrievePropertiesAsync(this IStorageItemProperties item, IEnumerable<string> propertyNames)
    {
        return new WindowCorePropertiesContainer(await item.Properties.RetrievePropertiesAsync(propertyNames));
    }
}
