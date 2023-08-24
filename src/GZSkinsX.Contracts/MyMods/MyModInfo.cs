// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

namespace GZSkinsX.Contracts.MyMods;

public sealed record class MyModInfo(string Name, string Author, string Description, string DateTime)
{
    public static readonly MyModInfo Empty = new(string.Empty, string.Empty, string.Empty, string.Empty);
}
