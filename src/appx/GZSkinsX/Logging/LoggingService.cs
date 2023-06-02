// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

using GZSkinsX.SDK.Logging;

namespace GZSkinsX.Logging;

/// <inheritdoc cref="ILoggingService"/>
[Shared, Export(typeof(ILoggingService))]
internal sealed class LoggingService : ILoggingService
{
#pragma warning disable format
    private const string AlwaysString   =   "ALWAYS";
    private const string DebugString    =   " DEBUG";
    private const string ErrorString    =   " ERROR";
    private const string OkayString     =   "  OKAY";
    private const string WarningString  =   "  WARN";

    private const string LoggingMessageFormat   = "{0:yyyy-MM-ddTHH:mm:ss}| {1}| {2}";
    private const string LoggingStartedFormat   = "Logging started at {0:yyyy-MM-ddThh:mm:ss.ffff}";
#pragma warning restore format

    /// <summary>
    /// 内部日志器默认实现
    /// </summary>
    private readonly LoggerImpl _logger;

    /// <summary>
    /// 初始化 <see cref="LoggingService"/> 的新实例
    /// </summary>
    public LoggingService()
    {
        _logger = LoggerImpl.Shared;
        LogAlways(string.Format(LoggingStartedFormat, DateTime.Now));
    }

    /// <inheritdoc/>
    public void LogAlways(string message)
    {
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTime.Now, AlwaysString, message));
    }

    /// <inheritdoc/>
    public void LogDebug(string message)
    {
#if DEBUG
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTime.Now, DebugString, message));
#endif
    }

    /// <inheritdoc/>
    public void LogError(string message)
    {
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTime.Now, ErrorString, message));
    }

    /// <inheritdoc/>
    public void LogOkay(string message)
    {
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTime.Now, OkayString, message));
    }

    /// <inheritdoc/>
    public void LogWarning(string message)
    {
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTime.Now, WarningString, message));
    }

    /// <inheritdoc/>
    public void Log(LogLevel level, string message)
    {
        switch (level)
        {
            case LogLevel.Debug:
                LogDebug(message);
                break;
            case LogLevel.Error:
                LogError(message);
                break;
            case LogLevel.Okay:
                LogOkay(message);
                break;
            case LogLevel.Warning:
                LogWarning(message);
                break;
            default:
                LogAlways(message);
                break;
        }
    }
}
