// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace GZSkinsX.Extensions.CreatorStudio.Wad.ViewModels;

internal sealed partial class WadFolderNodeVM : WadItemNodeVM
{
    [ObservableProperty]
    private ObservableCollection<WadItemNodeVM> _children;

    public WadFolderNodeVM(string name, string fullPath) : base(name, fullPath, "\uE188")
    {
        _children = new ObservableCollection<WadItemNodeVM>();
    }

    public void AddChild(WadItemNodeVM item)
    {
        Children.Add(item);
    }

    public WadItemNodeVM? GetChild(string name)
    {
        for (var i = 0; i < Children.Count; i++)
        {
            var item = Children[i];
            if (StringComparer.OrdinalIgnoreCase.Equals(item.Name, name))
            {
                return item;
            }
        }

        return null;
    }

    public T? GetChild<T>(string name) where T : WadItemNodeVM
    {
        for (var i = 0; i < Children.Count; i++)
        {
            var item = Children[i] as T;
            if (item is not null && StringComparer.OrdinalIgnoreCase.Equals(item.Name, name))
            {
                return item;
            }
        }

        return null;
    }
}
