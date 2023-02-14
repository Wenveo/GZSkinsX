// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 自动加载的扩展的触发类型
/// </summary>
public enum AutoLoadedType
{
    /// <summary>
    /// 在加载扩展之前
    /// </summary>
    BeforeExtensions,
    /// <summary>
    /// 在加载完扩展之后
    /// </summary>
    AfterExtensions,
    /// <summary>
    /// 在触发扩展的 <see cref="ExtensionEvent.Loaded"/> 事件之后
    /// </summary>
    AfterExtensionsLoaded,
    /// <summary>
    /// 在应用程序加载时
    /// </summary>
    AppLoaded
}
