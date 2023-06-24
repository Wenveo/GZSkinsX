// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Settings;

namespace GZSkinsX.MainApp;

[Shared, Export]
internal sealed class ShellWindowSettings
{
#pragma warning disable format
    private const string THE_GUID                       = "98376233-8B50-4E6C-8CC5-7267EBA2B89B";
    private const string RESTOREWINDOWPOSITION_NAME     = "RestoreWindowPosition";
    private const string ISMAXIMIZED_NAME               = "IsMaximized";
    private const string WINDOWHEIGHT_NAME              = "WindowHeight";
    private const string WINDOWWIDTH_NAME               = "WindowWidth";
    private const string X_AXIS_NAME                    = "X_Axis";
    private const string Y_AXIS_NAME                    = "Y_Axis";
#pragma warning restore format

    private readonly ISettingsSection _settingsSection;

    public bool RestoreWindowPosition
    {
        get => _settingsSection.Attribute<bool>(RESTOREWINDOWPOSITION_NAME);
        set => _settingsSection.Attribute(RESTOREWINDOWPOSITION_NAME, value);
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

    public int X_Axis
    {
        get => _settingsSection.Attribute<int>(X_AXIS_NAME);
        set => _settingsSection.Attribute(X_AXIS_NAME, value);
    }

    public int Y_Axis
    {
        get => _settingsSection.Attribute<int>(Y_AXIS_NAME);
        set => _settingsSection.Attribute(Y_AXIS_NAME, value);
    }

    public ShellWindowSettings()
    {
        _settingsSection = AppxContext.SettingsService.GetOrCreateSection(THE_GUID);
    }
}
