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
/// 该接口为 <see cref="IActivationHandler"/> 的次要接口，提供一些额外的接口成员/方法定义。
/// </summary>
public interface IActivationHandler2
{
    /// <summary>
    /// 根据传入的应用激活参数判断当前处理程序能否对其进行处理（可等待）。
    /// </summary>
    /// <param name="args">应用激活参数。</param>
    /// <returns>当返回 true 时代表可处理，否则将返回 false。</returns>
    Task<bool> CanHandleAsync(AppActivationArguments args);
}
