// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;

namespace GZSkinsX.Uwp.IO.Extensions;

/// <summary>
/// 
/// </summary>
public sealed class WindowCorePropertiesBuilder
{
    /// <summary>
    /// 
    /// </summary>
    private readonly HashSet<string> _propertyNames;

    /// <summary>
    /// 
    /// </summary>
    public WindowCorePropertiesBuilder()
    {
        _propertyNames = new HashSet<string>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> Build() => _propertyNames;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public WindowCorePropertiesBuilder WithProperty(WindowCoreProperties value)
    {
        _propertyNames.Add(WindowCorePropertiesConverter.Convert(value));
        return this;
    }
}
