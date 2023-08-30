// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Settings;

namespace GZSkinsX.Views;

[Shared, Export]
internal sealed class IndexSettings
{
    private const string THE_GUID = "F888DACE-FF71-4A33-8AB4-932BD7C8350D";
    private const string ISINITIALIZE_NAME = "IsInitialize";

    private readonly ISettingsSection _settingsSection;

    public bool IsInitialize
    {
        get => _settingsSection.Attribute<bool>(ISINITIALIZE_NAME);
        set => _settingsSection.Attribute(ISINITIALIZE_NAME, value);
    }

    public IndexSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
