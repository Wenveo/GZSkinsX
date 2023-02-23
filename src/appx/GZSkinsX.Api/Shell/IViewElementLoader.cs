// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Api.Shell;

/// <summary>
/// 表示对页面进行初始化等操作的一个中间组件
/// </summary>
public interface IViewElementLoader : IViewElement
{
    /// <summary>
    /// 当页面初始化时触发，可对目标页面属性进行更改及调整
    /// </summary>
    /// <param name="viewElement">目标视图对象</param>
    void OnInitialized(Page viewElement);
}
