// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls.Primitives;

namespace GZSkinsX.Contracts.ContextMenu;

/// <summary>
/// 表示上下文菜单的基本服务，并提供创建上下文菜单的能力。
/// </summary>
public interface IContextMenuService
{
    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值创建一个新的上下文命令菜单实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <returns>已创建的上下文命令菜单实例。</returns>
    FlyoutBase CreateCommandBarMenu(string ownerGuidString);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值和 <see cref="ICommandBarMenuOptions"/> 配置选项创建一个新的上下文命令菜单实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <returns>已创建的上下文命令菜单实例。</returns>
    FlyoutBase CreateCommandBarMenu(string ownerGuidString, ICommandBarMenuOptions options);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值和 <see cref="ContextMenuUIContextCallback"/> UI 上下文的回调委托创建一个新的上下文命令菜单实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <param name="callback">目标 UI 上下文的回调委托。</param>
    /// <returns>已创建的上下文命令菜单实例。</returns>
    FlyoutBase CreateCommandBarMenu(string ownerGuidString, ContextMenuUIContextCallback callback);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值和 <see cref="ICommandBarMenuOptions"/> 配置选项以及 <see cref="ContextMenuUIContextCallback"/> UI 上下文的回调委托创建一个新的上下文命令菜单实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <param name="callback">目标 UI 上下文的回调委托。</param>
    /// <returns>已创建的上下文命令菜单实例。</returns>
    FlyoutBase CreateCommandBarMenu(string ownerGuidString, ICommandBarMenuOptions options, ContextMenuUIContextCallback callback);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值创建一个新的上下文菜单实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <returns>已创建的上下文菜单实例。</returns>
    FlyoutBase CreateContextMenu(string ownerGuidString);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值和 <see cref="IContextMenuOptions"/> 配置选项创建一个新的上下文菜单实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <returns>已创建的上下文菜单实例。</returns>
    FlyoutBase CreateContextMenu(string ownerGuidString, IContextMenuOptions options);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值和 <see cref="ContextMenuUIContextCallback"/> UI 上下文的回调委托创建一个新的上下文菜单实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值。</param>
    /// <param name="callback">目标 UI 上下文的回调委托。</param>
    /// <returns>已创建的上下文菜单实例。</returns>
    FlyoutBase CreateContextMenu(string ownerGuidString, ContextMenuUIContextCallback callback);

    /// <summary>
    /// 通过指定的 <see cref="System.Guid"/> 字符串值和 <see cref="IContextMenuOptions"/> 配置选项以及 <see cref="ContextMenuUIContextCallback"/> UI 上下文的回调委托创建一个新的上下文菜单实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="Guid"/> 字符串值。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <param name="callback">目标 UI 上下文的回调委托。</param>
    /// <returns>已创建的上下文菜单实例。</returns>
    FlyoutBase CreateContextMenu(string ownerGuidString, IContextMenuOptions options, ContextMenuUIContextCallback callback);
}
