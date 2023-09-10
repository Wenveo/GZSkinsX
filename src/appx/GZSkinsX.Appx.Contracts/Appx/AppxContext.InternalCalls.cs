// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.Composition;

[assembly: InternalsVisibleTo("GZSkinsX")]

namespace GZSkinsX.Contracts.Appx;

public static partial class AppxContext
{
    /// <summary>
    /// 初始化应用程序的生命周期服务。
    /// </summary>
    /// <param name="exportProvider">组件容器的 <seealso cref="ExportProvider"/> 对象的实例。</param>
    /// <exception cref="ArgumentNullException"><paramref name="exportProvider"/> 的默认值为空。</exception>
    internal static void InitializeLifetimeService(ExportProvider exportProvider)
    {
        ArgumentNullException.ThrowIfNull(exportProvider);

        if (_exportProvider is not null)
        {
            throw new InvalidOperationException("The lifetime service has been initialized!");
        }

        _exportProvider = exportProvider;
    }
}
