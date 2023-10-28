// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Settings;

namespace GZSkinsX.Appx.MyMods.Views;

[Shared, Export]
internal sealed class ModsViewSettings
{
    private const string THE_GUID = "8DBDA9C8-8752-4D7A-8E7C-E2BE73F70BE3";
    private const string USELEGACYWIN10STYLECONTEXTMENU_NAME = "UseLegacyWin10StyleContextMenu";

    private readonly ISettingsSection _settingsSection;

    public bool UseLegacyWin10StyleContextMenu
    {
        get => _settingsSection.Attribute<bool>(USELEGACYWIN10STYLECONTEXTMENU_NAME);
        set => _settingsSection.Attribute(USELEGACYWIN10STYLECONTEXTMENU_NAME, value);
    }

    public ModsViewSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID, SettingsType.Local);
    }
}
