// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Api.Controls;

//// Note: ItemsRepeater will request all the available horizontal space: https://github.com/microsoft/microsoft-ui-xaml/issues/3842
[TemplatePart(Name = PART_ItemsRepeater, Type = typeof(MUXC.ItemsRepeater))]
public partial class SettingsExpander : Control
{
    private const string PART_ItemsRepeater = "PART_ItemsRepeater";

    private MUXC.ItemsRepeater? _itemsRepeater;

    /// <summary>
    /// The SettingsExpander is a collapsable control to host multiple SettingsCards.
    /// </summary>
    public SettingsExpander()
    {
        DefaultStyleKey = typeof(SettingsExpander);
        Items = new List<object>();
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        RegisterAutomation();

        if (_itemsRepeater != null)
        {
            _itemsRepeater.ElementPrepared -= ItemsRepeater_ElementPrepared;
        }

        _itemsRepeater = GetTemplateChild(PART_ItemsRepeater) as MUXC.ItemsRepeater;

        if (_itemsRepeater != null)
        {
            _itemsRepeater.ElementPrepared += ItemsRepeater_ElementPrepared;

            // Update it's source based on our current items properties.
            OnItemsConnectedPropertyChanged(this, null!); // Can't get it to accept type here? (DependencyPropertyChangedEventArgs)EventArgs.Empty
        }
    }

    private void RegisterAutomation()
    {
        if (Header is string headerString && headerString != string.Empty)
        {
            if (!string.IsNullOrEmpty(headerString) && string.IsNullOrEmpty(AutomationProperties.GetName(this)))
            {
                AutomationProperties.SetName(this, headerString);
            }
        }
    }

    /// <summary>
    /// Creates AutomationPeer
    /// </summary>
    /// <returns>An automation peer for <see cref="SettingsExpander"/>.</returns>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SettingsExpanderAutomationPeer(this);
    }

    private void OnIsExpandedChanged(bool oldValue, bool newValue)
    {
        var peer = FrameworkElementAutomationPeer.FromElement(this) as SettingsExpanderAutomationPeer;
        peer?.RaiseExpandedChangedEvent(newValue);
    }
}
