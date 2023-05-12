// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Extension;
using GZSkinsX.Api.Logging;

using Windows.UI.Xaml;

namespace GZSkinsX.Extension;

/// <summary>
/// 应用程序扩展服务，负责加载和通知已枚举的扩展
/// </summary>
[Shared, Export]
internal sealed class ExtensionService
{
    /// <summary>
    /// 存放已枚举的先行扩展的集合
    /// </summary>
    private readonly Lazy<IAdvanceExtension, AdvanceExtensionMetadataAttribute>[] _mefAdvanceExtensions;

    /// <summary>
    /// 存放已枚举的通用扩展的集合
    /// </summary>
    private readonly Lazy<IUniversalExtension, UniversalExtensionMetadataAttribute>[] _mefUniversalExtensions;

    /// <summary>
    /// 用于记录日志的日志服务
    /// </summary>
    private readonly ILoggingService _loggingService;

    /// <summary>
    /// 获取所有通用扩展的实例
    /// </summary>
    public IEnumerable<IUniversalExtension> Extensions
    {
        get
        {
            foreach (var item in _mefUniversalExtensions)
            {
                yield return item.Value;
            }
        }
    }

    /// <summary>
    /// 初始化 <see cref="ExtensionService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public ExtensionService(
        [ImportMany] IEnumerable<Lazy<IAdvanceExtension, AdvanceExtensionMetadataAttribute>> mefAdvanceExtensions,
        [ImportMany] IEnumerable<Lazy<IUniversalExtension, UniversalExtensionMetadataAttribute>> mefUniversalExtensions)
    {
        _mefAdvanceExtensions = mefAdvanceExtensions.OrderBy(a => a.Metadata.Order).ToArray();
        _mefUniversalExtensions = mefUniversalExtensions.OrderBy(a => a.Metadata.Order).ToArray();

        _loggingService = AppxContext.LoggingService;
        _loggingService.LogAlways("ExtensionService: Initialized successfully.");
    }

    /// <summary>
    /// 获取所有通用扩展中声明的资源字典的集合
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ResourceDictionary> GetMergedResourceDictionaries()
    {
        foreach (var extension in _mefUniversalExtensions)
        {
            var value = extension.Value;
            foreach (var rsrc in value.MergedResourceDictionaries)
            {
                var asm = value.GetType().Assembly.GetName();
                var uri = new Uri($"ms-appx:///{asm.Name}/{rsrc}", UriKind.Absolute);
                yield return new ResourceDictionary { Source = uri };
            }
        }
    }

    /// <summary>
    /// 通过筛选指定触发类型的先行扩展进行加载
    /// </summary>
    /// <param name="trigger">指定的触发类型</param>
    public void LoadAdvanceExtensions(AdvanceExtensionTrigger trigger)
    {
        foreach (var extension in _mefAdvanceExtensions)
        {
            if (extension.Metadata.Trigger == trigger)
            {
                _ = extension.Value;
            }
        }

        _loggingService.LogAlways($"ExtensionService: Load all AdvanceExtension of type '{trigger}'.");
    }

    /// <summary>
    /// 对所有的通用扩展进行事件通知
    /// </summary>
    /// <param name="eventType">需要通知的事件类型</param>
    public void NotifyUniversalExtensions(UniversalExtensionEvent eventType)
    {
        foreach (var extension in _mefUniversalExtensions)
        {
            extension.Value.OnEvent(eventType);
        }

        _loggingService.LogAlways($"ExtensionService: Notify event '{eventType}' for all universal extensions.");
    }
}
