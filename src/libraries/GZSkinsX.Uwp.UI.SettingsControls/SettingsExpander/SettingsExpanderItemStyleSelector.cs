// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Uwp.UI.SettingsControls;

/// <summary>
/// <see cref="StyleSelector"/> used by <see cref="SettingsExpander"/> to choose the proper <see cref="SettingsCard"/> container style (clickable or not).
/// </summary>
public class SettingsExpanderItemStyleSelector : StyleSelector
{
    /// <summary>
    /// Gets or sets the default <see cref="Style"/>.
    /// </summary>
    public Style DefaultStyle { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Style"/> when clickable.
    /// </summary>
    public Style ClickableStyle { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsExpanderItemStyleSelector"/> class.
    /// </summary>
    public SettingsExpanderItemStyleSelector()
    {
    }

    /// <inheritdoc/>
    protected override Style SelectStyleCore(object item, DependencyObject container)
    {
        if (container is SettingsCard card && card.IsClickEnabled)
        {
            return ClickableStyle;
        }
        else
        {
            return DefaultStyle;
        }
    }
}
