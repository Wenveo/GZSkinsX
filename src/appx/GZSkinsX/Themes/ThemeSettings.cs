// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Settings;

using Windows.UI.Xaml;

namespace GZSkinsX.Themes;

[Shared, Export]
internal sealed class ThemeSettings
{
    private const string THE_GUID = "29F0DBF1-D323-48E4-851A-A617AAAC708E";
    private const string CURRENT_THEME_NAME = "CurrentTheme";

    private readonly ISettingsSection _settingsSection;

    public ElementTheme CurrentTheme
    {
        get => _settingsSection.Attribute<ElementTheme>(CURRENT_THEME_NAME);
        set => _settingsSection.Attribute(CURRENT_THEME_NAME, value);
    }

    public ThemeSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
