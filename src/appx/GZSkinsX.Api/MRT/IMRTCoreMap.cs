// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;

namespace GZSkinsX.Api.MRT;

/// <summary>
/// 相关资源的集合，通常用于获取本地化资源内容
/// </summary>
public interface IMRTCoreMap
{
    /// <summary>
    /// 获取当前资源图中所包含的资源的总数
    /// </summary>
    uint ResourceCount { get; }

    /// <summary>
    /// 在当前资源图中获取与指定的资源标识符所匹配的本地化资源
    /// </summary>
    /// <param name="resourceKey">指定为名称或引用的资源标识符</param>
    /// <returns>与标识符符合的本地化资源的字节数组数据</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> 上声明的默认值为 null</exception>
    byte[] GetBytes(string resourceKey);

    /// <summary>
    /// 在当前资源图中获取与指定的资源标识符所匹配的本地化资源
    /// </summary>
    /// <param name="resourceKey">指定为名称或引用的资源标识符</param>
    /// <returns>与标识符符合的本地化资源的字符串内容</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resourceKey"/> 上声明的默认值为 null</exception>
    string GetString(string resourceKey);

    /// <summary>
    /// 在当前资源图中获取特定的资源子集
    /// </summary>
    /// <param name="reference">用于标识新子树根的资源映射标识符</param>
    /// <returns>与标识符所符合的子树资源图</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reference"/> 上声明的默认值为 null</exception>
    IMRTCoreMap GetSubtree(string reference);

    /// <summary>
    /// 尝试当前资源图中获取与指定的资源标识符所匹配的本地化资源
    /// </summary>
    /// <param name="resourceKey">指定为名称或引用的资源标识符</param>
    /// <param name="bytes">已获取到的与标识符符合的本地化资源的字节数组数据</param>
    /// <returns>如果在当前资源图中找到匹配的本地化资源则返回 true，否则返回 false</returns>
    bool TryGetBytes(string resourceKey, [NotNullWhen(true)] out byte[]? bytes);

    /// <summary>
    /// 尝试当前资源图中获取与指定的资源标识符所匹配的本地化资源
    /// </summary>
    /// <param name="resourceKey">指定为名称或引用的资源标识符</param>
    /// <param name="value">已获取到的与标识符符合的本地化资源的字符串内容</param>
    /// <returns>如果在当前资源图中找到匹配的本地化资源则返回 true，否则返回 false</returns>
    bool TryGetString(string resourceKey, [NotNullWhen(true)] out string? value);

    /// <summary>
    /// 尝试在当前资源图中获取特定的资源子集
    /// </summary>
    /// <param name="reference">用于标识新子树根的资源映射标识符</param>
    /// <param name="mrtCoreMap">已获取到的与标识符所符合的子树资源图</param>
    /// <returns>如果在当前资源图中找到匹配的资源子集则返回 true，否则返回 false</returns>
    bool TryGetSubtree(string reference, [NotNullWhen(true)] out IMRTCoreMap? mrtCoreMap);
}
