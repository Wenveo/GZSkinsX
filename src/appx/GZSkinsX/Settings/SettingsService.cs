// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.Api.Settings;

using Windows.Storage;

namespace GZSkinsX.Settings;

[Shared, Export(typeof(ISettingsService))]
internal sealed class SettingsService : ISettingsService
{
    private readonly SettingsSection _localSettingsSection;

    private readonly SettingsSection _roamingSettingsSection;

    public SettingsService()
    {
        var current = ApplicationData.Current;
        _localSettingsSection = new(current.LocalSettings);
        _roamingSettingsSection = new(current.RoamingSettings);
    }

    public ISettingsSection GetOrCreateSection(string name)
    => _localSettingsSection.GetOrCreateSection(name);

    public ISettingsSection GetOrCreateSection(string name, SettingsType type)
    {
        if (type == SettingsType.Roaming)
            return _roamingSettingsSection.GetOrCreateSection(name);
        else
            return _localSettingsSection.GetOrCreateSection(name);
    }

    public void DeleteSection(string name)
    => _localSettingsSection.GetOrCreateSection(name);

    public void DeleteSection(string name, SettingsType type)
    {
        if (type == SettingsType.Roaming)
            _roamingSettingsSection.DeleteSection(name);
        else
            _localSettingsSection.GetOrCreateSection(name);
    }
}
