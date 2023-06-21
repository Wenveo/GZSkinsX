// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Extension;

/// <summary>
/// 通用扩展触发的事件类型
/// </summary>
public enum UniversalExtensionEvent
{
    /// <summary>
    /// 当扩展被加载时发生
    /// </summary>
    Loaded,

    /// <summary>
    /// 当应用加载时发生
    /// </summary>
    AppLoaded,

    /// <summary>
    /// 当应用退出时发生
    /// </summary>
    AppExit
}
