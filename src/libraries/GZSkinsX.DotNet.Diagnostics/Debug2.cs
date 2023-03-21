// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace GZSkinsX.DotNet.Diagnostics;

/// <summary>
/// Provides a set of properties and methods for debugging code.
/// </summary>
public static class Debug2
{
    [Conditional("DEBUG")]
    public static void Assert([DoesNotReturnIf(false)] bool condition)
    => Assert(condition, string.Empty, string.Empty);

    [Conditional("DEBUG")]
    public static void Assert([DoesNotReturnIf(false)] bool condition, string? message)
    => Assert(condition, message, string.Empty);

    [Conditional("DEBUG")]
    public static void Assert(
        [DoesNotReturnIf(false)] bool condition,
        string? message, string? detailMessage)
    {
        if (!condition)
        {
            Debug.Fail(message, detailMessage);
        }
    }

    [Conditional("DEBUG")]
    public static void Assert([DoesNotReturnIf(false)] bool condition, string? message,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string detailMessageFormat,
        params object?[] args)
    => Assert(condition, message, string.Format(detailMessageFormat, args));
}
