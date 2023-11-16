// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.ContextMenu;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.MyMods;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace GZSkinsX.Appx.MyMods.Commands;

internal static class CommandManager
{
    public static IconElement? GetIcon(CommandCodes commandCode) => commandCode switch
    {
        CommandCodes.Delete => new SegoeFluentIcon("\uE74D"),
        CommandCodes.Import => new SegoeFluentIcon("\uE710"),
        CommandCodes.Install => new SegoeFluentIcon("\uE896"),
        CommandCodes.Uninstall => new SegoeFluentIcon("\uEA39"),
        CommandCodes.ClearAllInstalled => new SegoeFluentIcon("\uECC9"),
        CommandCodes.Copy => new SegoeFluentIcon("\uE8C8"),
        CommandCodes.CopyAsPath => new SegoeFluentIcon("\uE62F"),
        CommandCodes.OpenInFileExplorer => new SegoeFluentIcon("\uEC50"),
        CommandCodes.OpenModFolder or CommandCodes.OpenWadFolder => new SegoeFluentIcon("\uE8DA"),
        CommandCodes.Refresh => new SegoeFluentIcon("\uE72C"),
        CommandCodes.SelectAll => new SegoeFluentIcon("\uE8B3"),
        CommandCodes.DeselectAll => new SegoeFluentIcon("\uE711"),
        CommandCodes.Share => new SegoeFluentIcon("\uE72D"),
        _ => null,
    };

    public static string? GetHeader(CommandCodes commandCode) => ResourceHelper.GetLocalized(commandCode switch
    {
        CommandCodes.Delete => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Delete",
        CommandCodes.Import => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Import",
        CommandCodes.Install => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Install",
        CommandCodes.Uninstall => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Uninstall",
        CommandCodes.ClearAllInstalled => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_ClearAllInstalled",
        CommandCodes.Copy => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Copy",
        CommandCodes.CopyAsPath => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_CopyAsPath",
        CommandCodes.OpenInFileExplorer => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_OpenInFileExplorer",
        CommandCodes.OpenModFolder => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_OpenModFolder",
        CommandCodes.OpenWadFolder => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_OpenWadFolder",
        CommandCodes.Refresh => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Refresh",
        CommandCodes.SelectAll => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_SelectAll",
        CommandCodes.DeselectAll => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_DeselectAll",
        CommandCodes.Share => "GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Share",
        _ => null,
    });

    public static string? GetHeader2(CommandCodes commandCode) => commandCode switch
    {
        CommandCodes.Import => ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Import2"),
        CommandCodes.Copy => ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_Copy2"),
        CommandCodes.CopyAsPath => ResourceHelper.GetLocalized("GZSkinsX.Appx.MyMods/Resources/MyModsView_Commands_CopyAsPath2"),
        _ => GetHeader(commandCode)
    };

