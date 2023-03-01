// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

using GZSkinsX.Api.Settings;
using GZSkinsX.Diagnostics;

using Windows.Storage;

namespace GZSkinsX.Settings;

internal sealed class SettingsSection : ISettingsSection
{
    private readonly Dictionary<string, SettingsSection> _nameToSectionDict;

    private ApplicationDataContainer? _container;

    public string Name { get; }

    public SettingsSection(ApplicationDataContainer container)
    {
        _container = container;
        _nameToSectionDict = new Dictionary<string, SettingsSection>();
        foreach (var item in container.Containers)
        {
            _nameToSectionDict.Add(item.Key, new SettingsSection(item.Value));
        }

        Name = container.Name;
    }

    public object Attribute(string key)
    {
        Debug2.Assert(_container is not null);
        if (_container is null) throw new InvalidOperationException();
        if (key == null) throw new ArgumentNullException(nameof(key));

        return _container.Values[key];
    }

    public TValue? Attribute<TValue>(string key) where TValue : class
    {
        Debug2.Assert(_container is not null);
        if (_container is null) throw new InvalidOperationException();
        if (key == null) throw new ArgumentNullException(nameof(key));

        return _container.Values[key] as TValue;
    }

    public void Attribute(string key, object value)
    {
        Debug2.Assert(_container is not null);
        if (_container is null) throw new InvalidOperationException();
        if (key == null) throw new ArgumentNullException(nameof(key));

        _container.Values[key] = value ?? throw new ArgumentNullException(nameof(value));
    }

    public bool Delete(string key)
    {
        Debug2.Assert(_container is not null);
        if (_container is null) throw new InvalidOperationException();
        if (key == null) throw new ArgumentNullException(nameof(key));

        return _container.Values.Remove(key);
    }

    public void DeleteSection(string name)
    {
        if (_container is null)
        {
            return;
        }

        if (_nameToSectionDict.TryGetValue(name, out var settingsSection))
        {
            settingsSection._container = null;
            _nameToSectionDict.Remove(name);
            _container.DeleteContainer(name);
        }
    }

    public ISettingsSection GetOrCreateSection(string name)
    {
        Debug2.Assert(_container is not null);
        if (_container is null) throw new InvalidOperationException();
        if (name == null) throw new ArgumentNullException(nameof(name));

        if (!_nameToSectionDict.TryGetValue(name, out var settingsSection))
        {
            if (!_container.Containers.TryGetValue(name, out var container))
            {
                container = _container.CreateContainer(name, ApplicationDataCreateDisposition.Always);
            }

            _nameToSectionDict.Add(name, settingsSection = new(container));
        }

        return settingsSection;
    }
}
