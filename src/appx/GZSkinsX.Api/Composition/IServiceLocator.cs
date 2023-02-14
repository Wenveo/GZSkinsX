// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Composition;

/// <summary>
/// 提供获取已导出的类型的能力，范围包括应用程序加载的所有组件
/// </summary>
public interface IServiceLocator
{
    /// <summary>
    /// 从已加载的所有组件中获取导出的类型
    /// </summary>
    /// <typeparam name="T">需要获取的类型</typeparam>
    /// <returns>返回 <typeparamref name="T"/> 的实例</returns>
    /// <exception cref="InvalidOperationException"/>
    T Resolve<T>() where T : class;

    /// <summary>
    /// 尝试从已加载的所有组件中获取导出的类型
    /// </summary>
    /// <typeparam name="T">需要获取的类型</typeparam>
    /// <returns>返回 <typeparamref name="T"/> 的实例，如果获取失败则返回空</returns>
    T? TryResolve<T>() where T : class;
}
