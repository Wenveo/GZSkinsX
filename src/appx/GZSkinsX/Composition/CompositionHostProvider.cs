// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Reflection;

using GZSkinsX.Api.Appx;

namespace GZSkinsX.Composition;

/// <summary>
/// 一个组件容器代理类，负责提供并创建 <see cref="global::System.Composition.Hosting.CompositionHost"/> 的实例
/// </summary>
internal sealed class CompositionHostProvider
{
    /// <summary>
    /// 懒加载容器，只有当获取时才会创建目标类型
    /// </summary>
    private static readonly Lazy<CompositionHostProvider> s_lazy = new(() => new());

    /// <summary>
    /// 全局静态 <see cref="CompositionHostProvider"/> 的实例，该类仅能存在且只会有一个实例
    /// </summary>
    public static CompositionHostProvider Instance => s_lazy.Value;

    /// <summary>
    /// 当前组件容器宿主的实例
    /// </summary>
    private readonly CompositionHost _compositionHost;

    /// <summary>
    /// 获取当前容器组件宿主实例
    /// </summary>
    public CompositionHost CompositionHost => _compositionHost;

    /// <summary>
    /// 初始化 <see cref="CompositionHostProvider"/> 的新实例
    /// </summary>
    private CompositionHostProvider()
    {
        var configuration = new ContainerConfiguration();
        configuration.WithAssemblies(GetAssemblies());
        _compositionHost = configuration.CreateContainer();
    }

    /// <summary>
    /// 获取当前 Appx 引用程序集
    /// </summary>
    private static IEnumerable<Assembly> GetAssemblies()
    {
        // Self Assembly
        yield return typeof(App).Assembly;
        // GZSkinsX.Api
        yield return typeof(IAppxWindow).Assembly;
        // GZSkinsX.Appx.Navigation
        yield return typeof(Appx.Navigation.AppxNavigation).Assembly;
        // GZSkinsX.Appx.Preload
        yield return typeof(Appx.Preload.AppxPreload).Assembly;
        // GZSkinsX.Appx.StartUp
        yield return typeof(Appx.StartUp.AppxStartUp).Assembly;
    }
}
