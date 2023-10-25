// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.ContextMenu;

namespace GZSkinsX.Contracts.MyMods;

/// <summary>
/// 有关模组视图中的上下文菜单的 UI 上下文信息。
/// </summary>
/// <param name="UIObject">浮动菜单对象实例。</param>
/// <param name="MyModsView">模组视图对象实例。</param>
public sealed record class MyModsViewContextMenuUIContext(object UIObject, IMyModsView MyModsView) : IContextMenuUIContext
{

}
