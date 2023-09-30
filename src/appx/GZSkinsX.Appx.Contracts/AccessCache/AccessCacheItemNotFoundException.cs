// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace GZSkinsX.Contracts.AccessCache;

/// <summary>
/// 表示在找不到指定的可访问缓存项时发生的异常。
/// </summary>
/// <param name="itemName">缓存项的名称。</param>
public sealed class AccessCacheItemNotFoundException(string itemName)
    : Exception($"No item with a matching name was found in the stored list: {itemName}")
{ }
