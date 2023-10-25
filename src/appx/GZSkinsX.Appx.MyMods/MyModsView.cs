// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.HighPerformance;
using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;

using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

namespace GZSkinsX.Appx.MyMods;

/// <summary>
/// 内部的对 <see cref="IMyModsView"/> 接口的默认实现。
/// </summary>
internal sealed class MyModsView : IMyModsView
{
    /// <summary>
    /// 存放模组项的字典集合，并使用 Djb2 Hash 作为其关联的键。
    /// </summary>
    private readonly Dictionary<int, MyModItemViewModel> _hashCodeToItemViewModel;

    /// <summary>
    /// 模组视图的 UI 对象实例。
    /// </summary>
    private readonly GridView _myModsGridView;

    /// <summary>
    /// 判断是否正在显示集合项的安装顺序。
    /// </summary>
    private bool _isShowInstalledIndex;

    /// <summary>
    /// 判断是否正在处于更新集合的操作中。
    /// </summary>
    private bool _collectionIsBusy;

    /// <summary>
    /// 定义集合项的筛选条件字符串。
    /// </summary>
    private string? _itemsFilter;

    /// <summary>
    /// 获取或设置是否显示集合项的安装顺序。
    /// </summary>
    public bool IsShowInstalledIndex
    {
        get => _isShowInstalledIndex;
        set
        {
            _isShowInstalledIndex = value;

            var dispatcherQueue = _myModsGridView.DispatcherQueue;
            if (dispatcherQueue.HasThreadAccess)
            {
                UpdateIsShowInstalledIndex();
            }
            else
            {
                dispatcherQueue.TryEnqueue(UpdateIsShowInstalledIndex);
            }

            void UpdateIsShowInstalledIndex()
            {
                foreach (var item in _hashCodeToItemViewModel.Values)
                {
                    if (item.IsInstalled)
                    {
                        item.IsShowIndex = value;
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public string? ItemsFilter
    {
        get => _itemsFilter;
        set
        {
            _itemsFilter = value;

            if (_collectionIsBusy is false)
            {
                UpdateItemsSource(value);
            }
        }
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<MyModItemViewModel> RawCollection => _hashCodeToItemViewModel.Values;

    /// <inheritdoc/>
    public IEnumerable<MyModItemViewModel> FilteredItems
    {
        get
        {
            // 因为 UI 中显示的元素等同于筛选后的集合，所以直接使用 ItemsSource。
            if (_myModsGridView.ItemsSource is IEnumerable enumerable)
            {
                return enumerable.OfType<MyModItemViewModel>();
            }

            return Array.Empty<MyModItemViewModel>();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<MyModItemViewModel> SelectedItems => _myModsGridView.SelectedItems.OfType<MyModItemViewModel>();

    /// <inheritdoc/>
    public MyModItemViewModel? SelectedItem => _myModsGridView.SelectedItem as MyModItemViewModel;

    /// <inheritdoc/>
    public event EventHandler<MyModsViewSelectionChangedArgs>? SelectionChanged;

    /// <summary>
    /// 当进入集合刷新的操作时所触发的事件。
    /// </summary>
    public event EventHandler? RefreshStarting;

    /// <summary>
    /// 当集合刷新完成后所触发的事件。
    /// </summary>
    public event EventHandler? RefreshCompleted;

    /// <summary>
    /// 初始化 <see cref="MyModsView"/> 的新实例。
    /// </summary>
    public MyModsView(GridView myModsGridView)
    {
        _hashCodeToItemViewModel = [];
        _myModsGridView = myModsGridView;
        _myModsGridView.SelectionChanged += OnSelectionChanged;
    }

    /// <summary>
    /// 触发选择事件的处理函数，用于包装传递 <see cref="IMyModsView.SelectionChanged"/> 事件。
    /// </summary>
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectionChanged?.Invoke(this, new()
        {
            AddedItems = e.AddedItems.OfType<MyModItemViewModel>(),
            RemovedItems = e.RemovedItems.OfType<MyModItemViewModel>()
        });
    }

    /// <summary>
    /// 更新 UI 视图中呈现的集合的源。
    /// </summary>
    /// <param name="filter">筛选条件字符串。</param>
    private void UpdateItemsSource(string? filter)
    {
        void UpdateItemsSourceCore()
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                // 创建新的数组容器，目的是为了让 UI 呈现新的集合以刷新列表。
                _myModsGridView.ItemsSource = _hashCodeToItemViewModel.Values.OrderBy(item => item.ModInfo.Name).ToImmutableArray();
            }
            else
            {
                _myModsGridView.ItemsSource = _hashCodeToItemViewModel.Values.Where(item =>
                {
                    var modInfo = item.ModInfo;
                    if (modInfo.FileInfo.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)) return true;
                    if (modInfo.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase)) return true;
                    if (modInfo.Author.Contains(filter, StringComparison.InvariantCultureIgnoreCase)) return true;
                    if (modInfo.Description.Contains(filter, StringComparison.InvariantCultureIgnoreCase)) return true;
                    if (modInfo.DateTime.Contains(filter, StringComparison.InvariantCultureIgnoreCase)) return true;

                    return false;
                }).OrderBy(item => item.ModInfo.Name).ToImmutableArray();
            }
        }

        var dispatcherQueue = _myModsGridView.DispatcherQueue;
        if (dispatcherQueue.HasThreadAccess)
        {
            UpdateItemsSourceCore();
        }
        else
        {
            dispatcherQueue.TryEnqueue(UpdateItemsSourceCore);
        }
    }

    /// <inheritdoc cref="IMyModsView.DeleteAsync(IEnumerable{MyModItemViewModel})"/>
    private async Task DeleteCoreAsync(IEnumerable<MyModItemViewModel> itemsToDelete)
    {
        if (itemsToDelete is null || itemsToDelete.Any() is false)
        {
            return;
        }

        foreach (var fileInfo in itemsToDelete.OfType<MyModItemViewModel>().Select(a => a.ModInfo.FileInfo))
        {
            try
            {
                fileInfo.Delete();

                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.Appx.MyMods.MyModView.DeleteCoreAsync",
                    $"The mod file \"{fileInfo.Name}\" have been deleted.");
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MyMods.MyModView.DeleteCoreAsync",
                    $"""
                    Failed to delete the mod file \"{fileInfo.Name}\".
                    {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}.
                    """);

                continue;
            }
        }

        await RefreshAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(IEnumerable<MyModItemViewModel> itemsToDelete)
    {
        await DeleteCoreAsync(itemsToDelete);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(params MyModItemViewModel[] itemsToDelete)
    {
        await DeleteCoreAsync(itemsToDelete);
    }

    /// <inheritdoc cref="IMyModsView.ImportAsync(IEnumerable{string})"/>
    private async Task ImportCoreAsync(IEnumerable<string> paths)
    {
        if (paths is null || paths.Any() is false)
        {
            return;
        }

        var myModsService = AppxContext.MyModsService;
        var modFolder = await myModsService.GetModFolderAsync();
        if (string.IsNullOrWhiteSpace(modFolder))
        {
            return;
        }

        if (Directory.Exists(modFolder) is false)
        {
            Directory.CreateDirectory(modFolder);
        }

        var verifiedFiles = new List<string>();
        foreach (var item in paths)
        {
            try
            {
                myModsService.ReadModInfo(item);
                verifiedFiles.Add(item);
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.Appx.MyMods.MyModView.ImportCoreAsync",
                    $"{excp}: {excp.Message}. {Environment.NewLine}{excp.StackTrace}.");

                var contentDialog = new ContentDialog()
                {
                    XamlRoot = _myModsGridView.XamlRoot,
                    Title = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_InvaliedFileDialog_Title"),
                    CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_InvaliedFileDialog_CloseButtonText"),
                    Content = excp.Message
                };

                await contentDialog.ShowAsync();
            }
        }

        if (verifiedFiles.Count is 0)
        {
            return;
        }

        var importedFilesCount = 0;
        foreach (var file in verifiedFiles)
        {
            var directory = Path.GetDirectoryName(file);
            if (directory is not null && directory == modFolder)
            {
                // 如果导入的文件是从模组文件夹中选择的 ?!
                continue;
            }

            var fileInfo = new FileInfo(file);
            var destFilename = StringComparer.OrdinalIgnoreCase.Equals(fileInfo.Extension, ".lolgezi")
                ? fileInfo.Name : Path.GetFileNameWithoutExtension(fileInfo.Name) + ".lolgezi";

            var destFilePath = Path.Combine(modFolder, destFilename);
            if (File.Exists(destFilePath) is false)
            {
                try
                {
                    fileInfo.CopyTo(destFilePath);
                    AppxContext.LoggingService.LogOkay(
                        "GZSkinsX.Appx.MyMods.MyModView.ImportCoreAsync",
                        $"The mod file \"{destFilename}\" have been imported.");
                }
                catch (Exception excp)
                {
                    AppxContext.LoggingService.LogError(
                        "GZSkinsX.Appx.MyMods.MyModView.ImportCoreAsync",
                        $"{excp}: {excp.Message}. {Environment.NewLine}{excp.StackTrace}.");

                    continue;
                }
            }
            else
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.Appx.MyMods.MyModView.ImportCoreAsync",
                    $"A mod file with the same name already exists \"{destFilename}\".");

                var contentDialog = new ContentDialog()
                {
                    XamlRoot = _myModsGridView.XamlRoot,
                    DefaultButton = ContentDialogButton.Primary,
                    Title = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_ReplaceOrSkipFileDialog_Title"),
                    PrimaryButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_ReplaceOrSkipFileDialog_PrimaryButtonText"),
                    CloseButtonText = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_ReplaceOrSkipFileDialog_CloseButtonText"),
                    Content = string.Format(ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_ReplaceOrSkipFileDialog_Content"), destFilename)
                };

                var result = await contentDialog.ShowAsync();
                if (result is not ContentDialogResult.Primary)
                {
                    AppxContext.LoggingService.LogWarning(
                        "GZSkinsX.Appx.MyMods.MyModView.ImportCoreAsync",
                        $"The operation to replace the existing mod file have been canceled.");

                    continue;
                }

                // 如果在显示对话框期间，源文件被移动或删除了则跳过后续操作（可能会发生？）
                if (File.Exists(fileInfo.FullName) is false)
                {
                    continue;
                }

                try
                {
                    fileInfo.CopyTo(destFilePath, true);
                    AppxContext.LoggingService.LogOkay(
                        "GZSkinsX.Appx.MyMods.MyModView.ImportCoreAsync",
                        $"The existing mod file have been successfully replaced.");
                }
                catch (Exception excp)
                {
                    AppxContext.LoggingService.LogError(
                        "GZSkinsX.Appx.MyMods.MyModView.ImportCoreAsync",
                        $"{excp}: {excp.Message}. {Environment.NewLine}{excp.StackTrace}.");

                    continue;
                }
            }

            importedFilesCount++;
        }

        if (importedFilesCount > 0)
        {
            await RefreshAsync();
        }
    }

    /// <inheritdoc/>
    public async Task ImportAsync(IEnumerable<StorageFile> files)
    {
        await ImportCoreAsync(files.Select(a => a.Path));
    }

    /// <inheritdoc/>
    public async Task ImportAsync(params StorageFile[] files)
    {
        await ImportCoreAsync(files.Select(a => a.Path));
    }

    /// <inheritdoc/>
    public async Task ImportAsync(IEnumerable<string> paths)
    {
        await ImportCoreAsync(paths);
    }

    /// <inheritdoc/>
    public async Task ImportAsync(params string[] paths)
    {
        await ImportCoreAsync(paths);
    }

    /// <inheritdoc cref="IMyModsView.InstallAsync(IEnumerable{MyModItemViewModel})"/>
    private async Task InstallCoreAsync(IEnumerable<MyModItemViewModel> itemsToInstall)
    {
        if (itemsToInstall is null || itemsToInstall.Any() is false)
        {
            return;
        }

        try
        {
            var myModViewModels = itemsToInstall.OfType<MyModItemViewModel>();
            await AppxContext.MyModsService.InstallModsAsync(myModViewModels.Select(a => a.ModInfo.FileInfo.FullName));

            var b = myModViewModels.All(item => item.IsInstalled = true);
            Debug.Assert(b);

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.Appx.MyMods.MyModView.InstallCoreAsync",
                $"One or more mods were successfully installed. Items: \"{string.Join(',', myModViewModels.Select(a => a.ModInfo.FileInfo.Name))}\".");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MyMods.MyModView.InstallCoreAsync",
                $"""
                Failed to install one or more mods.
                {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}
                """);
        }

