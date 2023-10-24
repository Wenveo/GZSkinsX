// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 用于接管导航视图中的搜索行为的接口定义。
/// </summary>
public interface INavigationViewSearchHolder
{
    /// <summary>
    /// 获取用于搜索框的占位文本。
    /// </summary>
    string GetPlaceholderText();

    /// <summary>
    /// 在搜索文本更改后触发的处理方法。
    /// </summary>
    /// <param name="sender">与搜索行为相关联的导航视图。</param>
    /// <param name="newText">新的搜索文本字符串。</param>
    void OnSearchTextChanged(INavigationViewManager sender, string? newText);
}
