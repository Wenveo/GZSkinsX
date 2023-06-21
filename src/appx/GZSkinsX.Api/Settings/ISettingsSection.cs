// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace GZSkinsX.Api.Settings;

/// <summary>
/// 表示位于程序设置中的子节点配置
/// </summary>
public interface ISettingsSection
{
    /// <summary>
    /// 当前配置节点的名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 当前配置节点的类型
    /// </summary>
    SettingsType Type { get; }

    /// <summary>
    /// 获取与指定的键关联的值
    /// </summary>
    /// <param name="key">要获取的值的键</param>
    /// <returns>与指定的键相关联的值</returns>
    /// <exception cref="InvalidOperationException">当前对象已被释放或删除</exception>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 上声明的默认值为 null</exception>
    string? Attribute(string key);

    /// <summary>
    /// 获取与指定的键关联的值
    /// </summary>
    /// <typeparam name="TValue">指定值的类型</typeparam>
    /// <param name="key">要获取的值的键</param>
    /// <returns>与指定的键匹配的元素</returns>
    /// <exception cref="InvalidOperationException">当前对象已被释放或删除</exception>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 上声明的默认值为 null</exception>
    TValue? Attribute<TValue>(string key);

    /// <summary>
    /// 设置与指定的键关联的值
    /// </summary>
    /// <typeparam name="TValue">指定值的类型</typeparam>
    /// <param name="key">要设置的值的键</param>
    /// <param name="value">要设置的键的值</param>
    /// <exception cref="InvalidOperationException">当前对象已被释放或删除</exception>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 或 <paramref name="value"/> 上声明的默认值为 null</exception>
    void Attribute<TValue>(string key, TValue value);

    /// <summary>
    /// 从当前节点中删除与指定的键匹配的元素
    /// </summary>
    /// <param name="key">要删除的元素的键</param>
    /// <returns>如果在该节点中成功找到该元素并删除则返回 true，否则返回 false</returns>
    /// <exception cref="InvalidOperationException">当前对象已被释放或删除</exception>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> 上声明的默认值为 null</exception>
    bool Delete(string key);

    /// <summary>
    /// 从当前节点中删除与指定的名称匹配的子节点配置
    /// </summary>
    /// <param name="name">要删除的子节点配置的名称</param>
    void DeleteSection(string name);

    /// <summary>
    /// 从当前节点中获取或创建与指定的名称匹配的子节点配置
    /// </summary>
    /// <param name="name">要获取的子节点配置的名称</param>
    /// <returns>如果找到匹配的元素则会返回该对象；否则将会创建一个新的子节点配置</returns>
    /// <exception cref="InvalidOperationException">当前对象已被释放或删除</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> 上声明的默认值为 null</exception>
    ISettingsSection GetOrCreateSection(string name);
}
