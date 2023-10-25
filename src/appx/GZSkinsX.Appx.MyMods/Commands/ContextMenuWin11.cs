// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Contracts.ContextMenu;

namespace GZSkinsX.Appx.MyMods.Commands;

#region --- MyModsView / ContextMenu [Win11] / Primary / Group / Default ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_PRIMARY_DEFAULT_GUID,
    Guid = "DCA7E07D-37CD-4E28-A90A-16775ECA8658", Order = 0)]
sealed file class ImportMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNull(CommandCodes.Import)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_PRIMARY_DEFAULT_GUID,
    Guid = "BC87AC7A-9C10-400A-85E7-E938EB8CBB9D", Order = 1)]
sealed file class RefreshMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNull(CommandCodes.Refresh)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_PRIMARY_DEFAULT_GUID,
    Guid = "A77B58DF-78F3-4399-8045-464C859AFE60", Order = 0)]
sealed file class InstallMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Install)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_PRIMARY_DEFAULT_GUID,
    Guid = "8DDB9FF7-C647-40C9-985D-96F4077A3755", Order = 1)]
sealed file class UninstallMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Uninstall)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_PRIMARY_DEFAULT_GUID,
    Guid = "AD5B9091-8D47-4A2D-9A2B-05C1DDB64342", Order = 2)]
sealed file class CopyMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Copy)
{
    public override string? Header => CommandManager.GetHeader2(CommandCodes.Copy);
}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_PRIMARY_DEFAULT_GUID,
    Guid = "38DE9F39-D45F-4A2A-BE69-2E944CCC7671", Order = 3)]
sealed file class ShareMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Share)
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_PRIMARY_DEFAULT_GUID,
    Guid = "33CDBEDA-4B10-4738-BEDF-1A07E5E133FB", Order = 4)]
sealed file class DeleteMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Delete)
{
    public override string? KeyboardAcceleratorTextOverride => CommandManager.GetKeyboardAcceleratorTextOverride(CommandCodes.Delete);
}

#endregion --- MyModsView / ContextMenu [Win11] / Primary / Group / Default ---


#region --- MyModsView / ContextMenu [Win11] / Secondary / Group / Default ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_SECONDARY_DEFAULT_GUID,
    Guid = "0F864147-E0B0-4591-B8E8-E4D98A6BF194", Order = 0)]
sealed file class Import2MenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Import), ISecondaryCommandBarItem
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_SECONDARY_DEFAULT_GUID,
    Guid = "523A4DA2-671B-4C3B-B608-22D12313A6D8", Order = 1)]
sealed file class Refresh2MenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.Refresh), ISecondaryCommandBarItem
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_SECONDARY_DEFAULT_GUID,
    Guid = "F8342153-B5B7-4C87-AB6A-BC768D231C76", Order = 2)]
sealed file class OpenInFileExplorerMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.OpenInFileExplorer), ISecondaryCommandBarItem
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_SECONDARY_DEFAULT_GUID,
    Guid = "74D5BB62-FD98-461C-B0B6-D8E0B866D53D", Order = 3)]
sealed file class CopyAsPathMenuItem() : CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes.CopyAsPath), ISecondaryCommandBarItem
{
    public override string? Header => CommandManager.GetHeader2(CommandCodes.CopyAsPath);
}

#endregion --- MyModsView / ContextMenu [Win11] / Secondary / Group / Default ---


#region --- MyModsView / ContextMenu [Win11] / Secondary / Group / Folder ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_SECONDARY_FOLDER_GUID,
    Guid = "E4B47115-47EA-46C3-93D5-F8107FF5C785", Order = 0)]
sealed file class OpenModFolderMenuItem() : CommandCodeContextMenuItem(CommandCodes.OpenModFolder), ISecondaryCommandBarItem
{

}

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_SECONDARY_FOLDER_GUID,
    Guid = "DA13B8CF-2A60-4925-A81C-EAF24329211B", Order = 1)]
sealed file class OpenWadFolderMenuItem() : CommandCodeContextMenuItem(CommandCodes.OpenWadFolder), ISecondaryCommandBarItem
{

}
#endregion --- MyModsView / ContextMenu [Win11] / Secondary / Group / Folder ---


#region --- MyModsView / ContextMenu [Win11] / Secondary / Group / Misc ---

[Shared, ContextMenuItemContract(
    OwnerGuid = ContextMenuConstants.MYMODSVIEW_CTX_WIN11_GUID,
    Group = ContextMenuConstants.GROUP_MYMODSVIEW_CTX_WIN11_SECONDARY_MISC_GUID,
    Guid = "2E5C4696-B5F0-4338-8C94-D6D8953DB6C5", Order = 0)]
sealed file class ClearAllInstalledMenuItem() : CommandCodeContextMenuItem(CommandCodes.ClearAllInstalled), ISecondaryCommandBarItem
{

}

#endregion --- MyModsView / ContextMenu [Win11] / Secondary / Group / Misc ---
