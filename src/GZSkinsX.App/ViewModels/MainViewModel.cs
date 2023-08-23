// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.MyMods;
using GZSkinsX.MyMods;

using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    public IMyModsService MyModsService { get; }

    [ObservableProperty]
    private bool _enableWorkspace = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedIsNull))]
    [NotifyPropertyChangedFor(nameof(SelectedIsNotNull))]
    private MyModViewModel? _selectedMod;

    [ObservableProperty]
    private ObservableCollection<MyModViewModel> _myMods = new();

    public bool SelectedIsNull => SelectedMod is null;

    public bool SelectedIsNotNull => SelectedMod is not null;

    public MainViewModel()
    {
        MyModsService = AppxContext.MyModsService;
    }

    [RelayCommand]
    private async Task OnImportAsync()
    {
        var filePicker = new FileOpenPicker();
        filePicker.FileTypeFilter.Add(".lolgezi");

        var files = await filePicker.PickMultipleFilesAsync();
        if (files is null || files.Count == 0)
        {
            return;
        }

        await RefreshCoreAsync();
    }

    [RelayCommand]
    private async Task OnDeleteAsync(IList<object>? items)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        foreach (var file in items.OfType<MyModViewModel>().Select(a => a.ModFile))
        {
            await file.DeleteAsync();
        }

        await RefreshCoreAsync();
    }

    [RelayCommand]
    private async Task OnInstallAsync(IList<object>? items)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        await MyModsService.InstallModsAsync(
            items.OfType<MyModViewModel>().Select(a => a.ModFile));

        items.OfType<MyModViewModel>().Any(item => item.Enable = true);
    }

    [RelayCommand]
    private async Task OnUninstallAsync(IList<object>? items)
    {
        if (items is null || items.Count == 0)
        {
            return;
        }

        await MyModsService.UninstallModsAsync(
            items.OfType<MyModViewModel>().Select(a => a.ModFile));

        items.OfType<MyModViewModel>().Any(item => item.Enable = false);
    }

    [RelayCommand]
    private void OnDeselectAll(ListViewBase? listView)
    {
        if (listView is null)
        {
            return;
        }

        listView.DeselectAll();
    }

    [RelayCommand]
    private void OnSelectAll(ListViewBase? listView)
    {
        if (listView is null)
        {
            return;
        }

        listView.SelectAllSafe();
    }

    private async Task RefreshCoreAsync()
    {
        await MyModsService.RefreshAsync();

        var newList = new List<MyModViewModel>();
        var modsFolder = await MyModsService.GetModsFolderAsync();
        foreach (var file in await modsFolder.GetFilesAsync())
        {
            var modInfo = await MyModsService.ReadModInfoAsync(file);
            if (modInfo.IsEmpty)
            {
                continue;
            }

            var modImage = await MyModsService.GetModImageAsync(file);

            var isInstalled = MyModsService.IsInstalled(file);
            var indexOfTable = MyModsService.IndexOfTable(file);

            newList.Add(new(file, modImage, modInfo, isInstalled, indexOfTable));
        }

        MyMods = new ObservableCollection<MyModViewModel>(newList);
    }

    [RelayCommand]
    public async Task OnRefreshAsync()
    {
        EnableWorkspace = false;
        await RefreshCoreAsync();
        EnableWorkspace = true;
    }
}
