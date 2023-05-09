// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;

using GZSkinsX.Api.Commands;

namespace GZSkinsX.Commands;

internal sealed class CommandItemContext
{
    private readonly Lazy<ICommandItem, CommandItemMetadataAttribute> _lazy;

    public ICommandItem Value => _lazy.Value;

    public CommandItemMetadataAttribute Metadata => _lazy.Metadata;

    public CommandItemContext(Lazy<ICommandItem, CommandItemMetadataAttribute> lazy)
    {
        _lazy = lazy;
    }
}
