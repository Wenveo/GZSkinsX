// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.Command;

namespace GZSkinsX.Contracts.MyMods;

/// <summary>
/// 有关模组视图中的命令栏 UI 上下文信息。
/// </summary>
/// <param name="MyModsView">模组视图对象实例。</param>
public sealed record class MyModsViewCommandBarUIContext(IMyModsView MyModsView) : ICommandBarUIContext
{

}
