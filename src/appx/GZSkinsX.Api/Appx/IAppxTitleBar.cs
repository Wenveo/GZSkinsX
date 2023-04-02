// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

namespace GZSkinsX.Api.Appx;

/// <summary>
/// 提供对当前窗口标题栏管理的能力
/// </summary>
public interface IAppxTitleBar
{
    /// <summary>
    /// 获取和设置是否将当前窗口中的内容视图扩展至标题栏
    /// </summary>
    bool ExtendViewIntoTitleBar { get; set; }

    /// <summary>
    /// 设置当前窗口标题栏的界面元素
    /// </summary>
    /// <param name="value">需要设为标题栏的 UI 元素</param>
    void SetTitleBar(Windows.UI.Xaml.UIElement? value);
}
