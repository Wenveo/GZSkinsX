// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Settings;

using Windows.Storage;

namespace GZSkinsX.Settings;

/// <inheritdoc cref="ISettingsService"/>
[Shared, Export(typeof(ISettingsService))]
internal sealed class SettingsService : ISettingsService
{
    /// <summary>
    /// 用于存储本地数据的配置节点
    /// </summary>
    private readonly SettingsSection _localSettingsSection;

    /// <summary>
    /// 用于存储漫游数据的配置节点
    /// </summary>
    private readonly SettingsSection _roamingSettingsSection;

    /// <summary>
    /// 初始化 <see cref="SettingsService"/> 的新实例
    /// </summary>
    public SettingsService()
    {
        var current = ApplicationData.Current;
        _localSettingsSection = new(current.LocalSettings, SettingsType.Local);
        _roamingSettingsSection = new(current.RoamingSettings, SettingsType.Roaming);
    }

    /// <inheritdoc/>
    public void DeleteSection(string name)
    {
        _localSettingsSection.GetOrCreateSection(name);
    }

    /// <inheritdoc/>
    public void DeleteSection(string name, SettingsType type)
    {
        if (type == SettingsType.Roaming)
            _roamingSettingsSection.DeleteSection(name);
        else
            _localSettingsSection.DeleteSection(name);
    }

    /// <inheritdoc/>
    public ISettingsSection GetOrCreateSection(string name)
    {
        return _localSettingsSection.GetOrCreateSection(name);
    }

    /// <inheritdoc/>
    public ISettingsSection GetOrCreateSection(string name, SettingsType type)
    {
        return type == SettingsType.Roaming
            ? _roamingSettingsSection.GetOrCreateSection(name)
            : _localSettingsSection.GetOrCreateSection(name);
    }
}
