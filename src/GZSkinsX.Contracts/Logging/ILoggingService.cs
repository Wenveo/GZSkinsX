// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Logging;

/// <summary>
/// 基础日志服务，可通过此将记录内容实时输出至本地
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// 设置常规日志消息格式并写入该消息
    /// </summary>
    /// <param name="message">要写入的日志消息字符串</param>
    void LogAlways(string message);

    /// <summary>
    /// 设置调试日志消息格式并写入该消息
    /// </summary>
    /// <param name="message">要写入的日志消息字符串</param>
    void LogDebug(string message);

    /// <summary>
    /// 设置错误日志消息格式并写入该消息
    /// </summary>
    /// <param name="message">要写入的日志消息字符串</param>
    void LogError(string message);

    /// <summary>
    /// 设置执行成功的日志消息格式并写入该消息
    /// </summary>
    /// <param name="message">要写入的日志消息字符串</param>
    void LogOkay(string message);

    /// <summary>
    /// 设置警告日志消息格式并写入该消息
    /// </summary>
    /// <param name="message">要写入的日志消息字符串</param>
    void LogWarning(string message);

    /// <summary>
    /// 在指定的日志级别设置日志消息格式并写入该消息
    /// </summary>
    /// <param name="level">将在此级别上写入项</param>
    /// <param name="message">要写入的日志消息字符串</param>
    void Log(LogLevel level, string message);
}
