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

using GZSkinsX.Api.Extension;

using Windows.UI.Xaml;

namespace GZSkinsX.Extension;

/// <summary>
/// 应用程序扩展服务，负责加载和通知已加载的扩展
/// </summary>
[Shared]
[Export]
internal sealed class ExtensionService
{
    /// <summary>
    /// 自动加载的组件集
    /// </summary>
    private readonly Lazy<IAutoLoaded, AutoLoadedMetadataAttribute>[] _mefAutoLoaded;

    /// <summary>
    /// 基本组件扩展集
    /// </summary>
    private readonly Lazy<IExtension, ExtensionMetadataAttribute>[] _extensions;

    /// <summary>
    /// 一个用于获取所有扩展实例的迭代器
    /// </summary>
    public IEnumerable<IExtension> Extensions
    {
        get
        {
            foreach (var item in _extensions)
            {
                yield return item.Value;
            }
        }
    }

    /// <summary>
    /// 初始化 <see cref="ExtensionService"/> 的新实例
    /// </summary>
    /// <param name="mefAutoLoaded">可自动加载的组件集合</param>
    /// <param name="extensions">应用程序扩展集合</param>
    [ImportingConstructor]
    public ExtensionService(
        [ImportMany] IEnumerable<Lazy<IAutoLoaded, AutoLoadedMetadataAttribute>> mefAutoLoaded,
        [ImportMany] IEnumerable<Lazy<IExtension, ExtensionMetadataAttribute>> extensions)
    {
        _mefAutoLoaded = mefAutoLoaded.OrderBy(a => a.Metadata.Order).ToArray();
        _extensions = extensions.OrderBy(a => a.Metadata.Order).ToArray();
    }

    /// <summary>
    /// 获取所有扩展组件中的 <see cref="ResourceDictionary"/> 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ResourceDictionary> GetMergedResourceDictionaries()
    {
        foreach (var extension in _extensions)
        {
            var value = extension.Value;
            foreach (var rsrc in value.MergedResourceDictionaries)
            {
                var asm = value.GetType().Assembly.GetName();
                var uri = new Uri($"ms-appx://{asm.Name}/{rsrc}", UriKind.Absolute);
                yield return new ResourceDictionary { Source = uri };
            }
        }
    }

    /// <summary>
    /// 筛选并加载指定 '加载类型' 的组件实例
    /// </summary>
    /// <param name="loadType">目标组件加载类型</param>
    public void LoadAutoLoaded(AutoLoadedType loadType)
    {
        foreach (var extension in _mefAutoLoaded)
        {
            if (extension.Metadata.LoadType == loadType)
            {
                _ = extension.Value;
            }
        }
    }

    /// <summary>
    /// 对应用程序扩展组件进行事件通知
    /// </summary>
    /// <param name="eventType">需要通知的事件类型</param>
    public void NotifyExtensions(ExtensionEvent eventType)
    {
        foreach (var extension in _extensions)
        {
            extension.Value.OnEvent(eventType);
        }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(_mefAutoLoaded, _extensions);
    }
}
