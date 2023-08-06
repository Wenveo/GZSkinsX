// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Threading.Tasks;

namespace GZSkinsX.Helpers;

internal static class TaskExtensions
{
    public static void FireAndForget(this Task _)
    {
        // This method allows you to call an async method without awaiting it.
        // Use it when you don't want or need to wait for the task to complete.
    }
}
