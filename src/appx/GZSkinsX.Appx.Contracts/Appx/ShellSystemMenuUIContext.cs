// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.ContextMenu;

using Microsoft.UI;
using Microsoft.UI.Windowing;

namespace GZSkinsX.Contracts.Appx;

/// <summary>
/// 有关窗口标题栏的系统菜单的 UI 上下文。
/// </summary>
public sealed class ShellSystemMenuUIContext : IContextMenuUIContext
{
    /// <summary>
    /// 获取与 <see cref="AppWindow"/> 所关联的 <see cref="WindowId"/>。
    /// </summary>
    public WindowId AppWindowId { get; init; }

    /// <summary>
    /// 获取当前窗口的句柄。
    /// </summary>
    public nint WindowHandle { get; init; }

    /// <summary>
    /// 获取该窗口是否为最大化状态。
    /// </summary>
    public bool IsMaximized { get; init; }
}
