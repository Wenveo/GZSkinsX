// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.Composition;

[assembly: InternalsVisibleTo("GZSkinsX, PublicKey=00240000048000009400000006020000002400005253413100040000010001006d13ff08810f5c0937e5d36923b65fcd450723277974d8777efe5f5d919a78e8c9030d11f54f6044d3ecb2c1d94ea0ede3fb339774d048fbb9449e7f773ab757d5fd3c42351753d3ab19b8b68286ef28fd131441db8851cdf5d9853de411bfd62b08c260b13f8c65c8b1eb3df5c4fef891ec959dce72595cd109202cdbdfde9b")]

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
