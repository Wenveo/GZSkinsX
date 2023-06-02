// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;

using GZSkinsX.SDK.CreatorStudio.AssetsExplorer;

namespace GZSkinsX.Extensions.CreatorStudio.AssetsExplorer;

internal sealed class AssetsExplorerContainer : AssetsExplorerItem, IAssetsExplorerContainer
{
    public DirectoryInfo DirectoryInfo { get; }

    public List<AssetsExplorerItem> Children { get; }

    IEnumerable<IAssetsExplorerItem> IAssetsExplorerContainer.Children => Children;

    public AssetsExplorerContainer(DirectoryInfo directoryInfo) : base(directoryInfo.Name, "\uF12B")
    {
        DirectoryInfo = directoryInfo;
        Children = new List<AssetsExplorerItem>();
    }

    public void AddChild(AssetsExplorerItem item)
    {
        Children.Add(item);
    }

    public AssetsExplorerItem? GetChild(string name)
    {
        for (var i = 0; i < Children.Count; i++)
        {
            var item = Children[i];
            if (StringComparer.Ordinal.Equals(item.Name, name))
            {
                return item;
            }
        }

        return null;
    }

    public T? GetChild<T>(string name) where T : AssetsExplorerItem
    {
        for (var i = 0; i < Children.Count; i++)
        {
            var item = Children[i] as T;
            if (item is not null && StringComparer.Ordinal.Equals(item.Name, name))
            {
                return item;
            }
        }

        return null;
    }
}
