// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

namespace GZSkinsX.Api.ContextMenu;

[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ContextMenuItemMetadataAttribute : Attribute
{
    public string? Guid { get; set; }

    public string? OwnerGuid { get; set; }

    public string? Group { get; set; }

    public double Order { get; set; }
}
