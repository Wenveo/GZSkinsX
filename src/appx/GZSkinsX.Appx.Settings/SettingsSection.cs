// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Appx.Contracts.Settings;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

using Windows.Storage;

namespace GZSkinsX.Appx.Settings;

/// <inheritdoc cref="ISettingsSection"/>
internal sealed class SettingsSection : ISettingsSection
{
    /// <summary>
    /// 用于存储子节点配置的字典，并使用子节点配置的名称当作值的键。
    /// </summary>
    private readonly Dictionary<string, SettingsSection> _nameToSectionDict;

    /// <summary>
    /// 线程锁对象，以保证在多线程下资源的同步访问。
    /// </summary>
    private readonly ReaderWriterLockSlim _lockSlim;

    /// <summary>
    /// 内部定义的 UWP 中的配置容器。
    /// </summary>
    private ApplicationDataContainer? _container;

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public SettingsType Type { get; }

    /// <summary>
    /// 通过指定的 <see cref="ApplicationDataContainer"/> 和 <see cref="SettingsType"/> 来初始化 <see cref="SettingsSection"/> 的新实例。
    /// </summary>
    /// <param name="container">用于存储应用程序数据的容器。</param>
    /// <param name="settingsType">指定该设置节点的类型。</param>
    public SettingsSection(ApplicationDataContainer container, SettingsType settingsType)
    {
        _lockSlim = new();
        _container = container;
        _nameToSectionDict = new Dictionary<string, SettingsSection>();
        foreach (KeyValuePair<string, ApplicationDataContainer> item in container.Containers)
        {
            _nameToSectionDict.Add(item.Key, new SettingsSection(item.Value, settingsType));
        }

        Name = container.Name;
        Type = settingsType;
    }

    /// <inheritdoc/>
    public string? Attribute(string key)
    {
        EnsureConatinerIsNotNull();
        _lockSlim.EnterReadLock();
        try
        {
            if (_container.Values.TryGetValue(key, out object? value))
            {
                return (string?)value;
            }
        }
        finally
        {
            _lockSlim.ExitReadLock();
        }

        return default;
    }

    /// <inheritdoc/>
    public TValue? Attribute<TValue>(string key)
    {
        EnsureConatinerIsNotNull();
        _lockSlim.EnterReadLock();
        try
        {
            if (_container.Values.TryGetValue(key, out object? value))
            {
                TypeConverter c = TypeDescriptor.GetConverter(typeof(TValue));
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
            }
        }
        finally
        {
            _lockSlim.ExitReadLock();
        }

        return default;
    }

    /// <inheritdoc/>
    public void Attribute<TValue>(string key, TValue value)
    {
        EnsureConatinerIsNotNull();
        _lockSlim.EnterUpgradeableReadLock();
        try
        {
            TypeConverter c = TypeDescriptor.GetConverter(typeof(TValue));
            string? stringValue = c.ConvertToInvariantString(value);

            _lockSlim.EnterWriteLock();
            try
            {
                _container.Values[key] = stringValue;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }
        finally
        {
            _lockSlim.ExitUpgradeableReadLock();
        }
    }

    /// <inheritdoc/>
    public bool Delete(string key)
    {
        EnsureConatinerIsNotNull();
        _lockSlim.EnterUpgradeableReadLock();

        bool b;
        try
        {
            b = _container.Values.Remove(key);
        }
        finally
        {
            _lockSlim.ExitUpgradeableReadLock();
        }

        return b;
    }

    /// <inheritdoc/>
    public void DeleteSection(string name)
    {
        EnsureConatinerIsNotNull();
        _lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (_nameToSectionDict.TryGetValue(name, out SettingsSection? settingsSection))
            {
                _lockSlim.EnterWriteLock();
                try
                {
                    settingsSection._container = null;
                    _container.DeleteContainer(name);
                    _nameToSectionDict.Remove(name);
                }
                finally
                {
                    _lockSlim.ExitWriteLock();
                }
            }
        }
        finally
        {
            _lockSlim.ExitUpgradeableReadLock();
        }
    }

    /// <inheritdoc/>
    public ISettingsSection GetOrCreateSection(string name)
    {
        EnsureConatinerIsNotNull();
        _lockSlim.EnterReadLock();
        try
        {
            if (_nameToSectionDict.TryGetValue(name, out SettingsSection? settingsSection))
            {
                return settingsSection;
            }
        }
        finally
        {
            _lockSlim.ExitReadLock();
        }

        _lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (!_container.Containers.TryGetValue(name, out ApplicationDataContainer? container))
            {
                container = _container.CreateContainer(name, ApplicationDataCreateDisposition.Always);
            }

            _lockSlim.EnterWriteLock();
            try
            {
                SettingsSection settingsSection2 = new(container, Type);
                _nameToSectionDict.Add(name, settingsSection2);

                return settingsSection2;
            }
            finally
            {
                _lockSlim.ExitWriteLock();
            }
        }
        finally
        {
            _lockSlim.ExitUpgradeableReadLock();
        }
    }

    [MemberNotNull(nameof(_container))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureConatinerIsNotNull()
    {
        if (_container is not null)
        {
            return;
        }

        throw new InvalidOperationException("The container has been deleted. Invalid operation!");
    }
}
