// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;

using GZSkinsX.Contracts.MRTCore;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace GZSkinsX.Services.MRTCore;

/// <inheritdoc cref="IMRTCoreService"/>
[Shared, Export(typeof(IMRTCoreService))]
internal sealed class MRTCoreService : IMRTCoreService
{
    /// <summary>
    /// 当前应用程序的本地化资源管理实例
    /// </summary>
    private readonly ResourceManager _resourceManager;

    /// <summary>
    /// 用于存放本地所有的 <see cref="IMRTCoreMap"/> 资源图的字典
    /// </summary>
    private readonly Dictionary<string, IMRTCoreMap> _allResourceMaps;

    /// <summary>
    /// 表示为只读的 <see cref="IMRTCoreMap"/> 资源图的字典。因为内部只存在一个字典源，因此只需要一个成员实例即可。
    /// </summary>
    private readonly ReadOnlyDictionary<string, IMRTCoreMap> _readOnlyResourceMaps;

    /// <summary>
    /// 当前的主资源图实例，因经常访问故而不使用懒加载
    /// </summary>
    private readonly MRTCoreMap _mainResourceMap;

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, IMRTCoreMap> AllResourceMaps
    {
        get
        {
            var rawAllResourceMaps = _resourceManager.AllResourceMaps;

            // 从应用程序中的资源图进行同步
            foreach (var item in rawAllResourceMaps)
            {
                // 如果在本地的字典中找不到匹配的项则创建并将其添加进去
                if (_allResourceMaps.ContainsKey(item.Key) is false)
                {
                    _allResourceMaps.Add(item.Key, new MRTCoreMap(item.Value));
                }
            }

            // 查找和删除多余的项，并保存同步
            foreach (var item in _allResourceMaps)
            {
                // 当从应用程序中的资源表中找不到与本地的资源图相匹配的项时则将其移除
                if (rawAllResourceMaps.ContainsKey(item.Key) is false)
                {
                    _allResourceMaps.Remove(item.Key);
                }
            }

            return _readOnlyResourceMaps;
        }
    }

    /// <inheritdoc/>
    public IMRTCoreMap MainResourceMap => _mainResourceMap;

    /// <summary>
    /// 初始化 <see cref="MRTCoreService"/> 的新实例
    /// </summary>
    public MRTCoreService()
    {
        _resourceManager = ResourceManager.Current;
        _mainResourceMap = new MRTCoreMap(_resourceManager.MainResourceMap);
        _allResourceMaps = new Dictionary<string, IMRTCoreMap>()
        {
            { Package.Current.Id.Name, _mainResourceMap}
        };

        _readOnlyResourceMaps = new(_allResourceMaps);
    }

    /// <inheritdoc/>
    public void LoadPriFiles(IEnumerable<IStorageFile> files)
    {
        _resourceManager.LoadPriFiles(files);
    }

    /// <inheritdoc/>
    public void UnloadPriFiles(IEnumerable<IStorageFile> files)
    {
        _resourceManager.UnloadPriFiles(files);
    }
}
