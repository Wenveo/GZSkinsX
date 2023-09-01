// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Settings;

namespace GZSkinsX.Services.MyMods;

[Shared, Export(typeof(MyModsSettings))]
internal sealed class MyModsSettings
{
    private const string THE_GUID = "025E13EF-B1F8-425F-8D52-F713F8AFC234";
    private const string ENABLE_BLOOD_NAME = "EnableBlood";

    private readonly ISettingsSection _settingsSection;

    public bool EnableBlood
    {
        get => _settingsSection.Attribute<bool>(ENABLE_BLOOD_NAME);
        set => _settingsSection.Attribute(ENABLE_BLOOD_NAME, value);
    }

    public MyModsSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
