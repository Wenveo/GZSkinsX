// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

using Windows.Storage;

namespace GZSkinsX.Api.MRT;

/// <summary>
/// 提供对应用程序资源映射和本地化资源的访问权限
/// </summary>
public interface IMRTCoreService
{
    /// <summary>
    /// 获取与当前正在运行的应用程序的主包关联的 <seealso cref="IMRTCoreMap"/>
    /// </summary>
    IMRTCoreMap MainResourceMap { get; }

    /// <summary>
    /// 加载一个或多个包资源索引 (PRI) 文件，并将其内容添加到默认资源管理器
    /// </summary>
    /// <param name="files">要添加的包资源索引 (PRI) 文件</param>
    void LoadPriFiles(IEnumerable<IStorageFile> files);

    /// <summary>
    /// 卸载一个或多个包资源索引 (PRI) 文件
    /// </summary>
    /// <param name="files">要卸载的包资源索引 (PRI) 文件</param>
    void UnloadPriFiles(IEnumerable<IStorageFile> files);
}
