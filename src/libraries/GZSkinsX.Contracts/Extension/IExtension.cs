// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Extension;

/// <summary>
/// 应用程序扩展接口，所有的 "应用程序扩展" 都继承于此
/// </summary>
public interface IExtension
{
    /// <summary>
    /// 应用程序扩展的名称
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// 应用程序扩展的简短描述
    /// </summary>
    string? ShortDescription { get; set; }

    /// <summary>
    /// 应用程序扩展的版权信息
    /// </summary>
    string? Copyright { get; set; }

    /// <summary>
    /// 用于合并的资源字典列表，资源路径必须是相对于程序集的
    /// </summary>
    IEnumerable<string> MergedResourceDictionaries { get; }

    /// <summary>
    /// 应用程序扩展事件
    /// </summary>
    /// <param name="e">触发的事件类型</param>
    void OnEvent(ExtensionEvent e);
}
