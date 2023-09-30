// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.MRTCore;

using Microsoft.Windows.ApplicationModel.Resources;

namespace GZSkinsX.Appx.MRTCore;

/// <inheritdoc cref="IMRTCoreService"/>
[Shared, Export(typeof(IMRTCoreService))]
internal sealed class MRTCoreService : IMRTCoreService
{
    /// <summary>
    /// 当前应用程序的本地化资源管理实例。
    /// </summary>
    private readonly ResourceManager _resourceManager;

    /// <summary>
    /// 存放当前的主资源图实例。
    /// </summary>
    private readonly MRTCoreMap _mainResourceMap;

    /// <inheritdoc/>
    public IMRTCoreMap MainResourceMap => _mainResourceMap;

    /// <summary>
    /// 初始化 <see cref="MRTCoreService"/> 的新实例。
    /// </summary>
    public MRTCoreService()
    {
        _resourceManager = new ResourceManager();
        _resourceManager.ResourceNotFound += OnResourceNotFound;

        _mainResourceMap = new MRTCoreMap(_resourceManager.MainResourceMap);
    }

    /// <summary>
    /// 在出现资源访问异常时将错误信息输出至日志中。
    /// </summary>
    private void OnResourceNotFound(ResourceManager sender, ResourceNotFoundEventArgs args)
    {
        AppxContext.LoggingService.LogError(
            "GZSkinsX.Appx.MRTCore.MRTCoreService.OnResourceNotFound",
            $"The specific resource was not found \"{args.Name}\".");
    }

    /// <inheritdoc/>
    public IMRTCoreMap LoadPriFile(string priFile)
    {
        ArgumentException.ThrowIfNullOrEmpty(priFile);

        var resourceManager = new ResourceManager(priFile);
        return new MRTCoreMap(resourceManager.MainResourceMap);
    }
}
