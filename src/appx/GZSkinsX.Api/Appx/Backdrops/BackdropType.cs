// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Appx.Backdrops;

/// <summary>
/// 应用程序窗口背景类型
/// </summary>
public enum BackdropType
{
    /// <summary>
    /// 云母是在 Windows 11 中引入的新的不透明材料。
    /// <para>云母图面使用用户的桌面背景色进行淡染。</para>
    /// <para>云母可感知模式；它支持浅色和深色模式。云母还指示将活动状态和非活动状态作为内置功能的窗口焦点。</para>
    /// </summary>
    Mica,
    /// <summary>
    /// Mica Alt 是 Mica 的变体，具有更强的用户桌面背景色着色。
    /// <para>可以将 Mica Alt 应用于应用的背景，以提供比 Mica 更深层次的视觉层次结构，尤其是在创建具有选项卡式标题栏的应用时。</para>
    /// </summary>
    MicaAlt,
    /// <summary>
    /// 亚克力是一种半透明材料，可复制毛玻璃的效果。 
    /// <para>在 Windows 11 中，亚克力已更新为更亮且更半透明的材料，加强与它背后的视觉对象的上下文关系。</para>
    /// <para>亚克力仅用于暂时性的轻型消除图面，例如浮出控件和上下文菜单。</para>
    /// </summary>
    DesktopAcrylic,
    /// <summary>
    /// 默认主题颜色，不带任何背景材质。
    /// <para>使用其会将已有的材质背景全部清除，并还原回默认背景。</para>
    /// </summary>
    DefaultColor
}
