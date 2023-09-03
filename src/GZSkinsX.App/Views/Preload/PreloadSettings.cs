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
internal sealed class PreloadSettings
{
    private const string THE_GUID = "99092BD3-A425-403C-9EFA-A5C126B77F88";
    private const string DONTNEEDSHOWTHEAVAILABLEUPDATEDIALOG_NAME = "DontNeedShowTheAvailableUpdateDialog";

    private readonly ISettingsSection _settingsSection;

    public bool DontNeedShowTheAvailableUpdateDialog
    {
        get => _settingsSection.Attribute<bool>(DONTNEEDSHOWTHEAVAILABLEUPDATEDIALOG_NAME);
        set => _settingsSection.Attribute(DONTNEEDSHOWTHEAVAILABLEUPDATEDIALOG_NAME, value);
    }

    public PreloadSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
