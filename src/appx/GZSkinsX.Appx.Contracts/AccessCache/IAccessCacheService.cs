// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.AccessCache;

namespace GZSkinsX.Contracts.AccessCache;

/// <summary>
/// 表示对访问的存储项进行缓存管理的服务。该接口为一个通用基本接口，被用于 <seealso cref="IFutureAccessService"/>
/// 和 <seealso cref="IMostRecentlyUsedService"/>，并且由子类型实现和导出。相反，这个接口则永远不会被实现并导出。
/// </summary>
public interface IAccessCacheService
{
    /// <summary>
    /// 获取用于从访问列表中检索存储项的对象。
    /// </summary>
    AccessListEntryView Entries { get; }

    /// <summary>
    /// 获取访问列表可以包含的最大存储项数。
    /// </summary>
    uint MaximumItemsAllowed { get; }

    /// <summary>
    /// 将新的存储项添加到访问列表。
    /// </summary>
    /// <param name="storageItem">要添加的存储项。</param>
    /// <param name="name">要与存储项关联的名称。</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 或 <paramref name="storageItem"/> 上声明的默认值为 null。</exception>
    void Add(IStorageItem storageItem, string name);

    /// <summary>
    /// 确定应用是否有权访问访问列表中的指定存储项。
    /// </summary>
    /// <param name="item">要检查访问权限的存储项。</param>
    /// <returns>如果应用可以访问存储项则为 True，否则为 false。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> 上声明的默认值为 null。</exception>
    public bool CheckAccess(IStorageItem item);

    /// <summary>
    /// 从访问列表中删除所有存储项。
    /// </summary>
    public void Clear();

    /// <summary>
    /// 确定访问列表是否包含指定的存储项。
    /// </summary>
    /// <param name="name">要查找的存储项的名称。</param>
    /// <returns>如果访问列表包含指定的存储项则为 True，否则为 false。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null。</exception>
    bool ContainsItem(string name);

    /// <summary>
    /// 从列表中检索指定的 <see cref="StorageFile"/>。
    /// </summary>
    /// <param name="name">要检索的 <see cref="StorageFile"/> 的名称。</param>
    /// <returns>此方法成功完成后，将返回与指定名称关联的 <see cref="StorageFile"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null。</exception>
    /// <exception cref="AccessCacheItemNotFoundException">未在列表中检索到与名称关联的 <see cref="StorageFile"/>。</exception>
    Task<StorageFile> GetFileAsync(string name);

    /// <summary>
    /// 使用指定的选项从列表中检索指定的 <see cref="StorageFile"/>。
    /// </summary>
    /// <param name="name">要检索的 <see cref="StorageFile"/> 的名称。</param>
    /// <param name="options">描述应用访问项时要使用的行为的枚举值。</param>
    /// <returns>此方法成功完成后，将返回与指定名称关联的 <see cref="StorageFile"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null。</exception>
    /// <exception cref="AccessCacheItemNotFoundException">未在列表中检索到与名称关联的 <see cref="StorageFile"/>。</exception>
    Task<StorageFile> GetFileAsync(string name, AccessCacheOptions options);

    /// <summary>
    /// 从列表中检索指定的 <see cref="StorageFolder"/>。
    /// </summary>
    /// <param name="name">要检索的 <see cref="StorageFolder"/> 的名称。</param>
    /// <returns>此方法成功完成后，它将返回与指定名称关联的 <see cref="StorageFolder"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null。</exception>
    /// <exception cref="AccessCacheItemNotFoundException">未在列表中检索到与名称关联的 <see cref="StorageFolder"/>。</exception>
    Task<StorageFolder> GetFolderAsync(string name);

    /// <summary>
    /// 使用指定的选项从列表中检索指定的 <see cref="StorageFolder"/>。
    /// </summary>
    /// <param name="name">要检索的 <see cref="StorageFolder"/> 的名称。</param>
    /// <param name="options">枚举值，该值描述应用访问项目时要使用的行为。</param>
    /// <returns>此方法成功完成后，它将返回与指定名称关联的 <see cref="StorageFolder"/>。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null。</exception>
    /// <exception cref="AccessCacheItemNotFoundException">未在列表中检索到与名称关联的 <see cref="StorageFolder"/></exception>
    Task<StorageFolder> GetFolderAsync(string name, AccessCacheOptions options);

