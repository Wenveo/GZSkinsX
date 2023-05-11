// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace GZSkinsX.Api.AccessCache;

/// <summary>
/// 提供对最近使用 (MRU) 的存储项列表进行管理，可跟踪用户最近访问的 (文件和文件夹)
/// </summary>
public interface IMostRecentlyUsedItemService : IAccessCacheService
{
    /// <summary>
    /// 从最近使用的 (MRU) 列表中删除存储项时触发
    /// </summary>
    event TypedEventHandler<IMostRecentlyUsedItemService, ItemRemovedEventArgs>? ItemRemoved;

    /// <summary>
    /// 将新的存储项和关联的名称添加到最近使用的 (MRU) 列表中，指定其可见性范围
    /// </summary>
    /// <param name="storageItem">要添加的存储项</param>
    /// <param name="name">要与存储项关联的名称</param>
    /// <param name="visibility">列表中存储项可见性的范围</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 或 <paramref name="storageItem"/> 上声明的默认值为 null</exception>
    void Add(IStorageItem storageItem, string name, RecentStorageItemVisibility visibility);
}
