// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.SDK.Navigation;

/// <summary>
/// 导航视图工厂的接口定义，提供枚举导航项以创建 <see cref="INavigationViewManager"/> 的能力
/// </summary>
public interface INavigationViewManagerFactory
{
    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值创建一个新的 <see cref="INavigationViewManager"/> 实现
    /// </summary>
    /// <param name="ownerGuidString">导航项所归属的 <see cref="System.Guid"/> 字符串值</param>
    /// <returns>已创建的 <see cref="INavigationViewManager"/> 类型实例</returns>
    INavigationViewManager CreateNavigationViewManager(string ownerGuidString);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值和 <see cref="NavigationView"/> 创建一个新的 <see cref="INavigationViewManager"/> 实现
    /// </summary>
    /// <param name="ownerGuidString">导航项所归属的 <see cref="System.Guid"/> 字符串值</param>
    /// <param name="targetElement">用于添加导航项的目标导航视图元素</param>
    /// <returns>已创建的 <see cref="INavigationViewManager"/> 类型实例</returns>
    INavigationViewManager CreateNavigationViewManager(string ownerGuidString, NavigationView targetElement);
}
