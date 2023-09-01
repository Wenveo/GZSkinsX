// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#if WINDOWS_UWP
#nullable enable
#endif

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
    Task<string> EncryptConfigText(string str);

    Task<string> DecryptConfigText(string str);

    Task ExtractModImage(string input, string output);

    [return: ValueSetSerializer(typeof(ModInfoSerializer))]
    Task<ModInfo> ReadModInfo(string filePath);

    Task<bool> IsMTRunning();

    Task<bool> ProcessLaunch(string executable, string args, bool runAs);

    Task<bool> EnsureEfficiencyMode(int processId);

    Task SetEfficiencyMode(int processId, bool isEnable);

    Task SetOwner(int processId);

    Task<bool> SetWindowText(long windowHandle, string newTitle);
}


#if WINDOWS_UWP
public
#else
internal
#endif
record class ModInfo(string Name, string Author, string Description, string DateTime);


#if WINDOWS_UWP
public
#else
internal
#endif
sealed class ModInfoSerializer : IValueSetSerializer<ModInfo>
{
    ModInfo? IValueSetSerializer<ModInfo>.Deserialize(ValueSet? valueSet)
    {
        if (valueSet is null)
        {
            return null;
        }

        return new ModInfo(
            valueSet[nameof(ModInfo.Name)] as string ?? string.Empty,
            valueSet[nameof(ModInfo.Author)] as string ?? string.Empty,
            valueSet[nameof(ModInfo.Description)] as string ?? string.Empty,
            valueSet[nameof(ModInfo.DateTime)] as string ?? string.Empty);
    }

    ValueSet? IValueSetSerializer<ModInfo>.Serialize(ModInfo? value)
    {
        if (value is null)
        {
            return null;
        }

        return new ValueSet
        {
            { nameof(ModInfo.Name), value.Name },
            { nameof(ModInfo.Author), value.Author },
            { nameof(ModInfo.Description), value.Description },
            { nameof(ModInfo.DateTime), value.DateTime},
        };
    }
}
