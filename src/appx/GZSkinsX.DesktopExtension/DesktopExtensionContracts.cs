// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using CommunityToolkit.AppServices;

using Windows.Foundation.Collections;

namespace GZSkinsX.DesktopExtension;

[AppService("GZXDesktopExtension-AppService")]
#if WINDOWS_UWP
public
#else
internal
#endif 
interface IDesktopExtensionMethods
{
    [return: ValueSetSerializer(typeof(PackageMetadataSerializer))]
    Task<PackageMetadata> GetMTPackageMetadata();

    Task<bool> CheckUpdateForMounter();

    Task UpdateMounter();

    Task SetOwner(int processId);
}

#if WINDOWS_UWP
public
#else
internal
#endif
sealed class PackageMetadataSerializer : IValueSetSerializer<PackageMetadata>
{
    PackageMetadata IValueSetSerializer<PackageMetadata>.Deserialize(ValueSet? valueSet)
    {
        if (valueSet is not null)
        {
            return new PackageMetadata(
                (string)valueSet["author"], (string)valueSet["version"],
                (string)valueSet["settingsFile"], (string)valueSet["executableFile"],
                (string)valueSet["mounterStartUpParm"], (string)valueSet["mounterStopParm"],
                ((string[])valueSet["startUpArgNames"]).Zip((string[])valueSet["startUpArgValues"],
                (a, b) => new PackageMetadataStartUpArgs(a, b)).ToArray());
        }

        return PackageMetadata.Empty;
    }

    ValueSet? IValueSetSerializer<PackageMetadata>.Serialize(PackageMetadata value)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        string[] GetStartUpArgNames()
        {
            return value.StartUpArgs.Select(a => a.Name).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        string[] GetStartUpArgValues()
        {
            return value.StartUpArgs.Select(a => a.Value).ToArray();
        }

        if (!value.IsEmpty)
        {
#pragma warning disable format
            return new ValueSet
            {
                {             "author",     value.Author             },
                {            "version",     value.Version            },
                {       "settingsFile",     value.SettingsFile       },
                {     "executableFile",     value.ExecutableFile     },
                { "mounterStartUpParm",     value.MounterStartUpParm },
                {    "mounterStopParm",     value.MounterStopParm    },
                {    "startUpArgNames",     GetStartUpArgNames()     },
                {   "startUpArgValues",     GetStartUpArgValues()    },
            };
#pragma warning restore format
        }

        return null;
    }
}

#if WINDOWS_UWP
public
#else
internal
#endif
readonly struct PackageMetadata
{
    public static readonly PackageMetadata Empty = new();

    public readonly string Author;

    public readonly string Version;

    public readonly string SettingsFile;

    public readonly string ExecutableFile;

    public readonly string MounterStartUpParm;

    public readonly string MounterStopParm;

    public readonly PackageMetadataStartUpArgs[] StartUpArgs;

    public readonly bool IsEmpty = true;

    public PackageMetadata(string author, string version, string settingsFile, string executableFile,
        string mounterStartUpParm, string mounterStopParm, PackageMetadataStartUpArgs[] startUpArgs)
    {
        Author = author;
        Version = version;
        SettingsFile = settingsFile;
        ExecutableFile = executableFile;
        MounterStartUpParm = mounterStartUpParm;
        MounterStopParm = mounterStopParm;
        StartUpArgs = startUpArgs;
        IsEmpty = false;
    }
}

#if WINDOWS_UWP
public
#else
internal
#endif
readonly struct PackageMetadataStartUpArgs
{
    public readonly string Name;

    public readonly string Value;

    public PackageMetadataStartUpArgs(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
