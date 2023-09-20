// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 隐式扩展的触发类型。
/// </summary>
public enum ImplicitExtensionTrigger
{
    /// <summary>
    /// 在加载应用程序通用扩展之前。
    /// </summary>
    BeforeUniversalExtensions,
    /// <summary>
    /// 在加载完应用程序通用扩展之后。
    /// </summary>
    AfterUniversalExtensions,
    /// <summary>
    /// 在触发应用程序通用扩展的 <see cref="UniversalExtensionEvent.Loaded"/> 事件之后。
    /// </summary>
    AfterUniversalExtensionsLoaded,
    /// <summary>
    /// 在应用程序加载时。
    /// </summary>
    AppLoaded
}
