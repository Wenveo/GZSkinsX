// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using GZSkinsX.Api.ContextMenu;

using Windows.UI.Xaml;

namespace GZSkinsX.ContextMenu;

internal sealed class ContextMenuFlyoutHelper
{
    public static readonly DependencyProperty CoerceValueCallbackProperty =
        DependencyProperty.RegisterAttached("CoerceValueCallback", typeof(CoerceContextMenuUIContextCallback),
            typeof(ContextMenuFlyoutHelper), new PropertyMetadata(null));

    public static CoerceContextMenuUIContextCallback GetCoerceValueCallback(DependencyObject obj)
    => (CoerceContextMenuUIContextCallback)obj.GetValue(CoerceValueCallbackProperty);

    public static void SetCoerceValueCallback(DependencyObject obj, CoerceContextMenuUIContextCallback value)
    => obj.SetValue(CoerceValueCallbackProperty, value);
}
