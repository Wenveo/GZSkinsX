// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;

namespace GZSkinsX.Uwp.UI.SettingsControls;

/// <summary>
/// AutomationPeer for SettingsExpander
/// </summary>
public class SettingsExpanderAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsExpander"/> class.
    /// </summary>
    /// <param name="owner">SettingsExpander</param>
    public SettingsExpanderAutomationPeer(SettingsExpander owner)
        : base(owner)
    {
    }

    /// <summary>
    /// Gets the control type for the element that is associated with the UI Automation peer.
    /// </summary>
    /// <returns>The control type.</returns>
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Button;
    }

    /// <summary>
    /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
    /// differentiates the control represented by this AutomationPeer.
    /// </summary>
    /// <returns>The string that contains the name.</returns>
    protected override string GetClassNameCore()
    {
        var classNameCore = Owner.GetType().Name;
#if DEBUG_AUTOMATION
            System.Diagnostics.Debug.WriteLine("SettingsCardAutomationPeer.GetClassNameCore returns " + classNameCore);
#endif
        return classNameCore;
    }

    /// <summary>
    /// Raises the property changed event for this AutomationPeer for the provided identifier.
    /// Narrator does not announce this due to: https://github.com/microsoft/microsoft-ui-xaml/issues/3469
    /// </summary>
    /// <param name="newValue">New Expanded state</param>
    public void RaiseExpandedChangedEvent(bool newValue)
    {
        var newState = (newValue == true) ?
          ExpandCollapseState.Expanded :
          ExpandCollapseState.Collapsed;

        var oldState = (newState == ExpandCollapseState.Expanded) ?
          ExpandCollapseState.Collapsed :
          ExpandCollapseState.Expanded;

#if !HAS_UNO
        RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldState, newState);
#endif
    }
}
