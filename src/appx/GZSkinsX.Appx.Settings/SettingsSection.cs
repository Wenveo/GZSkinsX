// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;

using GZSkinsX.Api.Settings;
using GZSkinsX.DotNet.Diagnostics;

using Windows.Storage;

namespace GZSkinsX.Appx.Settings;

/// <inheritdoc cref="ISettingsSection"/>
internal sealed class SettingsSection : ISettingsSection
{
    /// <summary>
    /// 用于存储子节点配置的字典，并使用子节点配置的名称当作值的键
    /// </summary>
    private readonly Dictionary<string, SettingsSection> _nameToSectionDict;

    /// <summary>
    /// 线程锁对象，以保证在多线程下资源的同步访问
    /// </summary>
    private readonly object _lockObj;

    /// <summary>
    /// 内部定义的 UWP 中的配置容器
    /// </summary>
    private ApplicationDataContainer? _container;

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public SettingsType Type { get; }

    /// <summary>
    /// 初始化 <see cref="SettingsSection"/> 的新实例
    /// </summary>
    /// <param name="container"></param>
    public SettingsSection(ApplicationDataContainer container, SettingsType settingsType)
    {
        _lockObj = new();
        _container = container;
        _nameToSectionDict = new Dictionary<string, SettingsSection>();
        foreach (var item in container.Containers)
        {
            _nameToSectionDict.Add(item.Key, new SettingsSection(item.Value, settingsType));
        }

        Name = container.Name;
        Type = settingsType;
    }

    /// <inheritdoc/>
    public string? Attribute(string key)
    {
        Debug2.Assert(_container is not null);
        if (_container is null)
            throw new InvalidOperationException();
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        object? value;
        lock (_lockObj)
        {
            if (!_container.Values.TryGetValue(key, out value))
            {
                return default;
            }
        }

        return (string?)value;
    }

    /// <inheritdoc/>
    public TValue? Attribute<TValue>(string key)
    {
        Debug2.Assert(_container is not null);
        if (_container is null)
            throw new InvalidOperationException();
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        object? value;
        lock (_lockObj)
        {
            if (!_container.Values.TryGetValue(key, out value))
            {
                return default;
            }
        }

        var c = TypeDescriptor.GetConverter(typeof(TValue));
        try
        {
            return (TValue?)c.ConvertFromInvariantString((string)value);
        }
        catch (FormatException)
        {
        }
        catch (NotSupportedException)
        {
        }

        return default;
    }

    /// <inheritdoc/>
    public void Attribute<TValue>(string key, TValue value)
    {
        Debug2.Assert(_container is not null);
        if (_container is null)
            throw new InvalidOperationException();
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var c = TypeDescriptor.GetConverter(typeof(TValue));
        var stringValue = c.ConvertToInvariantString(value);

        lock (_lockObj)
            _container.Values[key] = stringValue;
    }

    /// <inheritdoc/>
    public bool Delete(string key)
    {
        Debug2.Assert(_container is not null);
        if (_container is null)
            throw new InvalidOperationException();
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        bool b;
        lock (_lockObj)
            b = _container.Values.Remove(key);

        return b;
    }

    /// <inheritdoc/>
    public void DeleteSection(string name)
    {
        if (_container is null)
        {
            return;
        }

        lock (_lockObj)
        {
            if (_nameToSectionDict.TryGetValue(name, out var settingsSection))
            {
                settingsSection._container = null;
                _container.DeleteContainer(name);
                _nameToSectionDict.Remove(name);
            }
        }
    }

    /// <inheritdoc/>
    public ISettingsSection GetOrCreateSection(string name)
    {
        Debug2.Assert(_container is not null);
        if (_container is null)
            throw new InvalidOperationException();
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        SettingsSection? settingsSection;
        lock (_lockObj)
        {
            if (!_nameToSectionDict.TryGetValue(name, out settingsSection))
            {
                if (!_container.Containers.TryGetValue(name, out var container))
                {
                    container = _container.CreateContainer(name, ApplicationDataCreateDisposition.Always);
                }

                _nameToSectionDict.Add(name, settingsSection = new(container, Type));
            }
        }

        return settingsSection;
    }
}
