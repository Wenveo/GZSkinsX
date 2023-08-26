// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;

namespace GZSkinsX.DesktopExtension;

internal static class EfficiencyManager
{
    [Flags]
    private enum ProcessorPowerThrottlingFlags : uint
    {
        None = 0x0,
        PROCESS_POWER_THROTTLING_EXECUTION_SPEED = 0x1,
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_POWER_THROTTLING_STATE
    {
        public const uint PROCESS_POWER_THROTTLING_CURRENT_VERSION = 1;

        public uint Version;
        public ProcessorPowerThrottlingFlags ControlMask;
        public ProcessorPowerThrottlingFlags StateMask;
    }

    private static readonly IntPtr s_pThrottleOn;
    private static readonly IntPtr s_pThrottleOff;
    private static readonly int s_szControlBlock;

    static EfficiencyManager()
    {
        s_szControlBlock = Marshal.SizeOf<PROCESS_POWER_THROTTLING_STATE>();
        s_pThrottleOn = Marshal.AllocHGlobal(s_szControlBlock);
        s_pThrottleOff = Marshal.AllocHGlobal(s_szControlBlock);

        var throttleState = new PROCESS_POWER_THROTTLING_STATE
        {
            Version = PROCESS_POWER_THROTTLING_STATE.PROCESS_POWER_THROTTLING_CURRENT_VERSION,
            ControlMask = ProcessorPowerThrottlingFlags.PROCESS_POWER_THROTTLING_EXECUTION_SPEED,
            StateMask = ProcessorPowerThrottlingFlags.PROCESS_POWER_THROTTLING_EXECUTION_SPEED
        };

        var unthrottleState = new PROCESS_POWER_THROTTLING_STATE
        {
            Version = PROCESS_POWER_THROTTLING_STATE.PROCESS_POWER_THROTTLING_CURRENT_VERSION,
            ControlMask = ProcessorPowerThrottlingFlags.PROCESS_POWER_THROTTLING_EXECUTION_SPEED,
            StateMask = ProcessorPowerThrottlingFlags.None,
        };

        Marshal.StructureToPtr(throttleState, s_pThrottleOn, false);
        Marshal.StructureToPtr(unthrottleState, s_pThrottleOff, false);
    }

    public static bool EnsureEfficiencyMode(IntPtr handle)
    {
        return PInvoke.GetPriorityClass(new HANDLE(handle)) == (uint)PROCESS_CREATION_FLAGS.IDLE_PRIORITY_CLASS;
    }

    public static void SetEfficiencyMode(IntPtr handle, bool isEnable)
    {
        unsafe
        {
            PInvoke.SetProcessInformation(new HANDLE(handle),
                    PROCESS_INFORMATION_CLASS.ProcessPowerThrottling,
                    (isEnable ? s_pThrottleOn : s_pThrottleOff).ToPointer(),
                    (uint)s_szControlBlock);

            PInvoke.SetPriorityClass(new HANDLE(handle), isEnable ?
                    PROCESS_CREATION_FLAGS.IDLE_PRIORITY_CLASS :
                    PROCESS_CREATION_FLAGS.NORMAL_PRIORITY_CLASS);
        }
    }
}
