// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

using Windows.Foundation;
using Windows.Foundation.Metadata;

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示在创建上下文菜单时可选的配置类
/// </summary>
[ContractVersion(typeof(UniversalApiContract), 65536u)]
public sealed class ContextMenuOptions
{
    /// <summary>
    /// 表示当用户与元素交互时是否自动获取焦点。
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 196608u)]
    public bool AllowFocusOnInteraction { get; set; }

    /// <summary>
    /// 表示控件在禁用时是否可以接收焦点
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 196608u)]
    public bool AllowFocusWhenDisabled { get; set; }

    /// <summary>
    /// 表示在浮出控件打开或关闭时是否播放动画
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 458752u)]
    public bool AreOpenCloseAnimationsEnabled { get; set; }

    /// <summary>
    /// 表示是否将 浅色消除 UI 外部的区域变暗
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 196608u)]
    public LightDismissOverlayMode LightDismissOverlayMode { get; set; }

    /// <summary>
    /// 设置在呈现 MenuFlyout 时使用的样式
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 65536u)]
    public Style? MenuFlyoutPresenterStyle { get; set; }

    /// <summary>
    /// 设置一个元素，该元素应接收指针输入事件，即使浮出控件覆盖之下也是如此
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 262144u)]
    public DependencyObject? OverlayInputPassThroughElement { get; set; }

    /// <summary>
    /// 设置要用于浮出控件的默认位置，相对于其放置目标
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 65536u)]
    public FlyoutPlacementMode Placement { get; set; }

    /// <summary>
    /// 表示是否应在 XAML 根的边界内显示浮出控件
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 524288u)]
    public bool ShouldConstrainToRootBounds { get; set; }

    /// <summary>
    /// 表示浮出控件在显示时的行为方式
    /// </summary>
    [ContractVersion(typeof(UniversalApiContract), 458752u)]
    public FlyoutShowMode ShowMode { get; set; }

    /// <summary>
    /// 初始化 <see cref="ContextMenuOptions"/> 的新实例
    /// </summary>
    public ContextMenuOptions()
    {
        AllowFocusOnInteraction = true;
        AreOpenCloseAnimationsEnabled = true;
        LightDismissOverlayMode = LightDismissOverlayMode.Auto;
        Placement = FlyoutPlacementMode.Top;
        ShowMode = FlyoutShowMode.Standard;
    }
}
