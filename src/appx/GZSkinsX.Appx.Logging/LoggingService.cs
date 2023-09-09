// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;

using GZSkinsX.Contracts.Logging;

namespace GZSkinsX.Appx.Logging;

/// <inheritdoc cref="ILoggingService"/>
[Shared, Export(typeof(ILoggingService))]
internal sealed class LoggingService : ILoggingService
{
#pragma warning disable format
    private const string AlwaysString   =   "[ALWAYS]";
    private const string DebugString    =   "[DEBUG]";
    private const string ErrorString    =   "[ERROR]";
    private const string OkayString     =   "[OKAY]";
    private const string WarningString  =   "[WARN]";
#pragma warning restore format

    private const string LoggingMessageFormat =
        """
        {0:yyyy-MM-ddTHH:mm:ss.ffffffK} {1}
        {2}
        {3}

        """;

    /// <summary>
    /// 内部日志器默认实现。
    /// </summary>
    private readonly LoggerImpl _logger;

    /// <summary>
    /// 初始化 <see cref="LoggingService"/> 的新实例。
    /// </summary>
    public LoggingService()
    {
        _logger = LoggerImpl.Shared;
        LogAlways("GZSkinsX::Services::LoggingService",
            $"Logging started at {DateTimeOffset.Now}");
    }

    /// <inheritdoc/>
    public void LogAlways(string title, string message)
    {
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTimeOffset.Now, AlwaysString, title, message));
    }

    /// <inheritdoc/>
    public void LogDebug(string title, string message)
    {
#if DEBUG
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTimeOffset.Now, DebugString, title, message));
#endif
    }

    /// <inheritdoc/>
    public void LogError(string title, string message)
    {
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTimeOffset.Now, ErrorString, title, message));
    }

    /// <inheritdoc/>
    public void LogOkay(string title, string message)
    {
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTimeOffset.Now, OkayString, title, message));
    }

    /// <inheritdoc/>
    public void LogWarning(string title, string message)
    {
        _logger.Log(string.Format(LoggingMessageFormat,
            DateTimeOffset.Now, WarningString, title, message));
    }

    /// <inheritdoc/>
    public void Log(LogLevel level, string title, string message)
    {
        switch (level)
        {
            case LogLevel.Debug:
                LogDebug(title, message);
                break;
            case LogLevel.Error:
                LogError(title, message);
                break;
            case LogLevel.Okay:
                LogOkay(title, message);
                break;
            case LogLevel.Warning:
                LogWarning(title, message);
                break;
            default:
                LogAlways(title, message);
                break;
        }
    }
}
