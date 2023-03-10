// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Numerics;

using Windows.UI;
using Windows.UI.Composition;

namespace GZSkinsX.Uwp.Composition;

public static partial class CompositionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, float value)
    {
        set.InsertScalar(name, value);
        return set;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, bool value)
    {
        set.InsertBoolean(name, value);
        return set;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Vector2 value)
    {
        set.InsertVector2(name, value);
        return set;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Vector3 value)
    {
        set.InsertVector3(name, value);
        return set;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Vector4 value)
    {
        set.InsertVector4(name, value);
        return set;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Color value)
    {
        set.InsertColor(name, value);
        return set;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Matrix3x2 value)
    {
        set.InsertMatrix3x2(name, value);
        return set;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Matrix4x4 value)
    {
        set.InsertMatrix4x4(name, value);
        return set;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="set"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CompositionPropertySet SetValue(this CompositionPropertySet set, string name, Quaternion value)
    {
        set.InsertQuaternion(name, value);
        return set;
    }
}
