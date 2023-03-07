// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

using GZSkinsX.Api.Game;
using GZSkinsX.Api.Settings;

namespace GZSkinsX.Game;

[Shared, Export]
internal sealed class GameSettings
{
    private const string THE_GUID = "BFEEF60A-222B-422C-B459-83FC27E84290";
    private const string ROOT_DIRECTORY_NAME = "RootDirectory";
    private const string CURRENT_REGION_GUID = "CurrentRegion";

    private readonly ISettingsSection _settingsSection;

    private string _rootDirectory;
    private GameRegion _currentRegion;

    public string RootDirectory
    {
        get => _rootDirectory;
        set
        {
            if (!StringComparer.Ordinal.Equals(_rootDirectory, value))
            {
                _rootDirectory = value;
                _settingsSection.Attribute(ROOT_DIRECTORY_NAME, value);
            }
        }
    }

    public GameRegion CurrentRegion
    {
        get => _currentRegion;
        set
        {
            if (_currentRegion != value)
            {
                _currentRegion = value;
                _settingsSection.Attribute(CURRENT_REGION_GUID, value);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="settingsService"></param>
    [ImportingConstructor]
    public GameSettings(ISettingsService settingsService)
    {
        _settingsSection = settingsService.GetOrCreateSection(THE_GUID, SettingsType.Local);
        _rootDirectory = _settingsSection.Attribute<string>(ROOT_DIRECTORY_NAME) ?? string.Empty;
        _currentRegion = _settingsSection.Attribute<GameRegion>(CURRENT_REGION_GUID);
    }
}
