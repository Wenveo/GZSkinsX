// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.UI.Xaml.Media.Imaging;

using Windows.Storage;

namespace GZSkinsX.Contracts.MyMods;

public interface IMyModsService
{
    Task ClearAllInstalledAsync();

    bool IsInstalled(StorageFile storageFile);

    int IndexOfTable(StorageFile storageFile);

    Task ImportModsAsync(params StorageFile[] storageFiles);

    Task ImportModsAsync(IEnumerable<StorageFile> storageFiles);

    Task InstallModsAsync(params StorageFile[] storageFiles);

    Task InstallModsAsync(IEnumerable<StorageFile> storageFiles);

    Task UninstallModsAsync(params StorageFile[] storageFiles);

    Task UninstallModsAsync(IEnumerable<StorageFile> storageFiles);

    Task<bool> GetIsEnableBloodAsync();

    Task<StorageFolder> GetModsFolderAsync();

    Task<StorageFolder> GetWadsFolderAsync();

    Task SetIsEnableBloodAsync(bool isEnable);

    Task SetModsFolderAsync(StorageFolder storageFolder);

    Task SetWadsFolderAsync(StorageFolder storageFolder);

    Task<BitmapImage> GetModImageAsync(StorageFile storageFile);

    Task<MyModInfo> ReadModInfoAsync(StorageFile storageFile);

    Task<MyModInfo?> TryReadModInfoAsync(StorageFile storageFile);

    Task RefreshAsync();

    Task UpdateSettingsAsync();
}
