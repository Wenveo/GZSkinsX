// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Hashing;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace GZSkinsX.Extensions.CreatorStudio.Wad;

internal static class WadHashTable
{
    private const string HASHTABLE_URI = "ms-appx:///GZSkinsX.Extensions.CreatorStudio.Wad/Hashes/hashes.game.txt";

    private static readonly Dictionary<ulong, string> s_hashNames = new();

    public static async Task InitializeAsync()
    {
        if (s_hashNames.Count != 0)
        {
            return;
        }

        var hashTable = await StorageFile.GetFileFromApplicationUriAsync(new(HASHTABLE_URI, UriKind.Absolute));
        var allLines = await FileIO.ReadLinesAsync(hashTable);

        foreach (var line in allLines)
        {
            var indexOfWhiteSpace = line.IndexOf(' ');

            var hash = ulong.Parse(line[..indexOfWhiteSpace], System.Globalization.NumberStyles.HexNumber);
            var name = line[++indexOfWhiteSpace..];

            s_hashNames.Add(hash, name);
        }
    }

    public static string GetNameOrDefault(ulong hash)
    {
        if (!s_hashNames.TryGetValue(hash, out var name))
        {
            name = hash.ToString("X");
        }

        return name;
    }
}
