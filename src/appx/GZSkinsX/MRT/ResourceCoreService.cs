// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Composition;

using GZSkinsX.Api.MRT;

using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace GZSkinsX.MRT;

/// <inheritdoc cref="IResourceCoreService"/>
[Shared, Export(typeof(IResourceCoreService))]
internal sealed class ResourceCoreService : IResourceCoreService
{
    /// <summary>
    /// 当前应用程序的资源管理器实例
    /// </summary>
    private readonly ResourceManager _resourceManager;

    /// <summary>
    /// 当前的主资源表实例，因经常访问故而不使用懒加载
    /// </summary>
    private readonly ResourceCoreMap _mainResourceMap;

    /// <inheritdoc/>
    public IResourceCoreMap MainResourceMap => _mainResourceMap;

    /// <summary>
    /// 初始化 <see cref="ResourceCoreService"/> 的新实例
    /// </summary>
    public ResourceCoreService()
    {
        _resourceManager = ResourceManager.Current;
        _mainResourceMap = new ResourceCoreMap(_resourceManager.MainResourceMap);
    }

    /// <inheritdoc/>
    public void LoadPriFiles(IEnumerable<IStorageFile> files)
    => _resourceManager.LoadPriFiles(files);

    /// <inheritdoc/>
    public void UnloadPriFiles(IEnumerable<IStorageFile> files)
    => _resourceManager.UnloadPriFiles(files);
}
