// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GZSkinsX.Contracts.Helpers;

public static class AssemblyExtensions
{
    public static string? GetAssemblyVersion(this Assembly assembly)
    {
        string? versionString = null;
        try
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            versionString = fileVersionInfo.ProductVersion ?? fileVersionInfo.FileVersion;
        }
        catch (FileNotFoundException)
        {
        }

        if (string.IsNullOrWhiteSpace(versionString))
        {
            var assemblyVersion = assembly.GetName().Version;
            if (assemblyVersion is not null)
            {
                versionString = assemblyVersion.ToString();
            }
        }

        return versionString;
    }
}
