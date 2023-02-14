// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Diagnostics;

using GZSkinsX.Api.Composition;

using Microsoft.VisualStudio.Composition;

namespace GZSkinsX.Composition;

/// <inheritdoc cref="IServiceLocator"/>
[Shared]
[Export, Export(typeof(IServiceLocator))]
internal sealed class ServiceLocator : IServiceLocator
{
    /// <summary>
    /// 用于获取导出类型的实例
    /// </summary>
    private ExportProvider? _exportProvider;

    /// <inheritdoc/>
    public T Resolve<T>() where T : class
    {
        Debug.Assert(_exportProvider != null);
        if (_exportProvider == null)
        {
            throw new InvalidOperationException();
        }

        return _exportProvider.GetExportedValue<T>();
    }

    /// <inheritdoc/>
    public T? TryResolve<T>() where T : class
    {
        Debug.Assert(_exportProvider != null);
        if (_exportProvider == null)
        {
            return null;
        }

        try
        {
            return _exportProvider.GetExportedValue<T>();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 设置当前类的 <see cref="ExportProvider"/> 容器实例
    /// </summary>
    /// <param name="exportProvider"><see cref="ExportProvider"/> 对象的实例</param>
    internal void SetExportProvider(ExportProvider exportProvider)
    => _exportProvider = exportProvider;
}
