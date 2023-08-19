// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

using GZSkinsX.Contracts.Settings;

using Windows.Storage;

namespace GZSkinsX.Services.Settings;

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
    private readonly ReaderWriterLockSlim _lockSlim;

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
        _lockSlim = new();
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
        ThrowIfConatinerIsNull(_container);

        _lockSlim.EnterReadLock();
        try
        {
            if (_container.Values.TryGetValue(key, out var value))
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
        ThrowIfConatinerIsNull(_container);

        _lockSlim.EnterReadLock();
        try
        {
            if (_container.Values.TryGetValue(key, out var value))
            {
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
        ThrowIfConatinerIsNull(_container);

        _lockSlim.EnterUpgradeableReadLock();
        try
        {
            var c = TypeDescriptor.GetConverter(typeof(TValue));
            var stringValue = c.ConvertToInvariantString(value);

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
        ThrowIfConatinerIsNull(_container);

        bool b;

        _lockSlim.EnterUpgradeableReadLock();
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
        ThrowIfConatinerIsNull(_container);

        _lockSlim.EnterUpgradeableReadLock();
        try
        {
            if (_nameToSectionDict.TryGetValue(name, out var settingsSection))
            {
                _lockSlim.EnterWriteLock();
                try
                {
                    settingsSection._container = null;
                    _container.DeleteContainer(name);
                    _ = _nameToSectionDict.Remove(name);
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
        ThrowIfConatinerIsNull(_container);

        _lockSlim.EnterReadLock();
        try
        {
            if (_nameToSectionDict.TryGetValue(name, out var settingsSection))
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
            if (!_container.Containers.TryGetValue(name, out var container))
            {
                container = _container.CreateContainer(name, ApplicationDataCreateDisposition.Always);
            }

            _lockSlim.EnterWriteLock();
            try
            {
                var settingsSection2 = new SettingsSection(container, Type);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowIfConatinerIsNull([NotNull] ApplicationDataContainer? container)
    {
        if (container is not null)
        {
            return;
        }

        throw new InvalidOperationException("The container has been deleted. Invalid operation!");
    }
}
