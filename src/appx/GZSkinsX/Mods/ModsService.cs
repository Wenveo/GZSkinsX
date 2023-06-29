// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using CommunityToolkit.HighPerformance.Buffers;

using GZSkinsX.Api.Mods;

using Windows.Storage;

namespace GZSkinsX.Mods;

[Shared, Export(typeof(IModsService))]
internal sealed class ModsService : IModsService
{
    public MemoryOwner<byte> ReadWGZModImage(StorageFile storageFile)
    {
        return WGZModHelper.ReadModImage(storageFile);
    }

    public WGZModInfo ReadWGZModInfo(StorageFile storageFile)
    {
        return WGZModHelper.ReadModInfo(storageFile);
    }
}