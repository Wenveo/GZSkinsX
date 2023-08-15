// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace GZSkinsX.Controls;

internal sealed class LaunchButtonAutomationPeer(LaunchButton owner)
    : FrameworkElementAutomationPeer(owner), IExpandCollapseProvider, IInvokeProvider
{
    public ExpandCollapseState ExpandCollapseState
    {
        get
        {
            if (Owner is LaunchButton launchButton && launchButton.IsFlyoutOpen)
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
        return nameof(LaunchButton);
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.SplitButton;
    }

    public void Collapse()
    {
        if (Owner is LaunchButton launchButton)
        {
            launchButton.CloseFlyout();
        }
    }

    public void Expand()
    {
        if (Owner is LaunchButton launchButton)
        {
            launchButton.OpenFlyout();
        }
    }

    public void Invoke()
    {
        if (Owner is LaunchButton launchButton)
        {
            launchButton.Invoke();
        }
    }
}
