// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Storage;

namespace GZSkinsX.Contracts.MyMods;

/// <summary>
/// 表示模组视图的接口，提供与 UI 视图交互相关的功能。
/// </summary>
public interface IMyModsView
{
    /// <summary>
    /// 获取或设置用于呈现集合项的筛选条件。
    /// </summary>
    string? ItemsFilter { get; set; }

    /// <summary>
    /// 获取原始的、只读的模组集合对象（无序）。
    /// </summary>
    IReadOnlyCollection<MyModItemViewModel> RawCollection { get; }

    /// <summary>
    /// 获取经过筛选后的集合项（该结果与 UI 中呈现的集合相同，并且为有序集合）。
    /// </summary>
    IEnumerable<MyModItemViewModel> FilteredItems { get; }

    /// <summary>
    /// 获取模组视图中选择的项。
    /// </summary>
    MyModItemViewModel? SelectedItem { get; }

    /// <summary>
    /// 获取模组视图中选择的项的集合。
    /// </summary>
    IEnumerable<MyModItemViewModel> SelectedItems { get; }

    /// <summary>
    /// 当模组视图中的选择项改变时触发的事件。
    /// </summary>
    event EventHandler<MyModsViewSelectionChangedArgs>? SelectionChanged;

    /// <summary>
    /// 将指定的元素从模组视图中删除（此操作会将其文件一同删除）。
    /// </summary>
    /// <param name="itemsToDelete">需要删除的集合项。</param>
    Task DeleteAsync(IEnumerable<MyModItemViewModel> itemsToDelete);

    /// <summary>
    /// 将指定的元素从模组视图中删除（此操作会将其文件一同删除）。
    /// </summary>
    /// <param name="itemsToDelete">需要删除的集合项。</param>
    Task DeleteAsync(params MyModItemViewModel[] itemsToDelete);

    /// <summary>
    /// 将指定的文件导入至模组视图中。
    /// </summary>
    /// <param name="files">需要导入的文件。</param>
    Task ImportAsync(IEnumerable<StorageFile> files);

    /// <summary>
    /// 将指定的文件导入至模组视图中。
    /// </summary>
    /// <param name="files">需要导入的文件。</param>
    Task ImportAsync(params StorageFile[] files);

    /// <summary>
    /// 将指定的文件导入至模组视图中。
    /// </summary>
    /// <param name="paths">需要导入的文件的路径。</param>
    Task ImportAsync(IEnumerable<string> paths);

    /// <summary>
    /// 将指定的文件导入至模组视图中。
    /// </summary>
    /// <param name="paths">需要导入的文件的路径。</param>
    Task ImportAsync(params string[] paths);

    /// <summary>
    /// 对指定的元素进行安装操作。
    /// </summary>
    /// <param name="itemsToInstall">需要安装的项。</param>
    Task InstallAsync(IEnumerable<MyModItemViewModel> itemsToInstall);

    /// <summary>
    /// 对指定的元素进行安装操作。
    /// </summary>
    /// <param name="itemsToInstall">需要安装的项。</param>
    Task InstallAsync(params MyModItemViewModel[] itemsToInstall);

    /// <summary>
    /// 对指定的元素进行卸载操作。
    /// </summary>
    /// <param name="itemsToUninstall">需要卸载的项。</param>
    Task UninstallAsync(IEnumerable<MyModItemViewModel> itemsToUninstall);

    /// <summary>
    /// 对指定的元素进行卸载操作。
    /// </summary>
    /// <param name="itemsToUninstall">需要卸载的项。</param>
    Task UninstallAsync(params MyModItemViewModel[] itemsToUninstall);

    /// <summary>
    /// 刷新集合列表。
    /// </summary>
    Task RefreshAsync();

    /// <summary>
    /// 设置模组视图中选择的集合项。
    /// </summary>
    /// <param name="itemsToSelect">需要选定的项。</param>
    void SetSelectedItems(IEnumerable<MyModItemViewModel> itemsToSelect);

    /// <summary>
    /// 设置模组视图中选择的集合项。
    /// </summary>
    /// <param name="itemsToSelect">需要选定的项。</param>
    void SetSelectedItems(params MyModItemViewModel[] itemsToSelect);

    /// <summary>
    /// 显示 "共享" UI 面板。
    /// </summary>
    void ShowShareUI();

    /// <summary>
    /// 选择全部 / 全选模组视图中的集合项。
    /// </summary>
    void SelectAll();

    /// <summary>
    /// 全部取消选择 / 清空模组视图中的选择项。
    /// </summary>
    void DeseleteAll();
}
