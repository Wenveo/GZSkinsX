// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.ContextMenu;

namespace GZSkinsX.Appx.MyMods.Commands;

#region --- MyModsView / ContextMenu [Win10] / Group / Install ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_Install_GUID,
    Guid = "590CD0D6-FE13-4487-99E0-37D3924B1E02", Order = 0)]
sealed file class InstallMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Install)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_Install_GUID,
    Guid = "552919A6-82F6-4717-A64F-71904A6C6D7D", Order = 1)]
sealed file class UninstallMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Uninstall)
{

}

#endregion --- MyModsView / ContextMenu [Win10] / Group / Install ---


#region --- MyModsView / ContextMenu [Win10] / Group / General ---
[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_GENERAL_GUID,
    Guid = "02FB00B4-60F2-406C-8807-D36FD360D0E9", Order = 0)]
sealed file class ImportMenuItem() : CommandCodeContextMenuItem(CommandCodes.Import)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_GENERAL_GUID,
    Guid = "DB4B5AEF-693C-4269-9979-0C366960D6B5", Order = 1)]
sealed file class RefreshMenuItem() : CommandCodeContextMenuItem(CommandCodes.Refresh)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_GENERAL_GUID,
    Guid = "CC9AA702-5CF5-4A91-8708-AD4E385C38C2", Order = 2)]
sealed file class CopyMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Copy)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_GENERAL_GUID,
    Guid = "9E5F81C7-B6E8-44B5-81DF-1250DC6F5735", Order = 3)]
sealed file class CopyAsPathMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.CopyAsPath)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_GENERAL_GUID,
    Guid = "6B9BDBC3-F973-4664-828E-7B1D3FFE9846", Order = 4)]
sealed file class OpenInFileExplorerMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.OpenInFileExplorer)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_GENERAL_GUID,
    Guid = "C01F6A41-2EAA-4486-AC2E-B49879981DE1", Order = 4)]
sealed file class ShareMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Share)
{

}

#endregion --- MyModsView / ContextMenu [Win10] / Group / General ---


#region --- MyModsView / ContextMenu [Win10] / Group / Folder ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_FOLDER_GUID,
    Guid = "C2576BC6-2565-4E46-89CB-B6FD7B7D93E5", Order = 0)]
sealed file class OpenModFolderMenuItem() : CommandCodeContextMenuItem(CommandCodes.OpenModFolder)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_FOLDER_GUID,
    Guid = "07BF5687-444F-4439-B1A4-93E438789931", Order = 1)]
sealed file class OpenWadFolderMenuItem() : CommandCodeContextMenuItem(CommandCodes.OpenWadFolder)
{

}

#endregion --- MyModsView / ContextMenu [Win10] / Group / Folder ---


#region --- MyModsView / ContextMenu [Win10] / Group / Misc ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_MISC_GUID,
    Guid = "D44337AD-9A58-42F4-8A32-625049D24A31", Order = 0)]
sealed file class ClearAllInstalledMenuItem() : CommandCodeContextMenuItem(CommandCodes.ClearAllInstalled)
{

}

#endregion --- MyModsView / ContextMenu [Win10] / Group / Misc ---


#region --- MyModsView / ContextMenu [Win10] / Group / Delete ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN10_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN10_DELETE_GUID,
    Guid = "EF96892B-2E18-4BE7-A2A6-E701CBD625E9", Order = 0)]
sealed file class DeleteMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Delete)
{
    public override string? KeyboardAcceleratorTextOverride => CommandManager.GetKeyboardAcceleratorTextOverride(CommandCodes.Delete);

}

#endregion --- MyModsView / ContextMenu [Win10] / Group / Delete ---
