// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.ContextMenu;

/// <summary>
/// 表示用作回调的方法，用于重新计算上下文菜单中的 UI 上下文内容
/// </summary>
/// <param name="sender">该上下文菜单的 UI 对象</param>
/// <param name="e">该上下文菜单打开时所传入的参数</param>
/// <returns>返回重新计算过的上下文内容</returns>
public delegate IContextMenuUIContext CoerceContextMenuUIContextCallback(object sender, object e);
