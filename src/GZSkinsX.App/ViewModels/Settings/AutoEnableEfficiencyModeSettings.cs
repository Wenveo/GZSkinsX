// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Settings;

namespace GZSkinsX.ViewModels;

[Shared, Export]
internal sealed class AutoEnableEfficiencyModeSettings
{
    private const string THE_GUID = "FF1E168C-21CB-4211-8422-03401AD881BA";
    private const string SHOULD_ENABLE_EFFICIENCY_MODE_NAME = "ShouldEnableEfficiencyMode";

    private readonly ISettingsSection _settingsSection;

    public bool ShouldEnableEfficiencyMode
    {
        get => _settingsSection.Attribute<bool>(SHOULD_ENABLE_EFFICIENCY_MODE_NAME);
        set => _settingsSection.Attribute(SHOULD_ENABLE_EFFICIENCY_MODE_NAME, value);
    }

    public AutoEnableEfficiencyModeSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