    public static IEnumerable<KeyboardAccelerator> GetKeyboardAccelerators(CommandCodes commandCode)
    {
        return commandCode switch
        {
            CommandCodes.Delete => GetDeleteKeyboardAccelerators(),
            CommandCodes.Import => GetImportKeyboardAccelerators(),
            CommandCodes.Install => GetInstallKeyboardAccelerators(),
            CommandCodes.Uninstall => GetUninstallKeyboardAccelerators(),
            CommandCodes.Copy => GetCopyKeyboardAccelerators(),
            CommandCodes.CopyAsPath => GetCopyAsPathKeyboardAccelerators(),
            CommandCodes.OpenInFileExplorer => GetOpenInFileExplorerKeyboardAccelerators(),
            CommandCodes.Refresh => GetRefreshKeyboardAccelerators(),
            CommandCodes.SelectAll => GetSelectAllKeyboardAccelerators(),
            CommandCodes.DeselectAll => GetDeselectAllKeyboardAccelerators(),
            CommandCodes.Share => GetShareKeyboardAccelerators(),
            _ => GetNoneKeyboardAccelerators(),
        };

        static IEnumerable<KeyboardAccelerator> GetNoneKeyboardAccelerators()
        {
            yield break;
        }

        static IEnumerable<KeyboardAccelerator> GetDeleteKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.Delete };
        }

        static IEnumerable<KeyboardAccelerator> GetImportKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.O, Modifiers = VirtualKeyModifiers.Control };
        }

        static IEnumerable<KeyboardAccelerator> GetInstallKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.E, Modifiers = VirtualKeyModifiers.Control };
        }

        static IEnumerable<KeyboardAccelerator> GetUninstallKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.E, Modifiers = VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift };
        }

        static IEnumerable<KeyboardAccelerator> GetCopyKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.C, Modifiers = VirtualKeyModifiers.Control };
        }

        static IEnumerable<KeyboardAccelerator> GetCopyAsPathKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.C, Modifiers = VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift };
        }

        static IEnumerable<KeyboardAccelerator> GetOpenInFileExplorerKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.F3 };
        }

        static IEnumerable<KeyboardAccelerator> GetRefreshKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.F5 };
        }

        static IEnumerable<KeyboardAccelerator> GetSelectAllKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.A, Modifiers = VirtualKeyModifiers.Control };
        }

        static IEnumerable<KeyboardAccelerator> GetDeselectAllKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.D, Modifiers = VirtualKeyModifiers.Control };
            yield return new KeyboardAccelerator { Key = VirtualKey.Escape };
        }

        static IEnumerable<KeyboardAccelerator> GetShareKeyboardAccelerators()
        {
            yield return new KeyboardAccelerator { Key = VirtualKey.S, Modifiers = VirtualKeyModifiers.Menu };
        }
    }

    public static string? GetKeyboardAcceleratorTextOverride(CommandCodes commandCode) => commandCode switch
    {
        CommandCodes.Delete => "Del",
        CommandCodes.DeselectAll => "Esc, Ctrl+D",
        _ => null
    };

    public static bool CanExecute(CommandCodes commandCode, IMyModsView myModsView) => commandCode switch
    {
        CommandCodes.Delete or
        CommandCodes.Install or
        CommandCodes.Uninstall or
        CommandCodes.Copy or
        CommandCodes.CopyAsPath or
        CommandCodes.OpenInFileExplorer or
        CommandCodes.Share => myModsView?.SelectedItem is not null,

        CommandCodes.Import or
        CommandCodes.ClearAllInstalled or
        CommandCodes.OpenModFolder or
        CommandCodes.OpenWadFolder or
        CommandCodes.Refresh or
        CommandCodes.SelectAll or
        CommandCodes.DeselectAll => myModsView is not null,

        _ => false
    };

    public static void Execute(CommandCodes commandCode, IMyModsView myModsView)
    {
        if (CanExecute(commandCode, myModsView) is false)
        {
            return;
        }

        try
        {
            switch (commandCode)
            {
                case CommandCodes.Delete:
                    OnDelete(myModsView);
                    break;

                case CommandCodes.Import:
                    OnImport(myModsView);
                    break;

                case CommandCodes.Install:
                    OnInstall(myModsView);
                    break;

                case CommandCodes.Uninstall:
                    OnUninstall(myModsView);
                    break;

                case CommandCodes.ClearAllInstalled:
                    OnClearAllInstalled(myModsView);
                    break;

                case CommandCodes.Copy:
                    OnCopy(myModsView);
                    break;

                case CommandCodes.CopyAsPath:
                    OnCopyAsPath(myModsView);
                    break;

                case CommandCodes.OpenInFileExplorer:
                    OnOpenInFileExplorer(myModsView);
                    break;

                case CommandCodes.OpenModFolder:
                    OnOpenModFolder(myModsView);
                    break;

                case CommandCodes.OpenWadFolder:
                    OnOpenWadFolder(myModsView);
                    break;

                case CommandCodes.Refresh:
                    OnRefresh(myModsView);
                    break;

                case CommandCodes.SelectAll:
                    OnSelectAll(myModsView);
                    break;

                case CommandCodes.DeselectAll:
                    OnDeselectAll(myModsView);
                    break;

                case CommandCodes.Share:
                    OnShare(myModsView);
                    break;

                default:
                    break;
            }
        }
        catch (Exception excp)
        {
            excp.ShowErrorDialogAsync().FireAndForget();
        }
    }

    private static async void OnDelete(IMyModsView myModsView)
    {
        await myModsView.DeleteAsync(myModsView.SelectedItems);
    }

    private static async void OnImport(IMyModsView myModsView)
    {
        var filePicker = new FileOpenPicker();
        filePicker.FileTypeFilter.Add(".lolgezi");

        WinRT.Interop.InitializeWithWindow.Initialize(
            filePicker, AppxContext.AppxWindow.MainWindowHandle);

        await myModsView.ImportAsync(await filePicker.PickMultipleFilesAsync());
    }

    private static async void OnInstall(IMyModsView myModsView)
    {
        await myModsView.InstallAsync(myModsView.SelectedItems);
    }

    private static async void OnUninstall(IMyModsView myModsView)
    {
        await myModsView.UninstallAsync(myModsView.SelectedItems);
    }

    private static async void OnClearAllInstalled(IMyModsView myModsView)
    {
        await AppxContext.MyModsService.ClearAllInstalledAsync();
        await myModsView.RefreshAsync();
    }

    private static async void OnCopy(IMyModsView myModsView)
    {
        var selectedItems = myModsView.SelectedItems;
        if (selectedItems.Any() is false)
        {
            return;
        }

        var existsFiles = from item in myModsView.SelectedItems
                          let fullName = item.ModInfo.FileInfo.FullName
                          where File.Exists(fullName)
                          select fullName;

        var storageItems = new List<StorageFile>();
        foreach (var file in existsFiles)
        {
            storageItems.Add(await StorageFile.GetFileFromPathAsync(file));
        }

        var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
        dataPackage.SetStorageItems(storageItems);
        Clipboard.SetContent(dataPackage);
    }

    private static void OnCopyAsPath(IMyModsView myModsView)
    {
        var selectedItems = myModsView.SelectedItems.ToArray();
        if (selectedItems.Length is 0)
        {
            return;
        }

        var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
        if (selectedItems.Length is 1)
        {
            dataPackage.SetText($"\"{selectedItems[0].ModInfo.FileInfo.FullName}\"");
        }
        else
        {

            var index = 0;
            var stringBuilder = new StringBuilder();

            do
            {
                stringBuilder.Append('\"');
                stringBuilder.Append(selectedItems[index].ModInfo.FileInfo.FullName);
                stringBuilder.Append('\"');

                if (++index != selectedItems.Length)
                {
                    stringBuilder.Append(Environment.NewLine);
                }

            } while (index < selectedItems.Length);

            dataPackage.SetText(stringBuilder.ToString());
        }

        Clipboard.SetContent(dataPackage);
    }

    private static async void OnOpenInFileExplorer(IMyModsView myModsView)
    {
        var selectedItems = myModsView.SelectedItems;
        if (selectedItems.Any() is false)
        {
            return;
        }

        var existsFiles = from item in myModsView.SelectedItems
                          let fullName = item.ModInfo.FileInfo.FullName
                          where File.Exists(fullName)
                          select fullName;

        if (existsFiles.Any() is false)
        {
            return;
        }

        var modFolder = await AppxContext.MyModsService.GetModFolderAsync();
        if (Directory.Exists(modFolder) is false)
        {
            return;
        }

        var storageFolder = await StorageFolder.GetFolderFromPathAsync(modFolder);
        if (storageFolder is null)
        {
            return;
        }

        var options = new FolderLauncherOptions();
        foreach (var file in existsFiles)
        {
            options.ItemsToSelect.Add(await StorageFile.GetFileFromPathAsync(file));
        }

        await Launcher.LaunchFolderAsync(storageFolder, options);
    }

    private static async void OnOpenModFolder(IMyModsView myModsView)
    {
        var modFolder = await AppxContext.MyModsService.GetModFolderAsync();
        if (Directory.Exists(modFolder) is false)
        {
            return;
        }

        var storageFolder = await StorageFolder.GetFolderFromPathAsync(modFolder);
        if (storageFolder is null)
        {
            return;
        }

        await Launcher.LaunchFolderAsync(storageFolder);
    }

    private static async void OnOpenWadFolder(IMyModsView myModsView)
    {
        var wadFolder = await AppxContext.MyModsService.GetWadFolderAsync();
        if (Directory.Exists(wadFolder) is false)
        {
            return;
        }

        var storageFolder = await StorageFolder.GetFolderFromPathAsync(wadFolder);
        if (storageFolder is null)
        {
            return;
        }

        await Launcher.LaunchFolderAsync(storageFolder);
    }

    private static async void OnRefresh(IMyModsView myModsView)
    {
        await myModsView.RefreshAsync();
    }

    private static void OnSelectAll(IMyModsView myModsView)
    {
        myModsView.SelectAll();
    }

    private static void OnDeselectAll(IMyModsView myModsView)
    {
        myModsView.DeseleteAll();
    }

    private static void OnShare(IMyModsView myModsView)
    {
        myModsView.ShowShareUI();
    }
}

