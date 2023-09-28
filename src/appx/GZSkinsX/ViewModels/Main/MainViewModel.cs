// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace GZSkinsX.ViewModels;

/// <inheritdoc/>
internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isShowInstalledIndex;

    [ObservableProperty]
    private bool _enableWorkspace = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MyModsCollection))]
    [NotifyPropertyChangedFor(nameof(MyModsCount))]
    private string? _modsFilter;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedIsNull))]
    [NotifyPropertyChangedFor(nameof(SelectedIsNotNull))]
    private MyModViewModel? _selectedMod;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MyModsCount))]
    private IEnumerable<MyModViewModel>? _myModsCollection;

    private readonly Dictionary<string, MyModViewModel> _pathToModViewModel = [];

    public bool SelectedIsNull => SelectedMod is null;

    public bool SelectedIsNotNull => SelectedMod is not null;

    public int MyModsCount => MyModsCollection?.Count() ?? 0;

    public IMyModsService MyModsService { get; }

    public Microsoft.UI.Dispatching.DispatcherQueue DispatcherQueue { get; }

    public MainViewModel()
    {
        MyModsService = AppxContext.MyModsService;
        DispatcherQueue = AppxContext.AppxWindow.MainWindow.DispatcherQueue;
    }

    partial void OnModsFilterChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            MyModsCollection = _pathToModViewModel.Values.OrderBy(a => a.ModInfo.FileInfo.Name);
            // 当筛选内容为空时，手动进行一次垃圾回收
            // 避免筛选过程中产生的过多的内存占用。
            GC.Collect();
        }
        else
        {
            MyModsCollection = _pathToModViewModel.Values.Where(item =>
            {
                if (item.ModInfo.FileInfo.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)) return true;
                if (item.ModInfo.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)) return true;
                if (item.ModInfo.Author.Contains(value, StringComparison.InvariantCultureIgnoreCase)) return true;
                if (item.ModInfo.Description.Contains(value, StringComparison.InvariantCultureIgnoreCase)) return true;
                if (item.ModInfo.DateTime.Contains(value, StringComparison.InvariantCultureIgnoreCase)) return true;

                return false;
            });
        }
    }

    [RelayCommand]
    private async Task OnClearAllInstalledAsync()
    {
        await MyModsService.ClearAllInstalledAsync();
        await RefreshCoreAsync();
    }

    [RelayCommand]
    private static async Task OnCopyAsync(IList<object>? items)
    {
        if (items is null || items.Any() is false)
        {
            return;
        }

        var dataPackage = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Copy
        };

        var storageItems = new List<StorageFile>();
        foreach (var item in items)
        {
            if (item is MyModViewModel modViewModel && File.Exists(modViewModel.ModInfo.FileInfo.FullName))
            {
                storageItems.Add(await StorageFile.GetFileFromPathAsync(modViewModel.ModInfo.FileInfo.FullName));
            }
        }

        dataPackage.SetStorageItems(storageItems);
        Clipboard.SetContent(dataPackage);

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.App.ViewModels.MainViewModel.OnCopyAsync",
            $"Files \"{string.Join(',', storageItems.Select(a => a.Name))}\" have been copied to the clipboard.");

        await Task.CompletedTask;
    }

    [RelayCommand]
    private static void OnCopyAsPath(IList<object>? items)
    {
        if (items is null || items.Any() is false)
        {
            return;
        }

        var dataPackage = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Copy
        };

        var files = items.OfType<MyModViewModel>().Select(a => a.ModInfo.FileInfo).ToArray();
        if (files.Length == 1)
        {
            dataPackage.SetText($"\"{files[0].FullName}\"");

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.OnCopyAsPathAsync",
                $"The path of file \"{files[0].Name}\" have been copied to the clipboard.");
        }
        else
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < files.Length;)
            {
                stringBuilder.Append('\"');
                stringBuilder.Append(files[i].FullName);
                stringBuilder.Append('\"');

                if (++i != files.Length)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
            }

            dataPackage.SetText(stringBuilder.ToString());

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.OnCopyAsPathAsync",
                $"The paths of files \"{string.Join(',', files.Select(a => a.Name))}\" have been copied to the clipboard.");
        }

        Clipboard.SetContent(dataPackage);
    }

    [RelayCommand]
    private async Task OnOpenInFileExplorerAsync(IList<object>? items)
    {
        if (items is null || items.Any() is false)
        {
            return;
        }

        var modsFolder = await MyModsService.GetModsFolderAsync();
        if (string.IsNullOrEmpty(modsFolder) || string.IsNullOrWhiteSpace(modsFolder))
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenInFileExplorerAsync",
                $"Warning: The mods folder is null!");

            return;
        }

        if (Directory.Exists(modsFolder) is false)
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenInFileExplorerAsync",
                $"Warning: The mods folder is not exists!");

            return;
        }

        var options = new FolderLauncherOptions();
        foreach (var item in items)
        {
            if (item is MyModViewModel modViewModel && File.Exists(modViewModel.ModInfo.FileInfo.FullName))
            {
                options.ItemsToSelect.Add(await StorageFile.GetFileFromPathAsync(modViewModel.ModInfo.FileInfo.FullName));
            }
        }

        if (options.ItemsToSelect.Count > 0)
        {
            if (await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(modsFolder), options))
            {
                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.App.ViewModels.MainViewModel.OnOpenInFileExplorerAsync",
                    $"Successfully open the mods folder, ItemToSelect: \"{string.Join(',', options.ItemsToSelect.Select(a => a.Name))}\".");
            }
            else
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.App.ViewModels.MainViewModel.OnOpenInFileExplorerAsync",
                    $"Failed to open the mods folder.");
            }
        }
        else
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenInFileExplorerAsync",
                $"No items to select, will not open the mods folder.");
        }
    }

    [RelayCommand]
    private async Task OnOpenModFolderAsync()
    {
        var modsFolder = await MyModsService.GetModsFolderAsync();
        if (string.IsNullOrEmpty(modsFolder) || string.IsNullOrWhiteSpace(modsFolder))
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenModFolderAsync",
                $"Warning: The mods folder is null!");

            return;
        }

        if (Directory.Exists(modsFolder) is false)
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenModFolderAsync",
                $"Warning: The mods folder is not exists!");

            return;
        }

        if (await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(modsFolder)))
        {
            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenModFolderAsync",
                $"Successfully open the mods folder.");
        }
        else
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenModFolderAsync",
                $"Failed to open the mods folder.");
        }
    }

    [RelayCommand]
    private async Task OnOpenWadFolderAsync()
    {
        var wadsFolder = await MyModsService.GetWadsFolderAsync();
        if (string.IsNullOrEmpty(wadsFolder) || string.IsNullOrWhiteSpace(wadsFolder))
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenWadFolderAsync",
                $"Warning: The wads folder is null!");

            return;
        }

        if (Directory.Exists(wadsFolder) is false)
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenWadFolderAsync",
                $"Warning: The wads folder is not exists!");

            return;
        }

        if (await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(wadsFolder)))
        {
            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenWadFolderAsync",
                $"Successfully open the wads folder.");
        }
        else
        {
            AppxContext.LoggingService.LogWarning(
                "GZSkinsX.App.ViewModels.MainViewModel.OnOpenWadFolderAsync",
                $"Failed to open the wads folder.");
        }
    }

    [RelayCommand]
    private async Task OnImportAsync()
    {
        var filePicker = new FileOpenPicker();
        filePicker.FileTypeFilter.Add(".lolgezi");

        WinRT.Interop.InitializeWithWindow.Initialize(
            filePicker, AppxContext.AppxWindow.MainWindowHandle);

        await ImportAsync(await filePicker.PickMultipleFilesAsync());
    }

    public async Task ImportAsync(IEnumerable<StorageFile> files)
    {
        await ImportAsync(files.Select(a => a.Path));
    }

    public async Task ImportAsync(IEnumerable<string> files)
    {
        if (files.Any() is false)
        {
            return;
        }

        var verifiedFiles = new List<string>();
        foreach (var item in files)
        {
            try
            {
                await MyModsService.ReadModInfoAsync(item);
                verifiedFiles.Add(item);
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                    $"{excp}: {excp.Message}. {Environment.NewLine}{excp.StackTrace}.");

                var contentDialog = new ContentDialog()
                {
                    XamlRoot = AppxContext.AppxWindow.MainWindow.Content.XamlRoot,
                    Title = ResourceHelper.GetLocalized("Resources/Main_MyMods_ImportFilesDialog_InvaliedFile"),
                    CloseButtonText = ResourceHelper.GetLocalized("Resources/Main_MyMods_ImportFilesDialog_CloseButtonText"),
                    Content = excp.Message
                };

                await contentDialog.ShowAsync();
            }
        }

        if (verifiedFiles.Count is 0)
        {
            return;
        }

        var importCount = 0;
        var modsFolder = await MyModsService.GetModsFolderAsync();
        if (string.IsNullOrEmpty(modsFolder) || string.IsNullOrWhiteSpace(modsFolder))
        {
            return;
        }

        if (Directory.Exists(modsFolder) is false)
        {
            Directory.CreateDirectory(modsFolder);
        }

        foreach (var file in verifiedFiles)
        {
            var parent = Path.GetDirectoryName(file);
            if (parent is not null && parent == modsFolder)
            {
                continue;
            }

            var fileInfo = new FileInfo(file);
            var destFilename = StringComparer.OrdinalIgnoreCase.Equals(fileInfo.Extension, ".lolgezi")
                ? fileInfo.Name : Path.GetFileNameWithoutExtension(fileInfo.Name) + ".lolgezi";

            var destFilePath = Path.Combine(modsFolder, destFilename);
            if (File.Exists(destFilePath) is false)
            {
                fileInfo.CopyTo(destFilePath);
                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                    $"The mod file \"{destFilename}\" have been imported.");
            }
            else
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                    $"A mod file with the same name already exists \"{destFilename}\".");

                var contentDialog = new ContentDialog()
                {
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = AppxContext.AppxWindow.MainWindow.Content.XamlRoot,
                    Title = ResourceHelper.GetLocalized("Resources/Main_MyMods_ReplaceOrSkipFilesDialog_Title"),
                    PrimaryButtonText = ResourceHelper.GetLocalized("Resources/Main_MyMods_ReplaceOrSkipFilesDialog_Replace"),
                    CloseButtonText = ResourceHelper.GetLocalized("Resources/Main_MyMods_ReplaceOrSkipFilesDialog_Skip"),
                    Content = string.Format(ResourceHelper.GetLocalized("Resources/Main_MyMods_ReplaceOrSkipFilesDialog_Content"), destFilename)
                };

                var result = await contentDialog.ShowAsync();
                if (result is not ContentDialogResult.Primary)
                {
                    AppxContext.LoggingService.LogWarning(
                        "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                        $"The operation to replace the existing mod file have been canceled.");

                    continue;
                }

                fileInfo.CopyTo(destFilePath, true);
                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                    $"The existing mod file have been successfully replaced.");
            }

            importCount++;
        }

        if (importCount > 0)
        {
            await RefreshCoreAsync();
        }
    }

    [RelayCommand]
    private async Task OnDeleteAsync(IList<object>? items)
    {
        if (items is null || items.Any() is false)
        {
            return;
        }

        foreach (var fileInfo in items.OfType<MyModViewModel>().Select(a => a.ModInfo.FileInfo))
        {
            try
            {
                fileInfo.Delete();

                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.App.ViewModels.MainViewModel.OnDeleteAsync",
                    $"The mod file \"{fileInfo.Name}\" have been deleted.");
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                    $"""
                    Failed to delete the mod file \"{fileInfo.Name}\".
                    {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}.
                    """);

                continue;
            }
        }

        await RefreshCoreAsync();
    }

    public async Task OnInstallAsync(params object[] mods)
    {
        await OnInstallAsync(items: mods);
    }

    [RelayCommand]
    private async Task OnInstallAsync(IList<object>? list)
    {
        await OnInstallAsync(items: list);
    }

    private async Task OnInstallAsync(IEnumerable<object>? items)
    {
        if (items is null || items.Any() is false)
        {
            return;
        }

        try
        {
            var myModViewModels = items.OfType<MyModViewModel>();
            await MyModsService.InstallModsAsync(myModViewModels.Select(a => a.ModInfo.FileInfo.FullName));

            var b = myModViewModels.All(item => item.IsInstalled = true);
            Debug.Assert(b);

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.OnInstallAsync",
                $"One or more mods were successfully installed. Items: \"{string.Join(',', myModViewModels.Select(a => a.ModInfo.FileInfo.Name))}\".");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.App.ViewModels.MainViewModel.OnInstallAsync",
                $"""
                Failed to install one or more mods.
                {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}
                """);
        }

        UpdateModInstalledIndex();
    }

    public async Task OnUninstallAsync(params object[] mods)
    {
        await OnUninstallAsync(items: mods);
    }

    [RelayCommand]
    private async Task OnUninstallAsync(IList<object>? list)
    {
        await OnUninstallAsync(items: list);
    }

    private async Task OnUninstallAsync(IEnumerable<object>? items)
    {
        if (items is null || items.Any() is false)
        {
            return;
        }

        try
        {
            var myModViewModels = items.OfType<MyModViewModel>();
            await MyModsService.UninstallModsAsync(myModViewModels.Select(a => a.ModInfo.FileInfo.Name));

            var b = myModViewModels.Any(item => item.IsInstalled = false);
            Debug.Assert(b is false);

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.OnUninstallAsync",
                $"One or more mods were successfully uninstalled. Items: \"{string.Join(',', myModViewModels.Select(a => a.ModInfo.FileInfo.Name))}\".");
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogError(
                "GZSkinsX.App.ViewModels.MainViewModel.OnUninstallAsync",
                $"""
                Failed to uninstall one or more mods.
                {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}
                """);
        }

        UpdateModInstalledIndex();
    }

    [RelayCommand]
    private static void OnDeselectAll(ListViewBase? listView)
    {
        if (listView is null)
        {
            return;
        }

        listView.DeselectAll();

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.App.ViewModels.MainViewModel.OnDeselectAll",
            $"All items have been deselected.");
    }

    [RelayCommand]
    private static void OnSelectAll(ListViewBase? listView)
    {
        if (listView is null)
        {
            return;
        }

        listView.SelectAllSafe();

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.App.ViewModels.MainViewModel.OnSelectAll",
            $"All items have been selected.");
    }

    private void UpdateModInstalledIndex()
    {
        foreach (var item in _pathToModViewModel.Values)
        {
            if (item.IsInstalled)
            {
                item.IndexOfTable = MyModsService.IndexOfTable(item.ModInfo.FileInfo.Name) + 1;
                item.IsShowIndex = IsShowInstalledIndex;
            }
            else
            {
                item.IsShowIndex = false;
            }
        }
    }

    partial void OnIsShowInstalledIndexChanged(bool value)
    {
        foreach (var item in _pathToModViewModel.Values)
        {
            if (item.IsInstalled)
            {
                item.IsShowIndex = value;
            }
        }
    }

    private async Task RefreshCoreAsync()
    {
        try
        {
            var myModsService = MyModsService;
            await myModsService.RefreshAsync();

            // 移除不存在的项
            foreach (var item in _pathToModViewModel.Where(item =>
            {
                return File.Exists(item.Value.ModInfo.FileInfo.FullName) is false;
            }).ToArray())
            {
                _pathToModViewModel.Remove(item.Key);
            }

            var modsFolder = await myModsService.GetModsFolderAsync();
            if (Directory.Exists(modsFolder))
            {
                foreach (var file in Directory.EnumerateFiles(modsFolder))
                {
                    var modInfo = await myModsService.TryReadModInfoAsync(file);
                    if (MyModInfo.IsEmpty(modInfo))
                    {
                        continue;
                    }

                    var modImage = await myModsService.GetModImageAsync(file);
                    var indexOfTable = myModsService.IndexOfTable(file) + 1;
                    var isInstalled = myModsService.IsInstalled(file);

                    if (_pathToModViewModel.TryGetValue(file, out var modViewModel))
                    {
                        await DispatcherQueue.EnqueueAsync(() =>
                        {
                            modViewModel.ModImage = modImage;
                            modViewModel.ModInfo = modInfo;
                            modViewModel.IsInstalled = isInstalled;
                            modViewModel.IndexOfTable = indexOfTable;
                        });
                    }
                    else
                    {
                        _pathToModViewModel.Add(file, new(modInfo, modImage, isInstalled, indexOfTable));
                    }
                }
            }

            await myModsService.UpdateSettingsAsync();

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.RefreshCoreAsync",
                $"The mods collection have been successfully refreshed.");

            await DispatcherQueue.EnqueueAsync(() =>
            {
                MyModsCollection = _pathToModViewModel.Values.OrderBy(a => a.ModInfo.FileInfo.Name);
            });
        }
        catch (Exception excp)
        {
            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.RefreshCoreAsync",
                $"""
                Unable to refresh the mods collection.
                {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}
                """);
        }
    }

    [RelayCommand]
    public async Task OnRefreshAsync()
    {
        EnableWorkspace = false;
        await Task.Run(RefreshCoreAsync);
        EnableWorkspace = true;
    }

    [RelayCommand]
    private static async Task OnSwitchTheme(ElementTheme newTheme)
    {
        var themeService = AppxContext.ThemeService;
        if (themeService.CurrentTheme != newTheme)
        {
            await themeService.SetElementThemeAsync(newTheme);
        }
    }
}
