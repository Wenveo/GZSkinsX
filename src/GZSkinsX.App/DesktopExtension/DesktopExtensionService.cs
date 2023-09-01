// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Contracts.DesktopExtension;
using GZSkinsX.Contracts.MyMods;

namespace GZSkinsX.DesktopExtension;

[Shared, Export(typeof(IDesktopExtensionService))]
internal sealed class DesktopExtensionService : IDesktopExtensionService
{
    public async Task<string> EncryptConfigTextAsync(string str)
    {
        return await App.DesktopExtensionMethods.EncryptConfigText(str);
    }

    public async Task<string> DecryptConfigTextAsync(string str)
    {
        return await App.DesktopExtensionMethods.DecryptConfigText(str);
    }

    public async Task<bool> EnsureEfficiencyModeAsync(int processId)
    {
        return await App.DesktopExtensionMethods.EnsureEfficiencyMode(processId);
    }

    public async Task SetEfficiencyModeAsync(int processId, bool isEnable)
    {
        await App.DesktopExtensionMethods.SetEfficiencyMode(processId, isEnable);
    }

    public async Task ExtractModImageAsync(string input, string output)
    {
        await App.DesktopExtensionMethods.ExtractModImage(input, output);
    }

    public async Task<MyModInfo> ReadModInfoAsync(string filePath)
    {
        var exModInfo = await App.DesktopExtensionMethods.ReadModInfo(filePath);
        return new(exModInfo.Name, exModInfo.Author, exModInfo.Description, exModInfo.DateTime);
    }

    public async Task<bool> ProcessLaunchAsync(string executable, string args, bool runAs)
    {
        return await App.DesktopExtensionMethods.ProcessLaunch(executable, args, runAs);
    }
}
