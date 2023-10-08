// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Command;

/// <summary>
/// 关于包装命令栏中的元素的容器接口。
/// </summary>
internal interface ICommandBarItemContainer
{
    /// <summary>
    /// 获取该容器提供的命令栏元素的对象实例。
    /// </summary>
    ICommandBarElement UIObject { get; }
}
