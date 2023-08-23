// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

namespace GZSkinsX.Contracts.MyMods;

public readonly struct MyModInfo
{
    public static readonly MyModInfo Empty = new();

    public string Name { get; }

    public string Author { get; }

    public string Description { get; }

    public string DateTime { get; }

    public bool IsEmpty { get; }

    public MyModInfo()
    {
        Name = string.Empty;
        Author = string.Empty;
        Description = string.Empty;
        DateTime = string.Empty;
        IsEmpty = true;
    }

    public MyModInfo(string name, string author, string description, string dateTime)
    {
        Name = name;
        Author = author;
        Description = description;
        DateTime = dateTime;
        IsEmpty = false;
    }
}
