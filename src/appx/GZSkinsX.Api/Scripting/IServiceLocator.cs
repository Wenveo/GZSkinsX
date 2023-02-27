// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace GZSkinsX.Api.Scripting;

/// <summary>
/// 提供获取已导出的类型的能力，范围包括应用程序加载的所有组件
/// </summary>
public interface IServiceLocator
{
    /// <summary>
    /// 从已加载的所有组件中获取导出的类型实例
    /// </summary>
    /// <typeparam name="T">需要获取的类型</typeparam>
    /// <returns>返回 <typeparamref name="T"/> 的实例</returns>
    T Resolve<T>() where T : class;

    /// <summary>
    /// 尝试从已加载的所有组件中获取导出的类型实例
    /// </summary>
    /// <typeparam name="T">ExportAttribute 中所声明的导出类型</typeparam>
    /// <param name="value">已获取到的类型实例，但如果获取失败则会返回 default</param>
    /// <returns>当获取成功时返回 true，否则返回 false</returns>
    bool TryResolve<T>([NotNullWhen(true)] out T? value) where T : class;
}
