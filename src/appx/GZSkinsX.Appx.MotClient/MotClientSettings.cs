// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Settings;

namespace GZSkinsX.Appx.MotClient;

[Shared, Export(typeof(MotClientSettings))]
internal sealed class MotClientSettings
{
    private const string THE_GUID = "593A56BE-B064-4D49-A2BC-AE9E97058FA9";
    private const string WORKINGDIRECTORY_NAME = "WorkingDirectory";

    private readonly ISettingsSection _settingsSection;

    public string? WorkingDirectory
    {
        get => _settingsSection.Attribute<string>(WORKINGDIRECTORY_NAME);
        set => _settingsSection.Attribute(WORKINGDIRECTORY_NAME, value);
    }

    public MotClientSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID, SettingsType.Roaming);
    }
}
