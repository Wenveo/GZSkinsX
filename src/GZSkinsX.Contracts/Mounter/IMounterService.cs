// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Threading.Tasks;

using Windows.Foundation;

namespace GZSkinsX.Contracts.Mounter;

public interface IMounterService
{
    event TypedEventHandler<IMounterService, bool>? IsRunningChanged;

    Task<bool> CheckUpdatesAsync();

    Task<bool> GetIsRunningAsync();

    Task<MTPackageMetadata> GetLocalMTPackageMetadataAsync(params string[] filter);

    Task LaunchAsync();

    Task LaunchAsync(string args);

    Task TerminateAsync();

    Task TryClearDownloadCacheAsync();

    Task UpdateAsync(IProgress<double>? progress = null);

    Task<bool> VerifyLocalMTPackageIntegrityAsync();
}