    /// <summary>
    /// 从列表中检索指定的项 (例如文件或文件夹)。
    /// </summary>
    /// <param name="name">要检索的项的名称。</param>
    /// <returns>此方法成功完成后，它将返回与指定标记关联的项 (<see cref="IStorageItem"/>)。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null。</exception>
    /// <exception cref="AccessCacheItemNotFoundException">未在列表中检索到与名称关联的 <see cref="IStorageItem"/>。</exception>
    Task<IStorageItem> GetItemAsync(string name);

    /// <summary>
    /// 使用指定的选项从列表中检索指定的项 (例如文件或文件夹)。
    /// </summary>
    /// <param name="name">要检索的项的名称。</param>
    /// <param name="options">描述应用访问项时要使用的行为的枚举值。</param>
    /// <returns>此方法成功完成后，它将返回与指定标记关联的项 (<see cref="IStorageItem"/>)。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null。</exception>
    /// <exception cref="AccessCacheItemNotFoundException">未在列表中检索到与名称关联的 <see cref="IStorageItem"/>。</exception>
    Task<IStorageItem> GetItemAsync(string name, AccessCacheOptions options);

    /// <summary>
    /// 从访问列表中删除指定的存储项。
    /// </summary>
    /// <param name="name">要删除的存储项的令牌名称。</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null。</exception>
    void Remove(string name);

    /// <summary>
    /// 尝试从列表中检索指定的 <see cref="StorageFile"/>。
    /// </summary>
    /// <param name="name">要检索的 <see cref="StorageFile"/> 的名称。</param>
    /// <returns>如果此方法成功完成后，将返回与指定名称关联的 <see cref="StorageFile"/>，否则将返回 null。</returns>
    Task<StorageFile?> TryGetFileAsync(string name);

    /// <summary>
    /// 尝试使用指定的选项从列表中检索指定的 <see cref="StorageFile"/>。
    /// </summary>
    /// <param name="name">要检索的 <see cref="StorageFile"/> 的名称。</param>
    /// <param name="options">描述应用访问项时要使用的行为的枚举值。</param>
    /// <returns>如果此方法成功完成后，将返回与指定名称关联的 <see cref="StorageFile"/>，否则将返回 null。</returns>
    Task<StorageFile?> TryGetFileAsync(string name, AccessCacheOptions options);

    /// <summary>
    /// 尝试从列表中检索指定的 <see cref="StorageFolder"/>。
    /// </summary>
    /// <param name="name">要检索的 <see cref="StorageFolder"/> 的名称。</param>
    /// <returns>如果此方法成功完成后，它将返回与指定名称关联的 <see cref="StorageFolder"/>，否则将返回 null。</returns>
    Task<StorageFolder?> TryGetFolderAsync(string name);

    /// <summary>
    /// 尝试使用指定的选项从列表中检索指定的 <see cref="StorageFolder"/>。
    /// </summary>
    /// <param name="name">要检索的 <see cref="StorageFolder"/> 的名称。</param>
    /// <param name="options">枚举值，该值描述应用访问项目时要使用的行为。</param>
    /// <returns>如果此方法成功完成后，它将返回与指定名称关联的 <see cref="StorageFolder"/>，否则将返回 null。</returns>
    Task<StorageFolder?> TryGetFolderAsync(string name, AccessCacheOptions options);

    /// <summary>
    /// 尝试从列表中检索指定的项 (例如文件或文件夹)。
    /// </summary>
    /// <param name="name">要检索的项的名称。</param>
    /// <returns>如果此方法成功完成后，它将返回与指定标记关联的项 (<see cref="IStorageItem"/>)，否则将返回 null。</returns>
    Task<IStorageItem?> TryGetItemAsync(string name);

    /// <summary>
    /// 尝试使用指定的选项从列表中检索指定的项 (例如文件或文件夹)。
    /// </summary>
    /// <param name="name">要检索的项的名称。</param>
    /// <param name="options">描述应用访问项时要使用的行为的枚举值。</param>
    /// <returns>如果此方法成功完成后，它将返回与指定标记关联的项 (<see cref="IStorageItem"/>)，否则将返回 null。</returns>
    Task<IStorageItem?> TryGetItemAsync(string name, AccessCacheOptions options);

    /// <summary>
    /// 尝试从访问列表中删除指定的存储项。
    /// </summary>
    /// <param name="name">要删除的存储项的令牌名称。</param>
    /// <returns>如果成功将与指定标记关联的项移除则返回 true，否则返回 false。</returns>
    bool TryRemove(string name);
}
