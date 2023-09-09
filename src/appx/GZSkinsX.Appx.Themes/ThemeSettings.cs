// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Appx.Contracts.App;
using GZSkinsX.Appx.Contracts.Settings;

using Microsoft.UI.Xaml;

namespace GZSkinsX.Services.Themes;

[Shared, Export]
internal sealed class ThemeSettings
{
    private const string THE_GUID = "215B274D-087C-4F5E-83A4-AE7429CE90E6";
    private const string CURRENT_THEME_NAME = "CurrentTheme";

    private readonly ISettingsSection _settingsSection =
        AppxContext.SettingsService.GetOrCreateSection(THE_GUID);

    public ElementTheme CurrentTheme
    {
        get => _settingsSection.Attribute<ElementTheme>(CURRENT_THEME_NAME);
        set => _settingsSection.Attribute(CURRENT_THEME_NAME, value);
    }
}
