// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Navigation;

/// <summary>
/// 提供对主视图中的页面操作和管理的能力
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 能否向后导航
    /// </summary>
    bool CanGoback { get; }

    /// <summary>
    /// 能否向前导航
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// 向后导航
    /// </summary>
    void GoBack();

    /// <summary>
    /// 向前导航
    /// </summary>
    void GoForward();
}
