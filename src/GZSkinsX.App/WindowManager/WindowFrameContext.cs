// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Contracts.WindowManager;

namespace GZSkinsX.WindowManager;

/// <inheritdoc cref="IWindowFrameContext"/>
internal sealed class WindowFrameContext(Lazy<IWindowFrame, WindowFrameMetadataAttribute> lazy) : IWindowFrameContext
{
    /// <summary>
    /// 当前上下文中的懒加载对象
    /// </summary>
    private readonly Lazy<IWindowFrame, WindowFrameMetadataAttribute> _lazy = lazy;

    /// <inheritdoc/>
    public IWindowFrame Value => _lazy.Value;

    /// <inheritdoc/>
    public WindowFrameMetadataAttribute Metadata => _lazy.Metadata;
}
