// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using Windows.UI.Xaml.Media.Animation;

namespace GZSkinsX.Api.Shell;

/// <summary>
/// 提供对当前应用程序主窗口中的根元素对象 切换/导航 的能力
/// </summary>
public interface IViewManagerService
{
    /// <summary>
    /// 获取当前容器是否可返回至上一个页面
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// 获取当前容器是否可向前导航
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// 返回至上一个页面
    /// </summary>
    void GoBack();

    /// <summary>
    /// 向前导航
    /// </summary>
    void GoForward();

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
    /// <param name="elemGuid">视图对象的 <see cref="Guid"/> 值</param>
    void NavigateTo(Guid elemGuid);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数
    /// </summary>
    /// <param name="elemGuid">视图对象的 <see cref="Guid"/> 值</param>
    /// <param name="parameter">传递给目标页面的参数</param>
    void NavigateTo(Guid elemGuid, object parameter);

    /// <summary>
    /// 导航到与标识符匹配的指定页面，并传递导航参数和指定导航页面切换效果
    /// </summary>
    /// <param name="elemGuid">视图对象的 <see cref="Guid"/> 值</param>
    /// <param name="parameter">传递给目标页面的参数</param>
    /// <param name="infoOverride"></param>
    void NavigateTo(Guid elemGuid, object parameter, NavigationTransitionInfo infoOverride);
}
