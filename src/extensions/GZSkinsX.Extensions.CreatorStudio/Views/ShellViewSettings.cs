// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.SDK.Appx;
using GZSkinsX.SDK.Settings;

namespace GZSkinsX.Extensions.CreatorStudio.Views;

[Shared, Export]
internal sealed class ShellViewSettings
{
    private const string THE_GUID = "DA78116A-F719-44B3-BE73-1A1F66C6E700";
    private const string LEFTCOLUMNWIDTH_NAME = "LeftColumnWidth";
    private const string ISHIDEASSETSEXPLORER_NAME = "IsHideAssetsExplorer";

    private readonly ISettingsSection _settingsSection;

    public double LeftColumnWidth
    {
        get => _settingsSection.Attribute<double>(LEFTCOLUMNWIDTH_NAME);
        set => _settingsSection.Attribute(LEFTCOLUMNWIDTH_NAME, value);
    }

    public bool IsHideAssetsExplorer
    {
        get => _settingsSection.Attribute<bool>(ISHIDEASSETSEXPLORER_NAME);
        set => _settingsSection.Attribute(ISHIDEASSETSEXPLORER_NAME, value);
    }

    public ShellViewSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
