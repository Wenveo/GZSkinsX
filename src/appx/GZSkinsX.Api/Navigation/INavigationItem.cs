// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GZSkinsX.Api.Navigation;

/// <summary>
/// 表示为导航视图中的导航项，该接口为最基本的接口定义
/// </summary>
public interface INavigationItem
{
    /// <summary>
    /// 表示该导航项的图标，用于在 UI 中显示
    /// </summary>
    IconElement? Icon { get; }

    /// <summary>
    /// 在导航至目标页面时触发的行为
    /// </summary>
    /// <param name="args">导航事件的参数</param>
    Task OnNavigatedToAsync(NavigationEventArgs args);

    /// <summary>
    /// 在离开当前页面时触发的行为
    /// </summary>
    Task OnNavigatedFromAsync();
}
