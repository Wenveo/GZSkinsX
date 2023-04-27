﻿// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;

namespace GZSkinsX.Api.Navigation;

/// <summary>
/// 
/// </summary>
[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class NavigationGroupMetadataAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public required string Guid { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double Order { get; set; }
}

