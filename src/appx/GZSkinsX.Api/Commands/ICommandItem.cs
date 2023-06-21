// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Commands;

/// <summary>
/// 该接口为所有命令项的基接口，所有命令项都应继承于此，并以该接口类型导出
/// </summary>
public interface ICommandItem
{
    /// <summary>
    /// 表示在 UI 初始化时触发的行为
    /// </summary>
    void OnInitialize();
}
