// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示上下文菜单的基本服务，并提供创建上下文菜单的能力
/// </summary>
public interface IContextMenuService
{
    /// <summary>
    /// 通过指定的 <see cref="Guid"/> 字符串值创建一个新的 <see cref="MenuFlyout"/> 实现
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值</param>
    /// <returns>已创建的 <see cref="MenuFlyout"/> 类型实例</returns>
    MenuFlyout CreateContextMenu(string ownerGuidString);

    /// <summary>
    /// 通过指定的 <see cref="Guid"/> 字符串值和 <see cref="ContextMenuOptions"/> 上下文菜单选项配置创建一个新的 <see cref="MenuFlyout"/> 实现
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项</param>
    /// <returns>已创建的 <see cref="MenuFlyout"/> 类型实例</returns>
    MenuFlyout CreateContextMenu(string ownerGuidString, ContextMenuOptions options);

    /// <summary>
    /// 通过指定的 <see cref="Guid"/> 字符串值和 <see cref="CoerceContextMenuUIContextCallback"/> UI 上下文的回调委托创建一个新的 <see cref="MenuFlyout"/> 实现
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值</param>
    /// <param name="coerceValueCallback">目标 UI 上下文的回调委托</param>
    /// <returns>已创建的 <see cref="MenuFlyout"/> 类型实例</returns>
    MenuFlyout CreateContextMenu(string ownerGuidString, CoerceContextMenuUIContextCallback coerceValueCallback);

    /// <summary>
    /// 通过指定的 <see cref="Guid"/> 字符串值和 <see cref="ContextMenuOptions"/> 上下文菜单选项配置以及 <see cref="CoerceContextMenuUIContextCallback"/> UI 上下文的回调委托创建一个新的 <see cref="MenuFlyout"/> 实现
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项</param>
    /// <param name="coerceValueCallback">目标 UI 上下文的回调委托</param>
    /// <returns>已创建的 <see cref="MenuFlyout"/> 类型实例</returns>
    MenuFlyout CreateContextMenu(string ownerGuidString, ContextMenuOptions options, CoerceContextMenuUIContextCallback coerceValueCallback);
}
