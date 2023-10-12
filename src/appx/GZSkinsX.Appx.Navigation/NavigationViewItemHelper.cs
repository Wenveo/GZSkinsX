// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Navigation;

/// <summary>
/// 定义导航视图项的自定义附加属性。
/// </summary>
internal static class NavigationViewItemHelper
{
    /// <summary>
    /// 定义 ItemContext 附加依赖属性。
    /// </summary>
    public static readonly DependencyProperty ItemContextProperty =
        DependencyProperty.RegisterAttached("ItemContext", typeof(NavigationItemMD),
            typeof(NavigationViewItemHelper), new PropertyMetadata(null));

    /// <summary>
    /// 获取指定对象中的 <see cref="ItemContextProperty"/> 附加属性的值。
    /// </summary>
    public static NavigationItemMD? GetItemContext(NavigationViewItem obj)
    {
        return (NavigationItemMD?)obj.GetValue(ItemContextProperty);
    }

    /// <summary>
    /// 对指定的对象设置 <see cref="ItemContextProperty"/> 附加属性的值。
    /// </summary>
    public static void SetItemContext(NavigationViewItem obj, NavigationItemMD? value)
    {
        obj.SetValue(ItemContextProperty, value);
    }
}
