// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GZSkinsX, PublicKey=00240000048000009400000006020000002400005253413100040000010001006d13ff08810f5c0937e5d36923b65fcd450723277974d8777efe5f5d919a78e8c9030d11f54f6044d3ecb2c1d94ea0ede3fb339774d048fbb9449e7f773ab757d5fd3c42351753d3ab19b8b68286ef28fd131441db8851cdf5d9853de411bfd62b08c260b13f8c65c8b1eb3df5c4fef891ec959dce72595cd109202cdbdfde9b")]

namespace GZSkinsX.Contracts.Appx;

public static partial class AppxContext
{
    /// <summary>
    /// 初始化应用程序的生命周期服务。
    /// </summary>
    /// <param name="resolver">解析器的对象的实例。</param>
    /// <exception cref="ArgumentNullException"><paramref name="resolver"/> 的默认值为空。</exception>
    internal static void InitializeLifetimeService(IServiceResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);

        if (_resolver is not null)
        {
            throw new InvalidOperationException("The lifetime service has been initialized!");
        }

        _resolver = resolver;
    }
}

/// <summary>
/// 有关提供检索和获取导出类型实例的能力的解析器接口。
/// </summary>
internal interface IServiceResolver
{
    /// <summary>
    /// 从已加载的所有组件中获取指定的导出的类型实例。
    /// </summary>
    /// <typeparam name="T">需要获取的已声明的导出类型。</typeparam>
    /// <returns>返回已检索的 <typeparamref name="T"/> 类型导出的实例。</returns>
    T Resolve<T>() where T : class;

    /// <summary>
    /// 尝试从已加载的所有组件中获取导出的类型实例。
    /// </summary>
    /// <typeparam name="T">目标类型中使用 <see cref="ExportAttribute"/> 所声明的导出类型。</typeparam>
    /// <param name="value">已获取到的类型实例，但如果获取失败则会返回 default。</param>
    /// <returns>当获取成功时返回 true，否则返回 false。</returns>
    bool TryResolve<T>([NotNullWhen(true)] out T? value) where T : class;
}
