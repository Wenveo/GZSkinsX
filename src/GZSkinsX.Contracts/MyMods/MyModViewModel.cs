// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using CommunityToolkit.Mvvm.ComponentModel;

using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace GZSkinsX.Contracts.MyMods;

public sealed partial class MyModViewModel(StorageFile modFile, ImageSource modImage,
    MyModInfo modInfo, bool enable, int indexOfTable) : ObservableObject
{
    [ObservableProperty]
    private StorageFile _modFile = modFile;

    [ObservableProperty]
    private ImageSource _modImage = modImage;

    [ObservableProperty]
    private MyModInfo _modInfo = modInfo;

    [ObservableProperty]
    private bool _enable = enable;

    [ObservableProperty]
    private int _indexOfTable = indexOfTable;

    [ObservableProperty]
    private bool _isShowIndex;
}
