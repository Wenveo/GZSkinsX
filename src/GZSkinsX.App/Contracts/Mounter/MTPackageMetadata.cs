// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace GZSkinsX.Contracts.Mounter;

public readonly struct MTPackageMetadata
{
    public static readonly MTPackageMetadata Empty = new();

    public readonly string Author;

    public readonly string Version;

    public readonly string SettingsFile;

    public readonly string ExecutableFile;

    public readonly string ProcStartupArgs;

    public readonly string ProcTerminateArgs;

    public readonly MTPackageMetadataStartUpArgument[] OtherStartupArgs;

    public readonly bool IsEmpty;

    public MTPackageMetadata()
    {
        Author = string.Empty;
        Version = string.Empty;
        SettingsFile = string.Empty;
        ExecutableFile = string.Empty;
        ProcStartupArgs = string.Empty;
        ProcTerminateArgs = string.Empty;
        OtherStartupArgs = Array.Empty<MTPackageMetadataStartUpArgument>();
        IsEmpty = true;
    }

    public MTPackageMetadata(string author, string version, string settingsFile, string executableFile,
        string procStartupArgs, string procTerminateArgs, params MTPackageMetadataStartUpArgument[] otherStartupArgs)
    {
        Author = author;
        Version = version;
        SettingsFile = settingsFile;
        ExecutableFile = executableFile;
        ProcStartupArgs = procStartupArgs;
        ProcTerminateArgs = procTerminateArgs;
        OtherStartupArgs = otherStartupArgs;
        IsEmpty = false;
    }
}