internal enum CommandCodes
{
    None,
    Delete,
    Import,
    Install,
    Uninstall,
    ClearAllInstalled,
    Copy,
    CopyAsPath,
    OpenInFileExplorer,
    OpenModFolder,
    OpenWadFolder,
    Refresh,
    SelectAll,
    DeselectAll,
    Share,
}

internal class CommandCodeContextMenuItem(CommandCodes commandCode) : ContextMenuItemBase<MyModsViewContextMenuUIContext>
{
    public override string? Header => CommandManager.GetHeader(commandCode);

    public override IconElement? Icon => CommandManager.GetIcon(commandCode);

    public override IEnumerable<KeyboardAccelerator> KeyboardAccelerators => CommandManager.GetKeyboardAccelerators(commandCode);

    public override void OnExecute(MyModsViewContextMenuUIContext context) => CommandManager.Execute(commandCode, context.MyModsView);
}

internal class CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNotNull(CommandCodes commandCode) : CommandCodeContextMenuItem(commandCode)
{
    public override bool IsVisible(MyModsViewContextMenuUIContext context) => context.MyModsView.SelectedItem is not null;
}

internal class CommandCodeContextMenuItem_VisibleWhenSelectedItemIsNull(CommandCodes commandCode) : CommandCodeContextMenuItem(commandCode)
{
    public override bool IsVisible(MyModsViewContextMenuUIContext context) => context.MyModsView.SelectedItem is null;
}
