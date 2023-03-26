// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using GZSkinsX.Api.Logging;
using GZSkinsX.DotNet.Diagnostics;

using Windows.Storage;

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

    private const string LoggingFileNameFormat  = "{0:yyyy-MM-ddTHH-mm-ss}_cor3.log";
    private const string LoggingMessageFormat   = "{0:yyyy-MM-ddTHH:mm:ss}| {1}| {2}";
    private const string LoggingStartedFormat   = "Logging started at {0:yyyy-MM-ddThh:mm:ss.ffff}";
    private const string LogsFolderNameString   = "Logs";
#pragma warning restore format

    private IStorageFolder? _logsFolder;
    private IStorageFile? _loggingFile;
    private StreamWriter? _logWriter;
    private readonly object _lockObj;
    private bool _isInitialize;

    /// <summary>
    /// 初始化 <see cref="LoggingService"/> 的新实例
    /// </summary>
    public LoggingService()
    {
        _lockObj = new object();
    }

    internal async Task InitializeAsync()
    {
        if (_isInitialize)
        {
            return;
        }

        _logsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(
        LogsFolderNameString, CreationCollisionOption.OpenIfExists);

        _loggingFile = await _logsFolder.CreateFileAsync(
            string.Format(LoggingFileNameFormat, DateTime.Now),
            CreationCollisionOption.ReplaceExisting);

        var stream = await _loggingFile.OpenStreamForWriteAsync();
        _logWriter = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        _isInitialize = true;
        LogAlways(string.Format(LoggingStartedFormat, DateTime.Now));
    }

    internal void CloseOutputStream()
    {
        lock (_lockObj)
        {
            if (_logWriter is not null)
            {
                _logWriter.Close();
                _logWriter.Dispose();

                _logWriter = null;
            }
        }
    }

    /// <inheritdoc/>
    public void LogAlways(string message)
    {
        Debug2.Assert(_logWriter is not null);

        lock (_lockObj)
        {
            _logWriter.WriteLine(string.Format(LoggingMessageFormat,
                DateTime.Now, AlwaysString, message));
        }
    }

    /// <inheritdoc/>
    public void LogDebug(string message)
    {
#if DEBUG
        Debug2.Assert(_logWriter is not null);

        lock (_lockObj)
        {
            _logWriter.WriteLine(string.Format(LoggingMessageFormat,
                DateTime.Now, DebugString, message));
        }
#endif
    }

    /// <inheritdoc/>
    public void LogError(string message)
    {
        Debug2.Assert(_logWriter is not null);

        lock (_lockObj)
        {
            _logWriter.WriteLine(string.Format(LoggingMessageFormat,
                DateTime.Now, ErrorString, message));
        }
    }

    /// <inheritdoc/>
    public void LogOkay(string message)
    {
        Debug2.Assert(_logWriter is not null);

        lock (_lockObj)
        {
            _logWriter.WriteLine(string.Format(LoggingMessageFormat,
                DateTime.Now, OkayString, message));
        }
    }

    /// <inheritdoc/>
    public void LogWarning(string message)
    {
        Debug2.Assert(_logWriter is not null);

        lock (_lockObj)
        {
            _logWriter.WriteLine(string.Format(LoggingMessageFormat,
                DateTime.Now, WarningString, message));
        }
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
