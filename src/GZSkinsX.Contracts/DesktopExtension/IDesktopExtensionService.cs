// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

using GZSkinsX.Contracts.MyMods;

namespace GZSkinsX.Contracts.DesktopExtension;

public interface IDesktopExtensionService
{
    Task<string> EncryptConfigTextAsync(string str);

    Task<string> DecryptConfigTextAsync(string str);

    Task<bool> EnsureEfficiencyModeAsync(int processId);

    Task SetEfficiencyModeAsync(int processId, bool isEnable);

    Task ExtractModImageAsync(string input, string output);

    Task<MyModInfo> ReadModInfoAsync(string filePath);

    Task<bool> ProcessLaunchAsync(string executable, string args, bool runAs);
}
