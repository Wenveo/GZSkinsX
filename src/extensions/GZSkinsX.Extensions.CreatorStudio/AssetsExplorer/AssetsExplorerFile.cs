// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.IO;

using GZSkinsX.SDK.CreatorStudio.AssetsExplorer;

namespace GZSkinsX.Extensions.CreatorStudio.AssetsExplorer;

internal sealed class AssetsExplorerFile : AssetsExplorerItem, IAssetsExplorerFile
{
    public FileInfo FileInfo { get; }

    public AssetsExplorerFile(FileInfo fileInfo) : base(fileInfo.Name, "\uE130")
    {
        FileInfo = fileInfo;
    }
}
