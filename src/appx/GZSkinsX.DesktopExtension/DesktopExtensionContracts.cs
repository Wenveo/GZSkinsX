// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;
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
    Task<PackageMetadata?> GetMTPackageMetadata();

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
    PackageMetadata? IValueSetSerializer<PackageMetadata>.Deserialize(ValueSet? valueSet)
    {
        if (valueSet is not null)
        {
            return new PackageMetadata()
            {
                Author = (string)valueSet["author"],
                Version = (string)valueSet["version"],
                SettingsFile = (string)valueSet["settingsFile"],
                ExecutableFile = (string)valueSet["executableFile"],
                MounterStartUpParm = (string)valueSet["mounterStartUpParm"],
                MounterStopParm = (string)valueSet["mounterStopParm"],
                StartUpArgs = (List<PackageMetadataStartUpArgs>)valueSet["startUpArgs"]
            };
        }

        return null;
    }

    ValueSet? IValueSetSerializer<PackageMetadata>.Serialize(PackageMetadata? value)
    {
        if (value is not null)
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
                {        "startUpArgs",     value.StartUpArgs        }
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
sealed class PackageMetadata
{
    public required string Author { get; init; }

    public required string Version { get; init; }

    public required string SettingsFile { get; init; }

    public required string ExecutableFile { get; init; }

    public required string MounterStartUpParm { get; init; }

    public required string MounterStopParm { get; init; }

    public required List<PackageMetadataStartUpArgs> StartUpArgs { get; init; }
}

#if WINDOWS_UWP
public
#else
internal
#endif
sealed class PackageMetadataStartUpArgs
{
    public required string Name { get; init; }

    public required string Args { get; init; }
}
