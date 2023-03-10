// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace GZSkinsX.Uwp.Composition;

public static partial class CompositionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="path"></param>
    /// <param name="animation"></param>
    /// <returns></returns>
    public static FrameworkElement SetImplicitAnimation(this FrameworkElement element, string path, ICompositionAnimationBase animation)
    {
        SetImplicitAnimation(GetElementVisual(element), path, animation);
        return element;
    }
}
