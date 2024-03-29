// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

using Windows.Foundation;
using Windows.Foundation.Metadata;

namespace GZSkinsX.Contracts.ContextMenu;

/// <summary>
/// 关于在创建上下文菜单时可选的配置。
/// </summary>
[ContractVersion(typeof(UniversalApiContract), 65536u)]
public interface IContextMenuOptions
{
    /// <summary>
    /// 表示当用户与元素交互时是否自动获取焦点。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 196608u)]
    bool AllowFocusOnInteraction { get; }

    /// <summary>
    /// 表示控件在禁用时是否可以接收焦点。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 196608u)]
    bool AllowFocusWhenDisabled { get; }

    /// <summary>
    /// 表示在浮出控件打开或关闭时是否播放动画。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 458752u)]
    bool AreOpenCloseAnimationsEnabled { get; }

    /// <summary>
    /// 表示是否将 浅色消除 UI 外部的区域变暗。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 196608u)]
    LightDismissOverlayMode LightDismissOverlayMode { get; }

    /// <summary>
    /// 设置一个元素，该元素应接收指针输入事件，即使浮出控件覆盖之下也是如此。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 262144u)]
    DependencyObject? OverlayInputPassThroughElement { get; }

    /// <summary>
    /// 设置要用于浮出控件的默认位置，相对于其放置目标。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 65536u)]
    FlyoutPlacementMode Placement { get; }

    /// <summary>
    /// 表示是否应在 XAML 根的边界内显示浮出控件。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 524288u)]
    bool ShouldConstrainToRootBounds { get; }

    /// <summary>
    /// 表示浮出控件在显示时的行为方式。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 458752u)]
    FlyoutShowMode ShowMode { get; }
}
