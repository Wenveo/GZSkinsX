// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.Mods;

public readonly struct WGZModInfo(string name, string author, string description, string dateTime)
{
    public readonly string Name = name;
    public readonly string Author = author;
    public readonly string Description = description;
    public readonly string DateTime = dateTime;
}
