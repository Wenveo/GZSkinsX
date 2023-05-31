// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using CommunityToolkit.Mvvm.ComponentModel;

namespace GZSkinsX.Extensions.CreatorStudio.Wad.ViewModels;

internal abstract partial class WadItemNodeVM : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _fullPath;

    [ObservableProperty]
    private string _glyph;

    public WadItemNodeVM(string name, string fullPath, string glyph)
    {
        _name = name;
        _fullPath = fullPath;
        _glyph = glyph;
    }
}
