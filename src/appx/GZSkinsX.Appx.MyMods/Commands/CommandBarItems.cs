// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Linq;

using GZSkinsX.Contracts.Command;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;

namespace GZSkinsX.Appx.MyMods.Commands;

[Shared, CommandBarItemContract(
    OwnerGuid = CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, Order = 0,
    Group = CommandConstants.GROUP_MYMODSVIEW_COMMANDBAR_PRIMARY_DESELECTALL2_GUID)]
sealed file class DeselectAll2Command : CommandBarButtonBase<MyModsViewCommandBarUIContext>
{
    private readonly string _formatString = ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_DeselectAll2_Count");

    public override void OnInitialize(MyModsViewCommandBarUIContext ctx)
    {
        Icon = CommandManager.GetIcon(CommandCodes.DeselectAll);
        DisplayName = string.Format(_formatString, ctx.MyModsView.SelectedItems.Count());
        KeyboardAccelerators = CommandManager.GetKeyboardAccelerator(CommandCodes.DeselectAll);
        IsVisible = ctx.MyModsView.SelectedItem is not null;
    }

    public override void OnLoaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged += OnMyModsViewSelectionChanged;
    }

    public override void OnUnloaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged -= OnMyModsViewSelectionChanged;
    }

    private void OnMyModsViewSelectionChanged(object? sender, MyModsViewSelectionChangedArgs e)
    {
        if (sender is IMyModsView { SelectedItem: not null } myModsView)
        {
            DisplayName = string.Format(_formatString, myModsView.SelectedItems.Count());
            IsVisible = true;
        }
        else
        {
            DisplayName = string.Empty;
            IsVisible = false;
        }
    }

    public override void OnClick(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.DeseleteAll();
    }
}

[Shared, CommandBarItemContract(
    OwnerGuid = CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, Order = 0,
    Group = CommandConstants.GROUP_MYMODSVIEW_COMMANDBAR_PRIMARY_GENERAL_GUID)]
sealed file class InstallCommand : CommandBarButtonBase<MyModsViewCommandBarUIContext>
{
    public override void OnInitialize(MyModsViewCommandBarUIContext ctx)
    {
        Icon = CommandManager.GetIcon(CommandCodes.Install);
        DisplayName = CommandManager.GetHeader(CommandCodes.Install);
        KeyboardAccelerators = CommandManager.GetKeyboardAccelerator(CommandCodes.Install);
        IsVisible = ctx.MyModsView.SelectedItem is not null;
    }

    public override void OnLoaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged += OnMyModsViewSelectionChanged;
    }

    public override void OnUnloaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged -= OnMyModsViewSelectionChanged;
    }

    private void OnMyModsViewSelectionChanged(object? sender, MyModsViewSelectionChangedArgs e)
    {
        IsVisible = sender is IMyModsView { SelectedItem: not null };
    }

    public override void OnClick(MyModsViewCommandBarUIContext ctx)
    {
        CommandManager.Execute(CommandCodes.Install, ctx.MyModsView);
    }
}

[Shared, CommandBarItemContract(
    OwnerGuid = CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, Order = 1,
    Group = CommandConstants.GROUP_MYMODSVIEW_COMMANDBAR_PRIMARY_GENERAL_GUID)]
sealed file class UninstallCommand : CommandBarButtonBase<MyModsViewCommandBarUIContext>
{
    public override void OnInitialize(MyModsViewCommandBarUIContext ctx)
    {
        Icon = CommandManager.GetIcon(CommandCodes.Uninstall);
        DisplayName = CommandManager.GetHeader(CommandCodes.Uninstall);
        KeyboardAccelerators = CommandManager.GetKeyboardAccelerator(CommandCodes.Uninstall);
        IsVisible = ctx.MyModsView.SelectedItem is not null;
    }

    public override void OnLoaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged += OnMyModsViewSelectionChanged;
    }

    public override void OnUnloaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged -= OnMyModsViewSelectionChanged;
    }

    private void OnMyModsViewSelectionChanged(object? sender, MyModsViewSelectionChangedArgs e)
    {
        IsVisible = sender is IMyModsView { SelectedItem: not null };
    }

    public override void OnClick(MyModsViewCommandBarUIContext ctx)
    {
        CommandManager.Execute(CommandCodes.Uninstall, ctx.MyModsView);
    }
}

[Shared, CommandBarItemContract(
    OwnerGuid = CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, Order = 2,
    Group = CommandConstants.GROUP_MYMODSVIEW_COMMANDBAR_PRIMARY_GENERAL_GUID)]
sealed file class ImportCommand : CommandBarButtonBase<MyModsViewCommandBarUIContext>
{
    public override void OnInitialize(MyModsViewCommandBarUIContext ctx)
    {
        Icon = CommandManager.GetIcon(CommandCodes.Import);
        DisplayName = CommandManager.GetHeader(CommandCodes.Import);
        KeyboardAccelerators = CommandManager.GetKeyboardAccelerator(CommandCodes.Import);
        IsVisible = ctx.MyModsView.SelectedItem is not null;
    }

    public override void OnLoaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged += OnMyModsViewSelectionChanged;
    }

    public override void OnUnloaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged -= OnMyModsViewSelectionChanged;
    }

    private void OnMyModsViewSelectionChanged(object? sender, MyModsViewSelectionChangedArgs e)
    {
        IsVisible = sender is IMyModsView { SelectedItem: not null };
    }

    public override void OnClick(MyModsViewCommandBarUIContext ctx)
    {
        CommandManager.Execute(CommandCodes.Import, ctx.MyModsView);
    }
}

