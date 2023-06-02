// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace GZSkinsX.SDK.Settings;

/// <summary>
/// 提供存储应用程序设置的服务，可用于存储本地数据或漫游数据
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// 从当前应用程序设置中删除与指定的名称匹配的子节点配置
    /// </summary>
    /// <param name="name">要删除的子节点配置名称</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null</exception>
    void DeleteSection(string name);

    /// <summary>
    /// 从当前应用程序设置中删除与指定的名称及类型所匹配的子节点配置
    /// </summary>
    /// <param name="name">要删除的子节点配置名称</param>
    /// <param name="type">要删除的子节点配置类型</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null</exception>
    void DeleteSection(string name, SettingsType type);

    /// <summary>
    /// 从当前应用程序设置中获取或创建与指定的名称匹配的子节点配置
    /// </summary>
    /// <param name="name">要获取的子节点配置的名称</param>
    /// <returns>如果找到匹配的元素则会返回该对象；否则将会创建一个新的子节点配置</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null</exception>
    ISettingsSection GetOrCreateSection(string name);

    /// <summary>
    /// 从当前应用程序设置中获取或创建与指定的名称及类型所匹配的子节点配置
    /// </summary>
    /// <param name="name">要获取的子节点配置的名称</param>
    /// <param name="type">要获取的子节点配置类型</param>
    /// <returns>如果找到匹配的元素则会返回该对象；否则将会创建一个新的子节点配置</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null</exception>
    ISettingsSection GetOrCreateSection(string name, SettingsType type);
}
