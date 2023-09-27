// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Appx;

using GZSkinsX.Contracts.Settings;

namespace GZSkinsX.Appx.Kernel;

[Shared, Export]
internal sealed class KernelSettings
{
    private const string THE_GUID = "BED35426-5942-4EB1-B208-CF558912A0AC";
    private const string MODULECHECKSUM_NAME = "ModuleChecksum";

    private readonly ISettingsSection _settingsSection;

    public string? ModuleChecksum
    {
        get => _settingsSection.Attribute<string>(MODULECHECKSUM_NAME);
        set => _settingsSection.Attribute(MODULECHECKSUM_NAME, value);
    }

    public KernelSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID, SettingsType.Roaming);
    }
}
