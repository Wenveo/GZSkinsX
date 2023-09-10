// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

using GZSkinsX.Contracts.AccessCache;
using GZSkinsX.Contracts.Settings;

using Windows.Storage;
using Windows.Storage.AccessCache;

namespace GZSkinsX.Services.AccessCache;

/// <inheritdoc cref="IFutureAccessService"/>
/// <summary>
/// 初始化 <see cref="FutureAccessService"/> 的新实例。
/// </summary>
[Shared, Export(typeof(IFutureAccessService))]
[method: ImportingConstructor]
internal sealed class FutureAccessService(ISettingsService settingsService) : IFutureAccessService
{
    /// <summary>
    /// 表示当前设置节点的 <seealso cref="Guid"/> 字符串值。
    /// </summary>
    private const string THE_GUID = "CF1A680D-E800-47E1-ABCF-116D64170C40";

    /// <summary>
    /// 用于存储本地数据的数据节点。
    /// </summary>
    private readonly ISettingsSection _settingsSection = settingsService.GetOrCreateSection(THE_GUID);

    /// <summary>
    /// 内部的存储项访问列表定义。
    /// </summary>
    private readonly StorageItemAccessList _futureAccessList = StorageApplicationPermissions.FutureAccessList;

    /// <inheritdoc/>
    public AccessListEntryView Entries => _futureAccessList.Entries;

    /// <inheritdoc/>
    public uint MaximumItemsAllowed => _futureAccessList.MaximumItemsAllowed;

    /// <inheritdoc/>
    public void Add(IStorageItem storageItem, string name)
    {
        ArgumentNullException.ThrowIfNull(storageItem);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        if (string.IsNullOrEmpty(token))
        {
            token = _futureAccessList.Add(storageItem, name);
            _settingsSection.Attribute(name, token);
        }
        else
        {
            _futureAccessList.AddOrReplace(token, storageItem, name);
        }
    }

    /// <inheritdoc/>
    public bool CheckAccess(IStorageItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _futureAccessList.CheckAccess(item);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _futureAccessList.Clear();
    }

    /// <inheritdoc/>
    public bool ContainsItem(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        if (token is null)
        {
            return false;
        }

        return _futureAccessList.ContainsItem(token);
    }

    /// <inheritdoc/>
    public async Task<StorageFile> GetFileAsync(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetFileAsync(token);
    }

    /// <inheritdoc/>
    public async Task<StorageFile> GetFileAsync(string name, AccessCacheOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetFileAsync(token, options);
    }

    /// <inheritdoc/>
    public async Task<StorageFolder> GetFolderAsync(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetFolderAsync(token);
    }

    /// <inheritdoc/>
    public async Task<StorageFolder> GetFolderAsync(string name, AccessCacheOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetFolderAsync(token, options);
    }

    /// <inheritdoc/>
    public async Task<IStorageItem> GetItemAsync(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetItemAsync(token);
    }

    /// <inheritdoc/>
    public async Task<IStorageItem> GetItemAsync(string name, AccessCacheOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        Debug.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetItemAsync(token, options);
    }

    /// <inheritdoc/>
    public void Remove(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var token = _settingsSection.Attribute<string>(name);
        if (token is not null)
        {
            _futureAccessList.Remove(token);
        }
    }

    /// <inheritdoc/>
    public async Task<StorageFile?> TryGetFileAsync(string name)
    {
        try
        {
            var file = await _futureAccessList.GetFileAsync(
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
            var file = await _futureAccessList.GetFileAsync(
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
            var folder = await _futureAccessList.GetFolderAsync(
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
            var folder = await _futureAccessList.GetFolderAsync(
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
            var item = await _futureAccessList.GetItemAsync(
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
            var item = await _futureAccessList.GetItemAsync(
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
            _futureAccessList.Remove(_settingsSection.Attribute<string>(name));
            return true;
        }
        catch
        {
            return false;
        }
    }
}
