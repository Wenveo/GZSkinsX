// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition.Hosting;
using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml;

[assembly: InternalsVisibleTo("GZSkinsX.App")]

namespace GZSkinsX.Api.Appx;

public static partial class AppxContext
{
    /// <summary>
    /// 初始化生命周期服务
    /// </summary>
    /// <param name="parmas">应用程序初始化的参数</param>
    /// <param name="compositionHost"><seealso cref="CompositionHost"/> 对象的实例</param>
    /// <exception cref="ArgumentNullException"><paramref name="parmas"/> 或 <paramref name="serviceLocator"/> 的默认值为空</exception>
    internal static void InitializeLifetimeService(ApplicationInitializationCallbackParams parmas, CompositionHost compositionHost)
    {
        ArgumentNullException.ThrowIfNull(parmas);
        ArgumentNullException.ThrowIfNull(compositionHost);

        if (_compositionHost is not null)
        {
            throw new InvalidOperationException("The application lifetime service has been initialized!");
        }

        _compositionHost = compositionHost;
    }
}
