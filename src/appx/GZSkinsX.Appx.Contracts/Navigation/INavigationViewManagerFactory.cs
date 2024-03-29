// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 导航视图工厂的接口定义，提供枚举导航项以创建 <see cref="INavigationViewManager"/> 的接口方法。
/// </summary>
public interface INavigationViewManagerFactory
{
    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值创建一个新的 <see cref="INavigationViewManager"/> 实现。
    /// </summary>
    /// <param name="ownerGuidString">导航项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <returns>已创建的导航视图管理实例。</returns>
    INavigationViewManager CreateNavigationViewManager(string ownerGuidString);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值和 <see cref="NavigationView"/> 创建一个新的 <see cref="INavigationViewManager"/> 实现。
    /// </summary>
    /// <param name="ownerGuidString">导航项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <param name="options">用于创建导航视图管理的配置选项。</param>
    /// <returns>已创建的导航视图管理实例。</returns>
    INavigationViewManager CreateNavigationViewManager(string ownerGuidString, NavigationViewManagerOptions options);
}
