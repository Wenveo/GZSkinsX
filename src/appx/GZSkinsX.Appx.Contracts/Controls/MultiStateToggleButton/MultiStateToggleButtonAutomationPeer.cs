// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace GZSkinsX.Contracts.Controls;

public sealed class MultiStateToggleButtonAutomationPeer(MultiStateToggleButton owner)
    : FrameworkElementAutomationPeer(owner), IExpandCollapseProvider, IInvokeProvider
{
    public ExpandCollapseState ExpandCollapseState
    {
        get
        {
            if (Owner is MultiStateToggleButton owner && owner.IsFlyoutOpen)
            {
                return ExpandCollapseState.Expanded;
            }

            return ExpandCollapseState.Collapsed;
        }
    }

    protected override object GetPatternCore(PatternInterface patternInterface)
    {
        if (patternInterface is PatternInterface.ExpandCollapse or PatternInterface.Invoke)
        {
            return this;
        }

        return base.GetPatternCore(patternInterface);
    }

    protected override string GetClassNameCore()
    {
        return nameof(MultiStateToggleButton);
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.SplitButton;
    }

    public void Collapse()
    {
        if (Owner is MultiStateToggleButton owner)
        {
            owner.CloseFlyout();
        }
    }

    public void Expand()
    {
        if (Owner is MultiStateToggleButton owner)
        {
            owner.OpenFlyout();
        }
    }

    public void Invoke()
    {
        if (Owner is MultiStateToggleButton owner)
        {
            owner.Invoke();
        }
    }
}
