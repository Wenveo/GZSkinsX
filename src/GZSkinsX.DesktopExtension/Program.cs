// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Windows.Forms;

namespace GZSkinsX.DesktopExtension;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DesktopExtensionContext());
        }
        catch (Exception excp)
        {
            Exit(excp.HResult);
        }
    }

    internal static void Exit(int exitCode)
    {
        Environment.Exit(exitCode);
    }
}
