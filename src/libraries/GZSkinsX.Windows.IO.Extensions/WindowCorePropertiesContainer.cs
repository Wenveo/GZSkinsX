// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GZSkinsX.Windows.IO.Extensions;

/// <summary>
/// 
/// </summary>
public sealed class WindowCorePropertiesContainer
{
    /// <summary>
    /// 
    /// </summary>
    private readonly IDictionary<string, object> _properties;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="properties"></param>
    public WindowCorePropertiesContainer(IDictionary<string, object> properties)
    {
        _properties = properties;
    }

    /// <summary>
    /// 
    /// </summary>
    public IDictionary<string, object> Raw => _properties;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object? this[WindowCoreProperties key] => _properties[WindowCorePropertiesConverter.Convert(key)];

    /// <summary>
    /// 
    /// </summary>
    public int Count => _properties.Count;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(WindowCoreProperties key) => _properties.ContainsKey(WindowCorePropertiesConverter.Convert(key));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(WindowCoreProperties key, [NotNullWhen(true)] out object? value) => _properties.TryGetValue(WindowCorePropertiesConverter.Convert(key), out value);
}
