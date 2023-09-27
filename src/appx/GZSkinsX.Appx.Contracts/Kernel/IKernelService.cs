// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;

namespace GZSkinsX.Contracts.Kernel;

/// <summary>
/// 提供对核心模块的访问端口。
/// </summary>
public interface IKernelService
{
    /// <summary>
    /// 初始化核心模块 (非必需)。
    /// <para>
    /// 由于模块初始化需要耗费一定的时间，如果能够提前进行调用，那么将能减少之后的首次操作所需的时间。
    /// </para>
    /// </summary>
    void InitializeModule();

    /// <summary>
    /// 确认组件服务是否存活。
    /// </summary>
    /// <returns>当服务组件处于运行中时返回 true，否则返回 false。</returns>
    bool EnsureMotClientAlive();

    /// <summary>
    /// 将输入的字符串加密为特定的配置格式文本。
    /// </summary>
    /// <param name="str">输入的字符串内容。</param>
    /// <returns>加密后的字符串结果。</returns>
    string EncryptConfigText(string str);

    /// <summary>
    /// 将特定的配置格式文本解密字符串原内容。
    /// </summary>
    /// <param name="str">加密后的配置格式文本。</param>
    /// <returns>解密后的字符串原内容。</returns>
    string DecryptConfigText(string str);

    /// <summary>
    /// 提取指定的格子模组文件中的图片并输出至目标路径。
    /// </summary>
    /// <param name="input">格子模组文件的路径。</param>
    /// <param name="output">图片的输出路径。</param>
    /// <exception cref="System.InvalidOperationException">当核心模块不存在或输入的文件无效时抛出。</exception>
    void ExtractWGZModImage(string input, string output);

    /// <summary>
    /// 读取指定的格子模组文件中的信息。
    /// </summary>
    /// <param name="filePath">格子模组文件的路径。</param>
    /// <returns>返回已读取的格子模组文件的信息。</returns>
    /// <exception cref="System.InvalidOperationException">当核心模块不存在或输入的文件无效时抛出。</exception>
    WGZModInfo ReadWGZModInfo(string filePath);

    /// <summary>
    /// 更新本地的模块清单配置信息。
    /// </summary>
    Task UpdateManifestAsync();

    /// <summary>
    /// 从在线服务器中下载和更新核心组件。
    /// </summary>
    /// <param name="progress">用于报告下载进度更新。</param>
    Task UpdateModuleAsync(IProgress<double>? progress = null);


    /// <summary>
    /// 验证本地已下载的核心模块的完整性。
    /// </summary>
    /// <returns>当对本地的核心模块完成验证后返回 true，否则返回 false。</returns>
    bool VerifyModuleIntegrity();
}
