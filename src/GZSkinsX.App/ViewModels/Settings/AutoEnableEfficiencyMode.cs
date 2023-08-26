// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Diagnostics;

using GZSkinsX.Contracts.Extension;

namespace GZSkinsX.ViewModels;

[Shared, ExportAdvanceExtension]
[AdvanceExtensionMetadata(Order = double.MaxValue)]
internal sealed class AutoEnableEfficiencyMode : IAdvanceExtension
{
    [ImportingConstructor]
    public AutoEnableEfficiencyMode(AutoEnableEfficiencyModeSettings settings)
    {
        if (settings.ShouldEnableEfficiencyMode)
        {
            App.DesktopExtensionMethods.SetEfficiencyMode(Process.GetCurrentProcess().Id, true);
        }
    }
}
