// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Contracts.Helpers;

using Microsoft.UI.Xaml.Markup;

namespace GZSkinsX.Contracts.Markups;

/// <summary>
/// 一个用于获取程序本地化资源的 XAML 标记扩展。
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class LocalizedStringExtension : MarkupExtension
{
    /// <summary>
    /// 获取或设置与目标资源所关联的键。
    /// </summary>
    public string? ResourceKey { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        if (string.IsNullOrEmpty(ResourceKey))
        {
            return string.Empty;
        }

        try
        {
            return ResourceHelper.GetLocalized(ResourceKey);
        }
        catch
        {
            return string.Empty;
        }
    }
}
