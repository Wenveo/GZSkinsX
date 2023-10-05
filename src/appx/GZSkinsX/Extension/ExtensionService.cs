// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Extension;

using Microsoft.UI.Xaml;

namespace GZSkinsX.Extension;

/// <summary>
/// 应用程序扩展服务，负责加载和通知已枚举的扩展。
/// </summary>
[Shared, Export]
internal sealed class ExtensionService
{
    /// <summary>
    /// 存放已枚举的自动加载的扩展的集合。
    /// </summary>
    private readonly Lazy<IExtensionClass, AutoLoadedContractAttribute>[] _mefAutoLoaded;

    /// <summary>
    /// 存放已枚举的应用程序扩展的集合。
    /// </summary>
    private readonly Lazy<IExtension, ExtensionContractAttribute>[] _mefExtensions;

    /// <summary>
    /// 初始化 <see cref="ExtensionService"/> 的新实例。
    /// </summary>
    [ImportingConstructor]
    public ExtensionService(
        [ImportMany] IEnumerable<Lazy<IExtensionClass, AutoLoadedContractAttribute>> mefAutoLoaded,
        [ImportMany] IEnumerable<Lazy<IExtension, ExtensionContractAttribute>> mefExtensions)
    {
        _mefAutoLoaded = [.. mefAutoLoaded.OrderBy(a => a.Metadata.Order)];
        _mefExtensions = [.. mefExtensions.OrderBy(a => a.Metadata.Order)];

        AppxContext.LoggingService.LogAlways("GZSkinsX.Extension.ExtensionService", "Successfully initialized.");
    }

    /// <summary>
    /// 获取所有应用程序扩展中声明的资源字典的集合。
    /// </summary>
    public IEnumerable<ResourceDictionary> GetMergedResourceDictionaries()
    {
        foreach (var extension in _mefExtensions)
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
    /// 通过筛选指定激活规则的自动加载的扩展进行激活。
    /// </summary>
    /// <param name="rule">指定的激活规则。</param>
    public void LoadAutoLoaded(AutoLoadedActivationConstraint rule)
    {
        foreach (var extension in _mefAutoLoaded)
        {
            if (extension.Metadata.LoadedWhen == rule)
            {
                _ = extension.Value;
            }
        }

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Extension.ExtensionService.LoadAutoLoaded",
            $"Load AutoLoaded extensions of loaded when '{rule}'.");
    }

    /// <summary>
    /// 对所有的应用程序扩展通知"已加载"事件。
    /// </summary>
    public void OnAppLoaded()
    {
        foreach (var extension in _mefExtensions)
        {
            extension.Value.OnAppLoaded();
        }

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Extension.ExtensionService.OnAppLoaded",
            $"Notify all application extensions of 'AppLoaded'.");
    }

    /// <summary>
    /// 对所有的应用程序扩展通知"退出"事件。
    /// </summary>
    public void OnAppExit()
    {
        foreach (var extension in _mefExtensions)
        {
            extension.Value.OnAppExit();
        }

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Extension.ExtensionService.OnAppExit",
            $"Notify all application extensions of 'AppExit'.");
    }
}
