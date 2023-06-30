// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Settings;

namespace GZSkinsX.Views.WindowFrames.Preload;

[Shared, Export]
internal sealed class StartUpSettings
{
    private const string THE_GUID = "09A5FCC5-4B0C-4476-8401-59EEBFB19213";
    private const string ISINITIALIZE_NAME = "IsInitialize";

    private readonly ISettingsSection _settingsSection;

    public bool IsInitialize
    {
        get => _settingsSection.Attribute<bool>(ISINITIALIZE_NAME);
        set => _settingsSection.Attribute(ISINITIALIZE_NAME, value);
    }

    public StartUpSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
