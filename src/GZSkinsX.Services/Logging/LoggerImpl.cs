// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

[assembly: InternalsVisibleTo("GZSkinsX")]

namespace GZSkinsX.Services.Logging;

/// <summary>
/// 应用程序的默认日志器的实现类
/// </summary>
internal sealed class LoggerImpl
{
    /// <summary>
    /// 用于获取日志器实例的懒加载容器
    /// </summary>
    private static readonly Lazy<LoggerImpl> s_lazy = new(() => new());

    /// <summary>
    /// 获取全局静态共享的默认日志器的实例
    /// </summary>
    public static LoggerImpl Shared => s_lazy.Value;

    /// <summary>
    /// 用于保证多线程下同步记录的线程锁对象
    /// </summary>
    private readonly object _lockObj;

    /// <summary>
    /// 日志文件的输出流，只有被初始化后才可使用
    /// </summary>
    private StreamWriter? _logWriter;

    /// <summary>
    /// 初始化 <see cref="LoggerImpl"/> 的新实例
    /// </summary>
    private LoggerImpl()
    {
        _lockObj = new object();
    }

    /// <summary>
    /// 初始化当前日志器的文件输出流
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_logWriter is null)
        {
            var _logsFolder = await ApplicationData.Current.LocalFolder
                .CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);

            var _loggingFile = await _logsFolder.CreateFileAsync(
                string.Format("{0:yyyy-MM-ddTHH-mm-ss}_cor3.log", DateTime.Now),
                    CreationCollisionOption.ReplaceExisting);

            var stream = await _loggingFile.OpenStreamForWriteAsync();
            _logWriter = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
        }
    }

    /// <summary>
    /// 将需要记录的内容输出至日志文件流
    /// </summary>
    /// <param name="message">要记录的内容</param>
    public void Log(string message)
    {
        lock (_lockObj)
        {
            _logWriter?.WriteLine(message);
        }
    }

    /// <summary>
    /// 关闭和释放当前日志输出流
    /// </summary>
    public void CloseOutputStream()
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
}
