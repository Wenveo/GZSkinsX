// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.Commands;

/// <summary>
/// 表示为 <see cref="ICommandBarElement"/> 的帮助类，用于获取和设置自定义的附加属性
/// </summary>
internal static class CommandBarElementHelper
{
    /// <summary>
    /// 定义 ParentCommandBar 的附加依赖属性
    /// </summary>
    public static readonly DependencyProperty ParentCommandBarProperty =
        DependencyProperty.RegisterAttached("ParentCommandBar", typeof(CommandBar),
            typeof(CommandBarElementHelper), new PropertyMetadata(null));

    /// <summary>
    /// 获取指定对象中的 <see cref="ParentCommandBarProperty"/> 附加属性的值
    /// </summary>
    public static CommandBar? GetParentCommandBar(DependencyObject obj)
    => (CommandBar)obj.GetValue(ParentCommandBarProperty);

    /// <summary>
    /// 对指定的对象设置 <see cref="ParentCommandBarProperty"/> 附加属性的值
    /// </summary>
    public static void SetParentCommandBar(DependencyObject obj, CommandBar? value)
    => obj.SetValue(ParentCommandBarProperty, value);
}
