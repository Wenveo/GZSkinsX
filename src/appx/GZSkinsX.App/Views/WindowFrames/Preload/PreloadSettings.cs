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
internal sealed class PreloadSettings
{
    private const string THE_GUID = "F4523D71-8866-4241-9F5B-D56D850BD878";
    private const string ISINITIALIZE_NAME = "IsInitialize";

    private readonly ISettingsSection _settingsSection;

    public bool IsInitialize
    {
        get => _settingsSection.Attribute<bool>(ISINITIALIZE_NAME);
        set => _settingsSection.Attribute(ISINITIALIZE_NAME, value);
    }

    public PreloadSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
