// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 有关导航视图的抽象管理，提供针对导航项的导航功能实现。
/// </summary>
public interface INavigationViewManager
{
    /// <summary>
    /// 获取能否向后导航。
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// 获取能否向前导航。
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// 获取导航视图的 UI 对象实例。
    /// </summary>
    object UIObject { get; }

    /// <summary>
    /// 在导航至目标页面后所触发的事件。
    /// </summary>
    event NavigatedEventHandler? Navigated;

    /// <summary>
    /// 向后导航。
    /// </summary>
    /// <returns>在完成操作时返回 true，否则返回 false。</returns>
    bool GoBack();

    /// <summary>
    /// 可等待的向后导航异步方法。
    /// </summary>
    /// <returns>在完成操作时返回 true，否则返回 false。</returns>
    Task<bool> GoBackAsync();

    /// <summary>
    /// 向前导航。
    /// </summary>
    /// <returns>在完成操作时返回 true，否则返回 false。</returns>
    bool GoForward();

    /// <summary>
    /// 可等待的向前导航异步方法。
    /// </summary>
    /// <returns>在完成操作时返回 true，否则返回 false。</returns>
    Task<bool> GoForwardAsync();

    /// <summary>
    /// 导航到与标识符匹配的指定页面。
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值。</param>
    void NavigateTo(string guidString);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数和指定页面动画切换。
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值。</param>
    /// <param name="parameter">传递给目标页面的参数。</param>
    /// <param name="infoOverride">有关动画切换的信息。</param>
    void NavigateTo(string guidString, object parameter, NavigationTransitionInfo infoOverride);

    /// <summary>
    /// 可等待的导航到与标识符匹配的指定页面的异步方法。
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值。</param>
    Task NavigateToAsync(string guidString);

    /// <summary>
    /// 可等待的导航到与标识符匹配的指定页面的异步方法，并传递导航参数。
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值。</param>
    /// <param name="parameter">传递给目标页面的参数。</param>
    Task NavigateToAsync(string guidString, object parameter);

    /// <summary>
    /// 可等待的导航到与标识符匹配的指定页面的异步方法，并传递导航参数和指定页面动画切换。
    /// </summary>
    /// <param name="guidString">视图对象的 <see cref="Guid"/> 的字符串值。</param>
    /// <param name="parameter">传递给目标页面的参数。</param>
    /// <param name="infoOverride">有关动画切换的信息。</param>
    Task NavigateToAsync(string guidString, object parameter, NavigationTransitionInfo infoOverride);
}
