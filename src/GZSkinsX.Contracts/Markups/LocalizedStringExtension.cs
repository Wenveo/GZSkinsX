// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Contracts.Helpers;

using Windows.UI.Xaml.Markup;

namespace GZSkinsX.Contracts.Markups;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class LocalizedStringExtension : MarkupExtension
{
    public string? ResourceKey { get; set; }

    protected override object ProvideValue()
    {
        if (string.IsNullOrEmpty(ResourceKey))
        {
            return string.Empty;
        }

        try
        {
#pragma warning disable CS8604
            return ResourceHelper.GetLocalized(ResourceKey);
#pragma warning restore CS8604
        }
        catch
        {
            return string.Empty;
        }
    }
}
