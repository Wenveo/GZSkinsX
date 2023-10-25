// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.ContextMenu;

using Microsoft.UI.Xaml;

namespace GZSkinsX.Contracts.Appx;

/// <summary>
/// 有关全局菜单的 UI 上下文信息。
/// </summary>
/// <param name="UIObject">浮动菜单对象实例。</param>
/// <param name="CurrentTheme">当前应用使用的主题。</param>
public sealed record class MainGlobalMenuUIContext(object UIObject, ElementTheme CurrentTheme) : IContextMenuUIContext
{

}
