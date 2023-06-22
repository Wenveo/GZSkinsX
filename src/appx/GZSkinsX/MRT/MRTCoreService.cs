// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.MRT;

using Microsoft.Windows.ApplicationModel.Resources;

namespace GZSkinsX.MRT;

/// <inheritdoc cref="IMRTCoreService"/>
[Shared, Export(typeof(IMRTCoreService))]
internal sealed class MRTCoreService : IMRTCoreService
{
    /// <summary>
    /// 当前应用程序的资源管理器实例
    /// </summary>
    private readonly ResourceManager _resourceManager;

    /// <summary>
    /// 当前的主资源表实例，因经常访问故而不使用懒加载
    /// </summary>
    private readonly MRTCoreMap _mainResourceMap;

    /// <inheritdoc/>
    public IMRTCoreMap MainResourceMap => _mainResourceMap;

    /// <summary>
    /// 初始化 <see cref="MRTCoreService"/> 的新实例
    /// </summary>
    public MRTCoreService()
    {
        _resourceManager = new ResourceManager();
        _mainResourceMap = new MRTCoreMap(_resourceManager.MainResourceMap);

        _resourceManager.ResourceNotFound += OnResourceNotFound;
    }

    private void OnResourceNotFound(ResourceManager sender, ResourceNotFoundEventArgs args)
    {
        AppxContext.LoggingService.LogError($"MRTCoreService: The specific resource was not found '{args.Name}'.");
    }

    /// <inheritdoc/>
    public IMRTCoreMap CreateMRTCoreMap(string priFile)
    {
        ArgumentException.ThrowIfNullOrEmpty(priFile);

        var resourceManager = new ResourceManager(priFile);
        return new MRTCoreMap(resourceManager.MainResourceMap);
    }
}