        UpdateModInstalledIndex();
    }

    /// <inheritdoc/>
    public async Task InstallAsync(IEnumerable<MyModItemViewModel> itemsToInstall)
    {
        await InstallCoreAsync(itemsToInstall);
    }

    /// <inheritdoc/>
    public async Task InstallAsync(params MyModItemViewModel[] itemsToInstall)
    {
        await InstallCoreAsync(itemsToInstall);
    }

    /// <inheritdoc cref="IMyModsView.UninstallAsync(IEnumerable{MyModItemViewModel})"/>
    private async Task UninstallCoreAsync(IEnumerable<MyModItemViewModel> itemsToUninstall)
    {
        if (itemsToUninstall is null || itemsToUninstall.Any() is false)
        {
            return;
        }

        try
        {
            var myModViewModels = itemsToUninstall.OfType<MyModItemViewModel>();
            await AppxContext.MyModsService.UninstallModsAsync(myModViewModels.Select(a => a.ModInfo.FileInfo.Name));

            var b = myModViewModels.Any(item => item.IsInstalled = false);
            Debug.Assert(b is false);

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.Appx.MyMods.MyModView.UninstallCoreAsync",
                $"One or more mods were successfully uninstalled. Items: \"{string.Join(',', myModViewModels.Select(a => a.ModInfo.FileInfo.Name))}\".");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.Appx.MyMods.MyModView.UninstallCoreAsync",
                $"""
                Failed to uninstall one or more mods.
                {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}
                """);
        }

        UpdateModInstalledIndex();
    }

    /// <inheritdoc/>
    public async Task UninstallAsync(IEnumerable<MyModItemViewModel> itemsToUninstall)
    {
        await UninstallCoreAsync(itemsToUninstall);
    }

    /// <inheritdoc/>
    public async Task UninstallAsync(params MyModItemViewModel[] itemsToUninstall)
    {
        await UninstallCoreAsync(itemsToUninstall);
    }

    /// <summary>
    /// 更新所有模组项的安装顺序。
    /// </summary>
    private void UpdateModInstalledIndex()
    {
        var myModsService = AppxContext.MyModsService;
        foreach (var item in _hashCodeToItemViewModel.Values)
        {
            if (item.IsInstalled)
            {
                item.IndexOfTable = myModsService.IndexOfTable(item.ModInfo.FileInfo.Name) + 1;
                item.IsShowIndex = IsShowInstalledIndex;
            }
            else
            {
                item.IsShowIndex = false;
            }
        }
    }

    /// <inheritdoc/>
    public async Task RefreshAsync()
    {
        _collectionIsBusy = true;
        RefreshStarting?.Invoke(this, EventArgs.Empty);

        try
        {
            var dispatcherQueue = _myModsGridView.DispatcherQueue;
            var myModsService = AppxContext.MyModsService;
            await myModsService.RefreshAsync();

            var modFolder = await myModsService.GetModFolderAsync();
            if (Directory.Exists(modFolder))
            {
                // 移除不存在的项
                foreach (var item in _hashCodeToItemViewModel.Where(item =>
                {
                    var fileInfo = item.Value.ModInfo.FileInfo;
                    if (fileInfo.DirectoryName != modFolder)
                    {
                        // 当父文件夹不匹配时应当移除。
                        return true;
                    }
                    else
                    {
                        // 其次是判断目标文件夹中该文件是否不存在。
                        return File.Exists(fileInfo.FullName) is false;
                    }
                }).ToImmutableArray()) _hashCodeToItemViewModel.Remove(item.Key);

                // 枚举文件夹中的模组文件和更新集合
                foreach (var file in Directory.EnumerateFiles(modFolder))
                {
                    var djb2HashCode = file.GetDjb2HashCode();
                    var modInfo = myModsService.TryReadModInfo(file);
                    if (modInfo.HasValue is false)
                    {
                        // 如果无法读取数据信息，则该文件等同于无效，此时应当将其从集合中移除。
                        _hashCodeToItemViewModel.Remove(djb2HashCode);
                        continue;
                    }

                    var modImage = myModsService.GetModImage(file);
                    var indexOfTable = myModsService.IndexOfTable(file) + 1;
                    var isInstalled = myModsService.IsInstalled(file);

                    if (_hashCodeToItemViewModel.TryGetValue(djb2HashCode, out var modViewModel))
                    {
                        if (dispatcherQueue.TryEnqueue(() =>
                        {
                            modViewModel.ModImage = modImage;
                            modViewModel.ModInfo = modInfo.Value;
                            modViewModel.IsInstalled = isInstalled;
                            modViewModel.IndexOfTable = indexOfTable;
                        })) continue; // 如果更新成功就返回，否则失败的话，将会由下方代码进行一个替换操作。
                    }

                    // 添加或替换
                    _hashCodeToItemViewModel[djb2HashCode] = new(modInfo.Value, modImage, isInstalled, indexOfTable);
                }
            }

            await myModsService.UpdateSettingsAsync();

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.Appx.MyMods.MyModsView.RefreshAsync",
                $"The mods collection have been successfully refreshed.");

            UpdateItemsSource(ItemsFilter);
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.Appx.MyMods.MyModsView.RefreshAsync",
                $"""
                Unable to refresh the mods collection.
                {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}
                """);
        }

        _collectionIsBusy = false;
        RefreshCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public void DeseleteAll()
    {
        var dispatcherQueue = _myModsGridView.DispatcherQueue;
        if (dispatcherQueue.HasThreadAccess)
        {
            _myModsGridView.DeselectAll();
        }
        else
        {
            var b = dispatcherQueue.TryEnqueue(_myModsGridView.DeselectAll);
            if (!b)
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.Appx.MyMods.MyModsView.DeseleteAll",
                    $"Failed to enqueue the operation.");

                return;
            }
        }

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.Appx.MyMods.MyModsView.DeseleteAll",
            $"All items have been deselected.");
    }

    /// <inheritdoc/>
    public void SelectAll()
    {
        var dispatcherQueue = _myModsGridView.DispatcherQueue;
        if (dispatcherQueue.HasThreadAccess)
        {
            _myModsGridView.SelectAllSafe();
        }
        else
        {
            var b = dispatcherQueue.TryEnqueue(_myModsGridView.SelectAllSafe);
            if (!b)
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.Appx.MyMods.MyModsView.SelectAll",
                    $"Failed to enqueue the operation.");

                return;
            }
        }

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.Appx.MyMods.MyModsView.SelectAll",
            $"All items have been selected.");
    }

    /// <inheritdoc cref="SetSelectedItems(IEnumerable{MyModItemViewModel})"/>
    private void SetSelectedItemsCore(IEnumerable<MyModItemViewModel> itemsToSelect)
    {
        if (itemsToSelect is null || itemsToSelect.Any() is false)
        {
            return;
        }

        var itemsCollection = _myModsGridView.SelectedItems;
        itemsCollection.Clear();

        foreach (var item in itemsToSelect.Where(_hashCodeToItemViewModel.ContainsValue))
        {
            itemsCollection.Add(item);
        }
    }

    /// <inheritdoc/>
    public void SetSelectedItems(IEnumerable<MyModItemViewModel> itemsToSelect)
    {
        SetSelectedItemsCore(itemsToSelect);
    }

    /// <inheritdoc/>
    public void SetSelectedItems(params MyModItemViewModel[] itemsToSelect)
    {
        SetSelectedItemsCore(itemsToSelect);
    }

    /// <inheritdoc/>
    public void ShowShareUI()
    {
        if (SelectedItems.Any())
        {
            DataTransferManagerHelper.GetDataTransferManager().DataRequested += OnDataRequested;
            var interop = DataTransferManagerHelper.GetDataTransferManagerInterop();
            interop.ShowShareUIForWindow(AppxContext.AppxWindow.MainWindowHandle);
        }
    }

    /// <summary>
    /// 用于处理即将要共享的数据的处理函数。
    /// </summary>
    private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        var dataRequest = args.Request;
        var dataRequestDeferral = dataRequest.GetDeferral();

        var items = new List<StorageFile>();
        foreach (var item in SelectedItems.Select(a => a.ModInfo.FileInfo.FullName).Where(File.Exists))
        {
            items.Add(await StorageFile.GetFileFromPathAsync(item));
        }

        if (items.Count > 0)
        {
            if (items.Count == 1)
            {
                var format = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_Share_Dialog_Single_Item_Title");
                dataRequest.Data.Properties.Title = string.Format(format, items[0].Name);
            }
            else
            {
                var format = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_Share_Dialog_Multiple_Items_Title");
                dataRequest.Data.Properties.Title = string.Format(format, items[0].Name, items[1].Name);
            }

            dataRequest.Data.SetStorageItems(items, false);
        }
        else
        {
            dataRequest.FailWithDisplayText(ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_Share_Dialog_Failed_Text"));
        }

        dataRequestDeferral.Complete();
    }
}
