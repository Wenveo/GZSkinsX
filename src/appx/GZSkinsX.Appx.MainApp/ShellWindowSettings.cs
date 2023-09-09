// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Appx.Contracts.App;
using GZSkinsX.Appx.Contracts.Settings;

namespace GZSkinsX.Appx.MainApp;

[Shared, Export]
internal sealed class ShellWindowSettings
{
#pragma warning disable format
    private const string THE_GUID                       = "452C1993-9AB7-4925-BB68-42B8590F1D72";
    private const string NEEDTORESTOREWINDOWSTATE_NAME  = "NeedToRestoreWindowState";
    private const string ISMAXIMIZED_NAME               = "IsMaximized";
    private const string WINDOWHEIGHT_NAME              = "WindowHeight";
    private const string WINDOWWIDTH_NAME               = "WindowWidth";
    private const string WINDOWLEFT_NAME                = "WindowLeft";
    private const string WINDOWTOP_NAME                 = "WindowTop";
#pragma warning restore format

    private readonly ISettingsSection _settingsSection;

    public bool NeedToRestoreWindowState
    {
        get => _settingsSection.Attribute<bool>(NEEDTORESTOREWINDOWSTATE_NAME);
        set => _settingsSection.Attribute(NEEDTORESTOREWINDOWSTATE_NAME, value);
    }

    public bool IsMaximized
    {
        get => _settingsSection.Attribute<bool>(ISMAXIMIZED_NAME);
        set => _settingsSection.Attribute(ISMAXIMIZED_NAME, value);
    }

    public int WindowHeight
    {
        get => _settingsSection.Attribute<int>(WINDOWHEIGHT_NAME);
        set => _settingsSection.Attribute(WINDOWHEIGHT_NAME, value);
    }

    public int WindowWidth
    {
        get => _settingsSection.Attribute<int>(WINDOWWIDTH_NAME);
        set => _settingsSection.Attribute(WINDOWWIDTH_NAME, value);
    }

    public int WindowLeft
    {
        get => _settingsSection.Attribute<int>(WINDOWLEFT_NAME);
        set => _settingsSection.Attribute(WINDOWLEFT_NAME, value);
    }

    public int WindowTop
    {
        get => _settingsSection.Attribute<int>(WINDOWTOP_NAME);
        set => _settingsSection.Attribute(WINDOWTOP_NAME, value);
    }

    public ShellWindowSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
