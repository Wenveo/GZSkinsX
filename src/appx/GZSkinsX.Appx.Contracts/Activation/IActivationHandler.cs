// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

using Microsoft.Windows.AppLifecycle;

namespace GZSkinsX.Contracts.Activation;

/// <summary>
/// 激活处理程序接口，使用该接口实现并处理应用程序激活事件。
/// </summary>
public interface IActivationHandler
{
    /// <summary>
    /// 根据传入的应用激活参数判断当前处理程序能否对其进行处理。
    /// </summary>
    /// <param name="args">应用激活参数。</param>
    /// <returns>当返回 true 时代表可处理，否则将返回 false。</returns>
    bool CanHandle(AppActivationArguments args);

    /// <summary>
    /// 表示对传入的应用激活参数进行处理的方法（可等待）。
    /// </summary>
    /// <param name="args">应用激活参数。</param>
    Task HandleAsync(AppActivationArguments args);
}
