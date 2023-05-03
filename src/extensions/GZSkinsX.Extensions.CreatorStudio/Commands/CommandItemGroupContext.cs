// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;

namespace GZSkinsX.Extensions.CreatorStudio.Commands;

internal sealed class CommandItemGroupContext
{
    public string Name { get; }

    public double Order { get; }

    public List<CommandItemContext> Items { get; }

    public CommandItemGroupContext(string name, double order)
    {
        Name = name;
        Order = order;
        Items = new List<CommandItemContext>();
    }
}
