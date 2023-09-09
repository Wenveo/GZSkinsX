// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.AccessCache;
using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Settings;

using System;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace GZSkinsX.Services.AccessCache;

/// <inheritdoc cref="IMostRecentlyUsedService"/>
[Shared, Export(typeof(IMostRecentlyUsedService))]
internal sealed class MostRecentlyUsedService : IMostRecentlyUsedService
{
    /// <summary>
    /// 表示当前设置节点的 <seealso cref="Guid"/> 字符串值。
    /// </summary>
    private const string THE_GUID = "6A50EFFD-185B-42FC-8509-14BE6EEC74EE";

    /// <summary>
    /// 用于存储本地数据的数据节点。
    /// </summary>
    private readonly ISettingsSection _settingsSection;

    /// <summary>
    /// 内部的最近使用 (MRU) 列表定义。
    /// </summary>
    private readonly StorageItemMostRecentlyUsedList _mostRecentlyUsedList;

    /// <inheritdoc/>
    public AccessListEntryView Entries => _mostRecentlyUsedList.Entries;

    /// <inheritdoc/>
    public uint MaximumItemsAllowed => _mostRecentlyUsedList.MaximumItemsAllowed;

    /// <inheritdoc/>
    public event TypedEventHandler<IMostRecentlyUsedService, ItemRemovedEventArgs>? ItemRemoved;

    /// <summary>
    /// 初始化 <see cref="MostRecentlyUsedService"/> 的新实例。
    /// </summary>
    public MostRecentlyUsedService()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
        _mostRecentlyUsedList = StorageApplicationPermissions.MostRecentlyUsedList;
        _mostRecentlyUsedList.ItemRemoved += OnItemRemoved;
    }

    /// <summary>
    /// 对接口中定义的事件 <seealso cref="IMostRecentlyUsedService.ItemRemoved"/> 进行代理通知。
    /// </summary>
    private void OnItemRemoved(StorageItemMostRecentlyUsedList sender, ItemRemovedEventArgs args)
    {
        // 传递当前服务对象，而不是 MRU 列表，以避免 MRU 列表在外部被使用
        ItemRemoved?.Invoke(this, args);
    }

    /// <inheritdoc/>
    public void Add(IStorageItem storageItem, string name)
    {
        ArgumentNullException.ThrowIfNull(storageItem);
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        if (string.IsNullOrEmpty(token))
        {
            token = _mostRecentlyUsedList.Add(storageItem, name);
            _settingsSection.Attribute(name, token);
        }
        else
        {
            _mostRecentlyUsedList.AddOrReplace(token, storageItem, name);
        }
    }

    /// <inheritdoc/>
    public void Add(IStorageItem storageItem, string name, RecentStorageItemVisibility visibility)
    {
        ArgumentNullException.ThrowIfNull(storageItem);
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        if (string.IsNullOrEmpty(token))
        {
            token = _mostRecentlyUsedList.Add(storageItem, name, visibility);
            _settingsSection.Attribute(name, token);
        }
        else
        {
            _mostRecentlyUsedList.AddOrReplace(token, storageItem, name, visibility);
        }
    }

    /// <inheritdoc/>
    public bool CheckAccess(IStorageItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _mostRecentlyUsedList.CheckAccess(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _mostRecentlyUsedList.Clear();
    }

    /// <inheritdoc/>
    public bool ContainsItem(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        if (token is null)
        {
            return false;
        }

        return _mostRecentlyUsedList.ContainsItem(token);
    }

    /// <inheritdoc/>
    public async Task<StorageFile> GetFileAsync(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _mostRecentlyUsedList.GetFileAsync(token);
    }

    /// <inheritdoc/>
    public async Task<StorageFile> GetFileAsync(string name, AccessCacheOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _mostRecentlyUsedList.GetFileAsync(token, options);
    }

    /// <inheritdoc/>
    public async Task<StorageFolder> GetFolderAsync(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _mostRecentlyUsedList.GetFolderAsync(token);
    }

    /// <inheritdoc/>
    public async Task<StorageFolder> GetFolderAsync(string name, AccessCacheOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _mostRecentlyUsedList.GetFolderAsync(token, options);
    }

    /// <inheritdoc/>
    public async Task<IStorageItem> GetItemAsync(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _mostRecentlyUsedList.GetItemAsync(token);
    }

    /// <inheritdoc/>
    public async Task<IStorageItem> GetItemAsync(string name, AccessCacheOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _mostRecentlyUsedList.GetItemAsync(token, options);
    }

    /// <inheritdoc/>
    public void Remove(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        string? token = _settingsSection.Attribute<string>(name);
        if (token is not null)
        {
            _mostRecentlyUsedList.Remove(token);
        }
    }

    /// <inheritdoc/>
    public async Task<StorageFile?> TryGetFileAsync(string name)
    {
        try
        {
            StorageFile file = await _mostRecentlyUsedList.GetFileAsync(
                _settingsSection.Attribute<string>(name));

            return file;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<StorageFile?> TryGetFileAsync(string name, AccessCacheOptions options)
    {
        try
        {
            StorageFile file = await _mostRecentlyUsedList.GetFileAsync(
                _settingsSection.Attribute<string>(name), options);

            return file;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<StorageFolder?> TryGetFolderAsync(string name)
    {
        try
        {
            StorageFolder folder = await _mostRecentlyUsedList.GetFolderAsync(
                _settingsSection.Attribute<string>(name));

            return folder;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<StorageFolder?> TryGetFolderAsync(string name, AccessCacheOptions options)
    {
        try
        {
            StorageFolder folder = await _mostRecentlyUsedList.GetFolderAsync(
                _settingsSection.Attribute<string>(name), options);

            return folder;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IStorageItem?> TryGetItemAsync(string name)
    {
        try
        {
            IStorageItem item = await _mostRecentlyUsedList.GetItemAsync(
                _settingsSection.Attribute<string>(name));

            return item;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IStorageItem?> TryGetItemAsync(string name, AccessCacheOptions options)
    {
        try
        {
            IStorageItem item = await _mostRecentlyUsedList.GetItemAsync(
                _settingsSection.Attribute<string>(name), options);

            return item;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public bool TryRemove(string name)
    {
        try
        {
            _mostRecentlyUsedList.Remove(_settingsSection.Attribute<string>(name));
            return true;
        }
        catch
        {
            return false;
        }
    }
}
