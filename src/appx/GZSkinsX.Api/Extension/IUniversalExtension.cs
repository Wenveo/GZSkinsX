// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace GZSkinsX.SDK.Extension;

/// <summary>
/// 应用程序的通用扩展接口，所有的 "应用程序扩展" 都继承于此
/// </summary>
public interface IUniversalExtension
{
    /// <summary>
    /// 表示该通用扩展的名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 表示该通用扩展的简短描述
    /// </summary>
    string ShortDescription { get; }

    /// <summary>
    /// 表示该通用扩展的版权信息
    /// </summary>
    string Copyright { get; }

    /// <summary>
    /// 用于合并的资源字典列表，资源路径必须是相对于程序集的
    /// </summary>
    IEnumerable<string> MergedResourceDictionaries { get; }

    /// <summary>
    /// 表示该通用扩展触发的事件
    /// </summary>
    /// <param name="e">触发的事件类型</param>
    void OnEvent(UniversalExtensionEvent e);
}
