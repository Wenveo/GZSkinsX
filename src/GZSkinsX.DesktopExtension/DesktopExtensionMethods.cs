// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Windows.Win32;

namespace GZSkinsX.DesktopExtension;

internal sealed partial class DesktopExtensionMethods : IDesktopExtensionMethods
{
    public Task<bool> IsMTRunning()
    {
        using var handle = PInvoke.OpenFileMapping(0xF001F, false, "Gz_services:execute");
        return Task.FromResult(handle.IsInvalid is false);
    }

    public Task<bool> ProcessLaunch(string executable, string args, bool runAs)
    {
        var startInfo = new ProcessStartInfo
        {
            Arguments = args,
            FileName = executable,
            UseShellExecute = false,
            Verb = runAs ? "RunAs" : string.Empty,
            WorkingDirectory = Path.GetDirectoryName(executable)
        };

        var taskCompletionSource = new TaskCompletionSource<bool>();

        try
        {
            using var process = Process.Start(startInfo);
            taskCompletionSource.SetResult(process.Handle != IntPtr.Zero);
        }
        catch (Exception excp)
        {
            taskCompletionSource.SetException(excp);
        }

        return taskCompletionSource.Task;
    }

    public Task SetOwner(int processId)
    {
        try
        {
            var owner = Process.GetProcessById(processId);
            owner.EnableRaisingEvents = true;
            owner.Exited += (_, _) => Program.Exit(0);
        }
        catch
        {

        }

        return Task.CompletedTask;
    }
}
