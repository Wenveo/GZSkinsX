// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using GZSkinsX.Extensions.CreatorStudio.Wad.Parser;

namespace GZSkinsX.Extensions.CreatorStudio.Wad.ViewModels;

internal sealed partial class WadViewModel : ObservableObject
{
    private readonly WadFile _wadFile;
    private readonly WadFolderNodeVM _rootNodeVM;

    public WadFolderNodeVM RootNodeVM => _rootNodeVM;

    [ObservableProperty]
    private WadFolderNodeVM _currentViewNode;

    public WadViewModel(WadFile wadFile)
    {
        _wadFile = wadFile;
        _rootNodeVM = new WadFolderNodeVM(string.Empty, string.Empty);
        _currentViewNode = _rootNodeVM;

        Initialize();
    }

    private void Initialize()
    {
        var allPaths = _wadFile.FileChunks.Keys
            .Select(key => WadHashTable.GetNameOrDefault(key));

        foreach (var item in allPaths)
        {
            var parts = item.Split('/');
            var folderNodeVM = _rootNodeVM;

            for (var i = 0; i < parts.Length - 1; i++)
            {
                var childFolderNodeVM = folderNodeVM.GetChild<WadFolderNodeVM>(parts[i]);
                if (childFolderNodeVM is null)
                {
                    var folderFullName = i == 0 ? parts[0] : string.Join('/', parts.Take(i + 1));
                    childFolderNodeVM = new WadFolderNodeVM(parts[i], folderFullName);
                    folderNodeVM.AddChild(childFolderNodeVM);
                }

                folderNodeVM = childFolderNodeVM;
            }

            folderNodeVM.AddChild(new WadFileNodeVM(parts.Last(), item));
        }
    }
}
