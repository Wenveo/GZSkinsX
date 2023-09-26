// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using GZSkinsX.Contracts.Appx;

namespace GZSkinsX.Appx.Logging;

/// <summary>
/// 应用程序的默认日志器的实现类。
/// </summary>
internal sealed class LoggerImpl : IDisposable
{
    /// <summary>
    /// 用于保证多线程下同步记录的线程锁对象。
    /// </summary>
    private readonly object _lockObj;

    /// <summary>
    /// 日志文件的输出流，只有被初始化后才可使用。
    /// </summary>
    private StreamWriter? _logWriter;

    /// <summary>
    /// 初始化 <see cref="LoggerImpl"/> 的新实例。
    /// </summary>
    public LoggerImpl()
    {
        _lockObj = new();
        Initialize();
    }

    /// <summary>
    /// 初始化当前日志器的文件输出流。
    /// </summary>
    [MemberNotNull(nameof(_logWriter))]
    private void Initialize()
    {
        if (_logWriter is null)
        {
            var logsFolder = Path.Combine(AppxContext.LocalFolder, "Logs");
            if (Directory.Exists(logsFolder) is false)
            {
                Directory.CreateDirectory(logsFolder);
            }

            var loggingFile = Path.Combine(logsFolder, string.Format("{0:yyyy-MM-ddTHH-mm-ss}_cor3.log", DateTime.Now));
            _logWriter = new StreamWriter(new FileStream(loggingFile, FileMode.Create, FileAccess.Write), Encoding.UTF8) { AutoFlush = true };
        }
    }

    /// <summary>
    /// 将需要记录的内容输出至日志文件流。
    /// </summary>
    /// <param name="message">要记录的内容。</param>
    public void Log(string message)
    {
        lock (_lockObj)
        {
            _logWriter?.WriteLine(message);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lockObj)
        {
            var logWriter = _logWriter;
            if (logWriter is null)
            {
                return;
            }

            _logWriter = null;

            logWriter.Close();
            logWriter.Dispose();
        }
    }
}
