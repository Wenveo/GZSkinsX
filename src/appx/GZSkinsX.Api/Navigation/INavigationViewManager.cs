// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace GZSkinsX.Api.Navigation;

/// <summary>
/// 表示对导航视图的抽象管理，并提供针对导航项的导航功能实现
/// </summary>
public interface INavigationViewManager
{
    /// <summary>
    /// 表示能否向后导航
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// 表示能否向前导航
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// 表示该 <see cref="INavigationViewManager"/> 中的 UI 对象
    /// </summary>
    object UIObject { get; }

    /// <summary>
    /// 在导航至目标页面后所触发的事件
    /// </summary>
    event NavigatedEventHandler? Navigated;

    /// <summary>
    /// 向后导航
    /// </summary>
    /// <returns>在完成目标操作时返回 true，否则返回 false</returns>
    bool GoBack();

    /// <summary>
    /// 向前导航
    /// </summary>
    /// <returns>在完成目标操作时返回 true，否则返回 false</returns>
    bool GoForward();

    /// <summary>
    /// 导航到与标识符匹配的指定页面
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值</param>
    void NavigateTo(string guidString);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值</param>
    /// <param name="parameter">传递给目标页面的参数</param>
    void NavigateTo(string guidString, object parameter);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数和指定导航页面切换效果
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值</param>
    /// <param name="parameter">传递给目标页面的参数</param>
    /// <param name="infoOverride"></param>
    void NavigateTo(string guidString, object parameter, NavigationTransitionInfo infoOverride);

    /// <summary>
    /// 导航到与标识符匹配的指定页面
    /// </summary>
    /// <param name="navItemGuid">视图对象的 <see cref="Guid"/> 值</param>
    void NavigateTo(Guid navItemGuid);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数
    /// </summary>
    /// <param name="navItemGuid">视图对象的 <see cref="Guid"/> 值</param>
    /// <param name="parameter">传递给目标页面的参数</param>
    void NavigateTo(Guid navItemGuid, object parameter);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数和指定导航页面切换效果
    /// </summary>
    /// <param name="navItemGuid">视图对象的 <see cref="Guid"/> 值</param>
    /// <param name="parameter">传递给目标页面的参数</param>
    /// <param name="infoOverride"></param>
    void NavigateTo(Guid navItemGuid, object parameter, NavigationTransitionInfo infoOverride);
}
