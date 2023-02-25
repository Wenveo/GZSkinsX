// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using GZSkinsX.Api.WindowManager;

namespace GZSkinsX.WindowManager;

/// <summary>
/// 用于存储导出的 <see cref="IViewElement"/> 以及元数据 <see cref="ViewElementMetadataAttribute"/> 的上下文
/// </summary>
internal sealed class ViewElementContext
{
    /// <summary>
    /// 当前上下文中的懒加载对象
    /// </summary>
    private readonly Lazy<IViewElement, ViewElementMetadataAttribute> _lazy;

    /// <summary>
    /// 获取当前懒加载对象的值
    /// </summary>
    public IViewElement Value => _lazy.Value;

    /// <summary>
    /// 获取当前懒加载对象的元数据
    /// </summary>
    public ViewElementMetadataAttribute Metadata => _lazy.Metadata;

    /// <summary>
    /// 初始化 <see cref="ViewElementContext"/> 的新实例
    /// </summary>
    public ViewElementContext(Lazy<IViewElement, ViewElementMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
