// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api.AccessCache;
using GZSkinsX.Api.Settings;
using GZSkinsX.DotNet.Diagnostics;

using Windows.Storage;
using Windows.Storage.AccessCache;

namespace GZSkinsX.AccessCache;

/// <inheritdoc cref="IFutureAccessService"/>
[Shared, Export(typeof(IFutureAccessService))]
internal sealed class FutureAccessService : IFutureAccessService
{
    /// <summary>
    /// 表示当前设置节点的 <seealso cref="Guid"/> 字符串值
    /// </summary>
    private const string THE_GUID = "CF1A680D-E800-47E1-ABCF-116D64170C40";

    /// <summary>
    /// 用于存储本地数据的数据节点
    /// </summary>
    private readonly ISettingsSection _settingsSection;

    /// <summary>
    /// 内部的存储项访问列表定义
    /// </summary>
    private readonly StorageItemAccessList _futureAccessList;

    /// <inheritdoc/>
    public AccessListEntryView Entries => _futureAccessList.Entries;

    /// <inheritdoc/>
    public uint MaximumItemsAllowed => _futureAccessList.MaximumItemsAllowed;

    /// <summary>
    /// 初始化 <see cref="FutureAccessService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public FutureAccessService(ISettingsService settingsService)
    {
        _settingsSection = settingsService.GetOrCreateSection(THE_GUID);
        _futureAccessList = StorageApplicationPermissions.FutureAccessList;
    }

    /// <inheritdoc/>
    public void Add(IStorageItem storageItem, string name)
    {
        if (storageItem is null)
        {
            throw new ArgumentNullException(nameof(storageItem));
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

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
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

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
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

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
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var token = _settingsSection.Attribute<string>(name);
        Debug2.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetFileAsync(token);
    }

    /// <inheritdoc/>
    public async Task<StorageFile> GetFileAsync(string name, AccessCacheOptions options)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var token = _settingsSection.Attribute<string>(name);
        Debug2.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetFileAsync(token, options);
    }

    /// <inheritdoc/>
    public async Task<StorageFolder> GetFolderAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var token = _settingsSection.Attribute<string>(name);
        Debug2.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetFolderAsync(token);
    }

    /// <inheritdoc/>
    public async Task<StorageFolder> GetFolderAsync(string name, AccessCacheOptions options)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var token = _settingsSection.Attribute<string>(name);
        Debug2.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetFolderAsync(token, options);
    }

    /// <inheritdoc/>
    public async Task<IStorageItem> GetItemAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var token = _settingsSection.Attribute<string>(name);
        Debug2.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetItemAsync(token);
    }

    /// <inheritdoc/>
    public async Task<IStorageItem> GetItemAsync(string name, AccessCacheOptions options)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var token = _settingsSection.Attribute<string>(name);
        Debug2.Assert(token != null);
        if (token is null)
        {
            throw new AccessCacheItemNotFoundException(name);
        }

        return await _futureAccessList.GetItemAsync(token, options);
    }

    /// <inheritdoc/>
    public void Remove(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var token = _settingsSection.Attribute<string>(name);
        if (token is not null)
        {
            _futureAccessList.Remove(token);
        }
    }
}
