// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace GZSkinsX.Contracts.MyMods;

/// <summary>
/// 模组的视图模型 (ViewModel)，用于上下文绑定。
/// </summary>
/// <param name="modInfo">模组的基本信息。</param>
/// <param name="modImage">模组的图片路径。</param>
/// <param name="isInstalled">是否为已安装。</param>
/// <param name="indexOfTable">模组的安装顺序。</param>
public sealed partial class MyModViewModel(MyModInfo modInfo, Uri modImage, bool isInstalled, int indexOfTable) : ObservableObject
{
    /// <summary>
    /// 表示该模组的基本信息。
    /// </summary>
    [ObservableProperty]
    private MyModInfo _modInfo = modInfo;

    /// <summary>
    /// 表示该模组的图片路径。
    /// </summary>
    [ObservableProperty]
    private Uri _modImage = modImage;

    /// <summary>
    /// 表示该模组是否已安装。
    /// </summary>
    [ObservableProperty]
    private bool _isInstalled = isInstalled;

    /// <summary>
    /// 该模组位于安装表中的顺序。
    /// </summary>
    [ObservableProperty]
    private int _indexOfTable = indexOfTable;

    /// <summary>
    /// 是否显示该模组的安装顺序。
    /// </summary>
    [ObservableProperty]
    private bool _isShowIndex;
}
