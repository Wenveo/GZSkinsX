// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

namespace GZSkinsX.Appx.Navigation;

/// <summary>
/// 
/// </summary>
internal sealed class QueryNavigationItem
{
    /// <summary>
    /// 
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 
    /// </summary>
    public Guid Guid { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="guid"></param>
    public QueryNavigationItem(string title, Guid guid)
    {
        Title = title;
        Guid = guid;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Title;
    }
}
