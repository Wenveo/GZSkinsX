// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Windows.Forms;

namespace GZSkinsX.DesktopExtension;

internal sealed class DesktopExtensionContext : ApplicationContext
{
    private readonly DesktopExtensionMethods _sideCar;

    public DesktopExtensionContext()
    {
        _sideCar = new DesktopExtensionMethods();
        _sideCar.ConnectionFailed += OnTerminateProcess;
        _sideCar.ConnectionFailed += OnTerminateProcess;
        _sideCar.InitializeAppServiceAsync();
    }

    private void OnTerminateProcess(object sender, EventArgs e)
    {
        Program.Exit(0);
    }
}