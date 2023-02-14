// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Navigation;

/// <summary>
/// 在主视图中显示的导航菜单项
/// </summary>
public interface INavigationViewItem
{
    /// <summary>
    /// 用于显示的名称
    /// </summary>
    string Header { get; }

    /// <summary>
    /// 默认显示的字形图标
    /// </summary>
    string Icon { get; }
}
