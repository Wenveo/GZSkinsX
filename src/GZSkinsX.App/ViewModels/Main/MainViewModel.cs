// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
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

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    public IMyModsService MyModsService { get; }

    [ObservableProperty]
    private bool _isShowInstalledIndex;

    [ObservableProperty]
    private bool _enableWorkspace = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MyModCollection))]
    [NotifyPropertyChangedFor(nameof(ShouldShowMyModsCount))]
    [NotifyPropertyChangedFor(nameof(MyModsCount))]
    private string? _modsFilter;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedIsNull))]
    [NotifyPropertyChangedFor(nameof(SelectedIsNotNull))]
    private MyModViewModel? _selectedMod;

    private IEnumerable<MyModViewModel> _myModCollection = Array.Empty<MyModViewModel>();

    public IEnumerable<MyModViewModel> MyModCollection
    {
        get
        {
            var filter = ModsFilter;
            if (filter is null || filter == string.Empty)
            {
                foreach (var item in _myModCollection)
                {
                    yield return item;
                }

                GC.Collect();
                yield break;
            }

            foreach (var item in _myModCollection)
            {
                if (item.ModFile.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                    item.ModInfo.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                    item.ModInfo.Author.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
                    item.ModInfo.Description.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return item;
                }
            }
        }
        set
        {
            SetProperty(ref _myModCollection, value, nameof(MyModCollection));
            OnPropertyChanged(nameof(ShouldShowMyModsCount));
            OnPropertyChanged(nameof(MyModsCount));
        }
    }

    public bool SelectedIsNull => SelectedMod is null;

    public bool SelectedIsNotNull => SelectedMod is not null;

    public bool ShouldShowMyModsCount => !string.IsNullOrEmpty(ModsFilter) || MyModCollection.Any();

    public int MyModsCount => MyModCollection.Count();

    public MainViewModel()
    {
        MyModsService = AppxContext.MyModsService;
    }

    [RelayCommand]
    private async Task OnClearAllInstalledAsync()
    {
        await MyModsService.ClearAllInstalledAsync();
        await RefreshCoreAsync();
    }

    [RelayCommand]
    private async Task OnCopyAsync(IList<object>? items)
    {
        if (items.Any() is false)
        {
            return;
        }

        var dataPackage = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Copy
        };

        var storageItems = items
            .OfType<MyModViewModel>()
            .Select(a => a.ModFile);

        dataPackage.SetStorageItems(storageItems);
        Clipboard.SetContent(dataPackage);

        AppxContext.LoggingService.LogOkay(
            "GZSkinsX.App.ViewModels.MainViewModel.OnCopyAsync",
            $"Files \"{string.Join(',', storageItems.Select(a => a.Name))}\" have been copied to the clipboard.");

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task OnCopyAsPathAsync(IList<object>? items)
    {
        if (items.Any() is false)
        {
            return;
        }

        var dataPackage = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Copy
        };

        var files = items.OfType<MyModViewModel>().Select(a => a.ModFile).ToArray();
        if (files.Length == 1)
        {
            dataPackage.SetText($"\"{files[0].Path}\"");

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
                stringBuilder.Append(files[i].Path);
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
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task OnOpenInFileExplorerAsync(IList<object>? items)
    {
        if (items.Any() is false)
        {
            return;
        }

        var options = new FolderLauncherOptions();
        foreach (var item in items.OfType<MyModViewModel>().Select(a => a.ModFile))
        {
            options.ItemsToSelect.Add(item);
        }

        if (options.ItemsToSelect.Count > 0)
        {
            var modsFolder = await MyModsService.GetModsFolderAsync();
            var b = await Launcher.LaunchFolderAsync(modsFolder, options);

            if (b)
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
        if (await Launcher.LaunchFolderAsync(await MyModsService.GetModsFolderAsync()))
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
        if (await Launcher.LaunchFolderAsync(await MyModsService.GetWadsFolderAsync()))
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

        await ImportAsync(await filePicker.PickMultipleFilesAsync());
    }

    public async Task ImportAsync(IEnumerable<StorageFile> files)
    {
        if (files.Any() is false)
        {
            return;
        }

        var verifiedFiles = new List<StorageFile>();
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
        foreach (var file in verifiedFiles)
        {
            var parent = await file.GetParentAsync();
            if (parent is not null && parent.Path == modsFolder.Path)
            {
                continue;
            }

            if (await modsFolder.TryGetItemAsync(file.Name) is StorageFile existsFile)
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                    $"A mod file with the same name already exists \"{existsFile.Name}\".");

                var contentDialog = new ContentDialog()
                {
                    DefaultButton = ContentDialogButton.Primary,
                    Title = ResourceHelper.GetLocalized("Resources/Main_MyMods_ReplaceOrSkipFilesDialog_Title"),
                    PrimaryButtonText = ResourceHelper.GetLocalized("Resources/Main_MyMods_ReplaceOrSkipFilesDialog_Replace"),
                    CloseButtonText = ResourceHelper.GetLocalized("Resources/Main_MyMods_ReplaceOrSkipFilesDialog_Skip"),
                    Content = string.Format(ResourceHelper.GetLocalized("Resources/Main_MyMods_ReplaceOrSkipFilesDialog_Content"), existsFile.Name)
                };

                var result = await contentDialog.ShowAsync();
                if (result is not ContentDialogResult.Primary)
                {
                    AppxContext.LoggingService.LogWarning(
                        "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                        $"The operation to replace the existing mod file have been canceled.");

                    continue;
                }

                await file.CopyAndReplaceAsync(existsFile);

                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                    $"The existing mod file have been successfully replaced.");
            }
            else
            {
                var newFile = await file.CopyAsync(modsFolder);
                if (Path.GetExtension(newFile.Name) != ".lolgezi")
                {
                    var newFileName = Path.GetFileName(newFile.Name) + ".lolgezi";

                    // Fix extension name
                    await newFile.RenameAsync(newFileName);

                    AppxContext.LoggingService.LogOkay(
                        "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                        $"The mod file \"{newFile.Name}\" have been imported. And fix the file extension name");
                }
                else
                {
                    AppxContext.LoggingService.LogOkay(
                        "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                        $"The mod file \"{newFile.Name}\" have been imported.");
                }
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
        if (items.Any() is false)
        {
            return;
        }

        foreach (var file in items.OfType<MyModViewModel>().Select(a => a.ModFile))
        {
            try
            {
                await file.DeleteAsync();

                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.App.ViewModels.MainViewModel.OnDeleteAsync",
                    $"The mod file \"{file.Name}\" have been deleted.");
            }
            catch (Exception excp)
            {
                AppxContext.LoggingService.LogError(
                    "GZSkinsX.App.ViewModels.MainViewModel.ImportAsync",
                    $"""
                    Failed to delete the mod file \"{file.Name}\".
                    {excp}: "{excp.Message}". {Environment.NewLine}{excp.StackTrace}.
                    """);

                continue;
            }
        }

        await RefreshCoreAsync();
    }

    [RelayCommand]
    private async Task OnInstallAsync(IList<object>? items)
    {
        if (items.Any() is false)
        {
            return;
        }

        try
        {
            var myModViewModels = items.OfType<MyModViewModel>();
            await MyModsService.InstallModsAsync(myModViewModels.Select(a => a.ModFile));
            myModViewModels.All(item => item.Enable = true);

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.OnInstallAsync",
                $"One or more mods were successfully installed. Items: \"{string.Join(',', myModViewModels.Select(a => a.ModFile.Name))}\".");
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

    [RelayCommand]
    private async Task OnUninstallAsync(IList<object>? items)
    {
        if (items.Any() is false)
        {
            return;
        }

        try
        {
            var myModViewModels = items.OfType<MyModViewModel>();
            await MyModsService.UninstallModsAsync(myModViewModels.Select(a => a.ModFile));
            myModViewModels.Any(item => item.Enable = false);

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.OnUninstallAsync",
                $"One or more mods were successfully uninstalled. Items: \"{string.Join(',', myModViewModels.Select(a => a.ModFile.Name))}\".");
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
    private void OnDeselectAll(ListViewBase? listView)
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
    private void OnSelectAll(ListViewBase? listView)
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
        foreach (var item in MyModCollection)
        {
            if (item.Enable)
            {
                item.IndexOfTable = MyModsService.IndexOfTable(item.ModFile) + 1;
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
        foreach (var item in MyModCollection)
        {
            if (item.Enable)
            {
                item.IsShowIndex = value;
            }
        }
    }

    private async Task RefreshCoreAsync()
    {
        try
        {
            await MyModsService.RefreshAsync();

            var newList = new List<MyModViewModel>();
            var modsFolder = await MyModsService.GetModsFolderAsync();
            foreach (var file in await modsFolder.GetFilesAsync())
            {
                var modInfo = await MyModsService.TryReadModInfoAsync(file);
                if (modInfo is not null)
                {
                    var modImage = await MyModsService.GetModImageAsync(file);

                    var isInstalled = MyModsService.IsInstalled(file);
                    var indexOfTable = MyModsService.IndexOfTable(file) + 1;

                    newList.Add(new(file, modImage, modInfo, isInstalled, indexOfTable));
                }
            }

            await MyModsService.UpdateSettingsAsync();

            MyModCollection = newList;

            AppxContext.LoggingService.LogOkay(
                "GZSkinsX.App.ViewModels.MainViewModel.RefreshCoreAsync",
                $"The mods collection have been successfully refreshed.");
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
        await RefreshCoreAsync();
        EnableWorkspace = true;
    }

    [RelayCommand]
    private async Task OnSwitchTheme(ElementTheme newTheme)
    {
        var themeService = AppxContext.ThemeService;
        if (themeService.CurrentTheme != newTheme)
        {
            await themeService.SetElementThemeAsync(newTheme);
        }
    }
}