[Shared, CommandBarItemContract(
    OwnerGuid = CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, Order = 3,
    Group = CommandConstants.GROUP_MYMODSVIEW_COMMANDBAR_PRIMARY_GENERAL_GUID)]
sealed file class DeleteCommand : CommandBarButtonBase<MyModsViewCommandBarUIContext>
{
    public override void OnInitialize(MyModsViewCommandBarUIContext ctx)
    {
        Icon = CommandManager.GetIcon(CommandCodes.Delete);
        DisplayName = CommandManager.GetHeader(CommandCodes.Delete);
        KeyboardAccelerators = CommandManager.GetKeyboardAccelerator(CommandCodes.Delete);
        KeyboardAcceleratorTextOverride = CommandManager.GetKeyboardAcceleratorTextOverride(CommandCodes.Delete);
        IsVisible = ctx.MyModsView.SelectedItem is not null;
    }

    public override void OnLoaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged += OnMyModsViewSelectionChanged;
    }

    public override void OnUnloaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged -= OnMyModsViewSelectionChanged;
    }

    private void OnMyModsViewSelectionChanged(object? sender, MyModsViewSelectionChangedArgs e)
    {
        IsVisible = sender is IMyModsView { SelectedItem: not null };
    }

    public override void OnClick(MyModsViewCommandBarUIContext ctx)
    {
        CommandManager.Execute(CommandCodes.Delete, ctx.MyModsView);
    }
}

[Shared, CommandBarItemContract(
    OwnerGuid = CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, Order = 0,
    Group = CommandConstants.GROUP_MYMODSVIEW_COMMANDBAR_PRIMARY_GENERAL_GUID)]
sealed file class Import2Command : CommandBarButtonBase<MyModsViewCommandBarUIContext>
{
    public override void OnInitialize(MyModsViewCommandBarUIContext ctx)
    {
        Icon = CommandManager.GetIcon(CommandCodes.Import);
        DisplayName = CommandManager.GetHeader2(CommandCodes.Import);
        KeyboardAccelerators = CommandManager.GetKeyboardAccelerator(CommandCodes.Import);
        IsVisible = ctx.MyModsView.SelectedItem is null;
    }

    public override void OnLoaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged += OnMyModsViewSelectionChanged;
    }

    public override void OnUnloaded(MyModsViewCommandBarUIContext ctx)
    {
        ctx.MyModsView.SelectionChanged -= OnMyModsViewSelectionChanged;
    }

    private void OnMyModsViewSelectionChanged(object? sender, MyModsViewSelectionChangedArgs e)
    {
        IsVisible = sender is IMyModsView { SelectedItem: null };
    }

    public override void OnClick(MyModsViewCommandBarUIContext ctx)
    {
        CommandManager.Execute(CommandCodes.Import, ctx.MyModsView);
    }
}


[Shared, CommandBarItemContract(
    OwnerGuid = CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, Order = 0,
    Group = CommandConstants.GROUP_MYMODSVIEW_COMMANDBAR_SECONDARY_GENERAL_GUID,
    Placement = CommandBarItemPlacement.Secondary)]
sealed file class SelectAllCommand : CommandBarButtonBase<MyModsViewCommandBarUIContext>
{
    public override void OnInitialize(MyModsViewCommandBarUIContext ctx)
    {
        Icon = CommandManager.GetIcon(CommandCodes.SelectAll);
        DisplayName = CommandManager.GetHeader(CommandCodes.SelectAll);
        KeyboardAccelerators = CommandManager.GetKeyboardAccelerator(CommandCodes.SelectAll);
    }

    public override void OnClick(MyModsViewCommandBarUIContext ctx)
    {
        CommandManager.Execute(CommandCodes.SelectAll, ctx.MyModsView);
    }
}

[Shared, CommandBarItemContract(
    OwnerGuid = CommandConstants.MYMODSVIEW_COMMANDBAR_GUID, Order = 1,
    Group = CommandConstants.GROUP_MYMODSVIEW_COMMANDBAR_SECONDARY_GENERAL_GUID,
    Placement = CommandBarItemPlacement.Secondary)]
sealed file class DeselectAllCommand : CommandBarButtonBase<MyModsViewCommandBarUIContext>
{
    public override void OnInitialize(MyModsViewCommandBarUIContext ctx)
    {
        Icon = CommandManager.GetIcon(CommandCodes.DeselectAll);
        DisplayName = CommandManager.GetHeader(CommandCodes.DeselectAll);
        KeyboardAccelerators = CommandManager.GetKeyboardAccelerator(CommandCodes.DeselectAll);
        KeyboardAcceleratorTextOverride = CommandManager.GetKeyboardAcceleratorTextOverride(CommandCodes.DeselectAll);
    }

    public override void OnClick(MyModsViewCommandBarUIContext ctx)
    {
        CommandManager.Execute(CommandCodes.DeselectAll, ctx.MyModsView);
    }
}
