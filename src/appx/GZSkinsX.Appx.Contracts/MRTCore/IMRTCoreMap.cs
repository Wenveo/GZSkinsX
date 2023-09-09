// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace GZSkinsX.Contracts.MRTCore;

/// <summary>
/// 相关资源的集合，通常用于获取本地化资源内容。
/// </summary>
public interface IMRTCoreMap
{
    /// <summary>
    /// 获取默认上下文中与指定的资源标识符所匹配的本地化资源。
    /// </summary>
    /// <param name="resourceKey">指定为名称或引用的资源标识符。</param>
    /// <returns>与标识符符合的本地化资源的字节数组数据。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> 上声明的默认值为 null。</exception>
    byte[] GetBytes(string resourceKey);

    /// <summary>
    /// 获取默认上下文中与指定的资源标识符所匹配的本地化资源。
    /// </summary>
    /// <param name="resourceKey">指定为名称或引用的资源标识符。</param>
    /// <returns>与标识符符合的本地化资源的字符串内容。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> 上声明的默认值为 null。</exception>
    string GetString(string resourceKey);

    /// <summary>
    /// 从当前默认上下文中获取特定的资源子集。
    /// </summary>
    /// <param name="reference">用于标识新子树根的资源映射标识符。</param>
    /// <returns>子树 <seealso cref="IMRTCoreMap"/> 类型的实例。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reference"/> 上声明的默认值为 null。</exception>
    IMRTCoreMap GetSubtree(string reference);
}
