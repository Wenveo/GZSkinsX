// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.MRTCore;

/// <summary>
/// 提供对应用程序资源映射和本地化资源的访问。
/// </summary>
public interface IMRTCoreService
{
    /// <summary>
    /// 获取与当前正在运行的应用程序的主包关联的 <seealso cref="IMRTCoreMap"/> 实例。
    /// </summary>
    IMRTCoreMap MainResourceMap { get; }

    /// <summary>
    /// 根据指定的包资源索引 (PRI) 文件载入 <seealso cref="IMRTCoreMap"/> 资源图实例。
    /// </summary>
    /// <param name="priFile">要载入的包资源索引 (PRI) 文件。</param>
    /// <returns>返回已加载的 <seealso cref="IMRTCoreMap"/> 资源图实例。</returns>
    IMRTCoreMap LoadPriFile(string priFile);
}
