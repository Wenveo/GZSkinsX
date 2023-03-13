// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Settings;

namespace GZSkinsX.Appx.Preload;

/// <summary>
/// 
/// </summary>
[Shared, Export]
internal sealed class PreloadSettings
{
    /// <summary>
    /// 
    /// </summary>
    private const string THE_GUID = "F4523D71-8866-4241-9F5B-D56D850BD878";

    /// <summary>
    /// 
    /// </summary>
    private const string ISINITIALIZE_NAME = "IsInitialize";

    /// <summary>
    /// 
    /// </summary>
    private readonly ISettingsSection _settingsSection;

    /// <summary>
    /// 
    /// </summary>
    public bool IsInitialize
    {
        get => _settingsSection.Attribute<bool>(ISINITIALIZE_NAME);
        set => _settingsSection.Attribute(ISINITIALIZE_NAME, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="settingsService"></param>
    [ImportingConstructor]
    public PreloadSettings(ISettingsService settingsService)
    {
        _settingsSection = settingsService.GetOrCreateSection(THE_GUID);
    }
}
