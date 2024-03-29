// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 关于自动加载的扩展的激活规则。
/// </summary>
public enum AutoLoadedActivationConstraint
{
    /// <summary>
    /// 在加载应用扩展之前。
    /// </summary>
    BeforeExtensions,

    /// <summary>
    /// 在加载完应用扩展之后。
    /// </summary>
    AfterExtensions,

    /// <summary>
    /// 在应用载入完成后激活。
    /// </summary>
    AppLoaded,
}
