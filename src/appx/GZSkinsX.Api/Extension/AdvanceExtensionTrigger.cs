// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.SDK.Extension;

/// <summary>
/// 先行扩展的触发类型
/// </summary>
public enum AdvanceExtensionTrigger
{
    /// <summary>
    /// 在加载通用扩展之前
    /// </summary>
    BeforeUniversalExtensions,

    /// <summary>
    /// 在加载完通用扩展之后
    /// </summary>
    AfterUniversalExtensions,

    /// <summary>
    /// 在触发通用扩展的 <see cref="UniversalExtensionEvent.Loaded"/> 事件之后
    /// </summary>
    AfterUniversalExtensionsLoaded,

    /// <summary>
    /// 在应用程序加载时
    /// </summary>
    AppLoaded
}
