// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.SDK.ContextMenu;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.ContextMenu;

/// <summary>
/// 表示为 <see cref="MenuFlyout"/> 的帮助类，用于获取和设置自定义的附加属性
/// </summary>
internal sealed class ContextMenuFlyoutHelper
{
    /// <summary>
    /// 定义 CoerceValueCallback 的附加依赖属性
    /// </summary>
    public static readonly DependencyProperty CoerceValueCallbackProperty =
        DependencyProperty.RegisterAttached("CoerceValueCallback", typeof(CoerceContextMenuUIContextCallback),
            typeof(ContextMenuFlyoutHelper), new PropertyMetadata(null));

    /// <summary>
    /// 获取指定对象中的 <see cref="CoerceValueCallbackProperty"/> 附加属性的值
    /// </summary>
    public static CoerceContextMenuUIContextCallback? GetCoerceValueCallback(DependencyObject obj)
    => (CoerceContextMenuUIContextCallback)obj.GetValue(CoerceValueCallbackProperty);

    /// <summary>
    /// 对指定的对象设置 <see cref="CoerceValueCallbackProperty"/> 附加属性的值
    /// </summary>
    public static void SetCoerceValueCallback(DependencyObject obj, CoerceContextMenuUIContextCallback? value)
    => obj.SetValue(CoerceValueCallbackProperty, value);
}
