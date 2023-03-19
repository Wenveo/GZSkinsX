// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

using Windows.Storage;

namespace GZSkinsX.Api.MRT;

/// <summary>
/// 
/// </summary>
public interface IResourceCoreService
{
    /// <summary>
    /// 
    /// </summary>
    IResourceCoreMap MainResourceMap { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    void LoadPriFiles(IEnumerable<IStorageFile> files);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="files"></param>
    void UnloadPriFiles(IEnumerable<IStorageFile> files);
}
