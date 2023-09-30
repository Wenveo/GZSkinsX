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
/// 应用程序激活处理服务，提供对激活处理程序的管理和激活事件处理。
/// </summary>
public interface IActivationService
{
    /// <summary>
    /// 对传入的应用激活参数进行处理（可等待）。
    /// </summary>
    /// <param name="args">应用激活参数。</param>
    Task ActivateAsync(AppActivationArguments args);

    /// <summary>
    /// 对目标的处理程序进行注册。
    /// </summary>
    /// <param name="handler">需要注册的激活处理程序。</param>
    void RegisterHandler(IActivationHandler handler);

    /// <summary>
    /// 取消注册目标处理程序。
    /// </summary>
    /// <param name="handler">需要取消注册的激活处理程序。</param>
    void UnregisterHandler(IActivationHandler handler);
}
