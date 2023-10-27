// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Composition;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.ContextMenu;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using Windows.System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace GZSkinsX.Appx.MainApp.Menus;

#region --- Shell System Menu / Group / General ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.SHELL_SYSTEMMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_SHELL_SYSTEMMENU_CTX_GENERAL_GUID,
    Guid = "6EAC41F5-2E11-44FC-802C-69D4472B0593", Order = 0)]
sealed file class RestoreMenuItem : ContextMenuItemBase<ShellSystemMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE923");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Shell_SystemMenu_Restore");

    public override IEnumerable<KeyboardAccelerator> KeyboardAccelerators
    {
        get
        {
            yield return new() { Key = VirtualKey.R };
        }
    }

    public override bool IsEnabled(ShellSystemMenuUIContext context)
    {
        return context.IsMaximized;
    }

    public override void OnExecute(ShellSystemMenuUIContext context)
    {
        PInvoke.ShowWindow((HWND)context.WindowHandle, SHOW_WINDOW_CMD.SW_SHOWNORMAL);
    }
}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.SHELL_SYSTEMMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_SHELL_SYSTEMMENU_CTX_GENERAL_GUID,
    Guid = "A8CF19EE-58C3-44D7-AEBC-7ED748C6D60B", Order = 1)]
sealed file class MoveMenuItem : ContextMenuItemBase<ShellSystemMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE7C2");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Shell_SystemMenu_Move");

    public override IEnumerable<KeyboardAccelerator> KeyboardAccelerators
    {
        get
        {
            yield return new() { Key = VirtualKey.M };
        }
    }

    public override bool IsEnabled(ShellSystemMenuUIContext context)
    {
        return context.IsMaximized is false;
    }

    public override void OnExecute(ShellSystemMenuUIContext context)
    {
        PInvoke.SendMessage((HWND)context.WindowHandle, PInvoke.WM_SYSCOMMAND, new(0xF010), new(nint.Zero));
    }
}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.SHELL_SYSTEMMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_SHELL_SYSTEMMENU_CTX_GENERAL_GUID,
    Guid = "4FF37935-A653-4F86-9725-5703CDB8F70A", Order = 2)]
sealed file class SizeMenuItem : ContextMenuItemBase<ShellSystemMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE740");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Shell_SystemMenu_Size");

    public override IEnumerable<KeyboardAccelerator> KeyboardAccelerators
    {
        get
        {
            yield return new() { Key = VirtualKey.S };
        }
    }

    public override bool IsEnabled(ShellSystemMenuUIContext context)
    {
        return context.IsMaximized is false;
    }

    public override void OnExecute(ShellSystemMenuUIContext context)
    {
        PInvoke.SendMessage((HWND)context.WindowHandle, PInvoke.WM_SYSCOMMAND, new(0xF000), new(nint.Zero));
    }
}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.SHELL_SYSTEMMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_SHELL_SYSTEMMENU_CTX_GENERAL_GUID,
    Guid = "CAC6D056-0A50-48C7-882C-5FC31639D142", Order = 3)]
sealed file class MinimizeMenuItem : ContextMenuItemBase<ShellSystemMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE921");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Shell_SystemMenu_Minimize");

    public override IEnumerable<KeyboardAccelerator> KeyboardAccelerators
    {
        get
        {
            yield return new() { Key = VirtualKey.N };
        }
    }

    public override void OnExecute(ShellSystemMenuUIContext context)
    {
        PInvoke.ShowWindow((HWND)context.WindowHandle, SHOW_WINDOW_CMD.SW_MINIMIZE);
    }
}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.SHELL_SYSTEMMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_SHELL_SYSTEMMENU_CTX_GENERAL_GUID,
    Guid = "B32DF8D0-DAA2-4604-BC0F-552505F43BB7", Order = 4)]
sealed file class MaximizeMenuItem : ContextMenuItemBase<ShellSystemMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE922");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Shell_SystemMenu_Maximize");

    public override IEnumerable<KeyboardAccelerator> KeyboardAccelerators
    {
        get
        {
            yield return new() { Key = VirtualKey.X };
        }
    }

    public override bool IsEnabled(ShellSystemMenuUIContext context)
    {
        return context.IsMaximized is false;
    }

    public override void OnExecute(ShellSystemMenuUIContext context)
    {
        PInvoke.ShowWindow((HWND)context.WindowHandle, SHOW_WINDOW_CMD.SW_MAXIMIZE);
    }
}

#endregion --- Shell Window System Menu / Group / General ---


#region --- Shell System Menu / Group / Settings ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.SHELL_SYSTEMMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_SHELL_SYSTEMMENU_CTX_SETTINGS_GUID,
    Guid = "A3D28798-979F-40FD-9B46-EDC64DE7C1D3", Order = 0)]
sealed file class SettingsMenuItem : ContextMenuItemBase<ShellSystemMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE713") { Margin = new(-2, -2, -2, -1) };

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Shell_SystemMenu_Settings");

    public override IEnumerable<KeyboardAccelerator> KeyboardAccelerators
    {
        get
        {
            yield return new() { Key = VirtualKey.I };
        }
    }

    public override bool IsVisible(ShellSystemMenuUIContext context)
    {
        return AppxContext.GameService.EnsureGameDataIsValid();
    }

    public override void OnExecute(ShellSystemMenuUIContext context)
    {
        AppxContext.WindowManagerService.NavigateTo(WindowFrameConstants.Settings_Guid);
    }
}

#endregion --- Shell Window System Menu / Group / Settings ---


#region --- Shell System Menu / Group / Close ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.SHELL_SYSTEMMENU_CTX_GUID,
    Group = ContextMenuConstants.GROUP_SHELL_SYSTEMMENU_CTX_CLOSE_GUID,
    Guid = "5B7D747B-937E-4F1F-BF3F-0C4003687C70", Order = 0)]
sealed file class CloseMenuItem : ContextMenuItemBase<ShellSystemMenuUIContext>
{
    public override IconElement? Icon => new SegoeFluentIcon("\uE8BB");

    public override string? Header => ResourceHelper.GetLocalized(
        "GZSkinsX.Appx.MainApp/Resources/Shell_SystemMenu_Close");

    public override IEnumerable<KeyboardAccelerator> KeyboardAccelerators
    {
        get
        {
            yield return new() { Key = VirtualKey.F4, Modifiers = VirtualKeyModifiers.Menu };
            yield return new() { Key = VirtualKey.C };
        }
    }

    public override void OnExecute(ShellSystemMenuUIContext context)
    {
        PInvoke.SendMessage((HWND)context.WindowHandle, PInvoke.WM_CLOSE, new(nuint.Zero), new(nint.Zero));
    }
}

#endregion --- Shell Window System Menu / Group / Close ---
