// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Contracts.Logging;

/// <summary>
/// 定义日志记录严重性级别
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// 跟踪应用程序的常规流的日志。 这些日志应具有长期价值。
    /// </summary>
    Always,
    /// <summary>
    /// 在开发过程中用于交互式调查的日志。 这些日志应主要包含对调试有用的信息，并且没有长期价值
    /// </summary>
    Debug,
    /// <summary>
    /// 当前执行流因故障而停止时突出显示的日志。 这些日志指示当前活动中的故障，而不是应用程序范围内的故障
    /// </summary>
    Error,
    /// <summary>
    /// 表示常规操作执行完成的日志。这些日志用于表明执行流操作成功的状态 (与 Error 相对)，并且具有长期价值
    /// </summary>
    Okay,
    /// <summary>
    /// 突出显示应用程序流中的异常或意外事件 (不会导致应用程序执行停止) 的日志
    /// </summary>
    Warning
}
