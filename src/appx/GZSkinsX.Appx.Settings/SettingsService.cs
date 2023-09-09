// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Appx.Contracts.Settings;

using System.Composition;

namespace GZSkinsX.Appx.Settings;

/// <inheritdoc cref="ISettingsService"/>
[Shared, Export(typeof(ISettingsService))]
internal sealed class SettingsService : ISettingsService
{
    /// <summary>
    /// 用于存储本地数据的配置节点。
    /// </summary>
    private readonly SettingsSection _localSettingsSection;

    /// <summary>
    /// 用于存储漫游数据的配置节点。
    /// </summary>
    private readonly SettingsSection _roamingSettingsSection;

    /// <summary>
    /// 初始化 <see cref="SettingsService"/> 的新实例。
    /// </summary>
    public SettingsService()
    {
        Windows.Storage.ApplicationData current = Windows.Storage.ApplicationData.Current;
        _roamingSettingsSection = new(current.RoamingSettings, SettingsType.Roaming);
        _localSettingsSection = new(current.LocalSettings, SettingsType.Local);
    }

    /// <inheritdoc/>
    public void DeleteSection(string name)
    {
        _localSettingsSection.DeleteSection(name);
    }

    /// <inheritdoc/>
    public void DeleteSection(string name, SettingsType type)
    {
        if (type is SettingsType.Roaming)
        {
            _roamingSettingsSection.DeleteSection(name);
        }
        else
        {
            _localSettingsSection.DeleteSection(name);
        }
    }

    /// <inheritdoc/>
    public ISettingsSection GetOrCreateSection(string name)
    {
        return _localSettingsSection.GetOrCreateSection(name);
    }

    /// <inheritdoc/>
    public ISettingsSection GetOrCreateSection(string name, SettingsType type)
    {
        return type is SettingsType.Roaming
            ? _roamingSettingsSection.GetOrCreateSection(name)
            : _localSettingsSection.GetOrCreateSection(name);
    }
}
