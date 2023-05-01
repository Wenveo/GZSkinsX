// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.ObjectModel;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Game;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.ViewModels;

internal sealed partial class ExplorerItem : ObservableObject
{
    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private ExplorerItemType _type;

    private ObservableCollection<ExplorerItem>? _childrens;

    public ObservableCollection<ExplorerItem> Childrens
    {
        get => _childrens ??= new ObservableCollection<ExplorerItem>();
    }
}

internal enum ExplorerItemType
{
    File,
    Folder
}

internal sealed class ExplorerItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? FolderTemplate { get; set; }

    public DataTemplate? FileTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        return (item is ExplorerItem explorerItem) ? (explorerItem.Type is ExplorerItemType.Folder ? FolderTemplate : FileTemplate) : null;
    }
}

internal sealed partial class ShellViewModel : ObservableObject
{
    private readonly IGameService _gameService;

    [ObservableProperty]
    private ObservableCollection<ExplorerItem> _explorerItems;

    [ObservableProperty]
    private ExplorerItem? _selectedItem;

    public ShellViewModel()
    {
        _gameService = AppxContext.ServiceLocator.Resolve<IGameService>();
        _explorerItems = new ObservableCollection<ExplorerItem>();

        Initialize();
    }

    partial void OnSelectedItemChanged(ExplorerItem? value)
    {
    }

    private void Initialize()
    {
        var game_Data_Directory = new DirectoryInfo(Path.Combine(_gameService.GameData.GameDirectory, "DATA"));
        if (game_Data_Directory.Exists)
        {
            var gameNode = new ExplorerItem { Name = Path.GetFileName(_gameService.GameData.GameDirectory), Type = ExplorerItemType.Folder };
            var dataNode = new ExplorerItem { Name = game_Data_Directory.Name, Type = ExplorerItemType.Folder };

            var container = dataNode.Childrens;

            foreach (var subFolderInfo in game_Data_Directory.EnumerateDirectories())
                container.Add(CreateExplorerFolder(subFolderInfo));

            foreach (var subFileInfo in game_Data_Directory.EnumerateFiles())
                container.Add(CreateExplorerFile(subFileInfo));

            gameNode.Childrens.Add(dataNode);
            ExplorerItems.Add(gameNode);
        }

        var client_Plugins_Directory = new DirectoryInfo(Path.Combine(_gameService.GameData.LCUDirectory, "Plugins"));
        if (client_Plugins_Directory.Exists)
        {
            var clientNode = new ExplorerItem { Name = Path.GetFileName(_gameService.GameData.LCUDirectory), Type = ExplorerItemType.Folder };
            var pluginsNode = new ExplorerItem { Name = client_Plugins_Directory.Name, Type = ExplorerItemType.Folder };

            foreach (var subFolderInfo in client_Plugins_Directory.EnumerateDirectories())
                pluginsNode.Childrens.Add(CreateExplorerFolder(subFolderInfo));

            foreach (var subFileInfo in client_Plugins_Directory.EnumerateFiles())
                pluginsNode.Childrens.Add(CreateExplorerFile(subFileInfo));

            clientNode.Childrens.Add(pluginsNode);
            ExplorerItems.Add(clientNode);
        }
    }

    private ExplorerItem CreateExplorerFolder(DirectoryInfo directoryInfo)
    {
        var explorerFolder = new ExplorerItem
        {
            Name = directoryInfo.Name,
            Type = ExplorerItemType.Folder
        };

        foreach (var subFolderInfo in directoryInfo.EnumerateDirectories())
        {
            explorerFolder.Childrens.Add(CreateExplorerFolder(subFolderInfo));
        }

        foreach (var subFileInfo in directoryInfo.EnumerateFiles())
        {
            explorerFolder.Childrens.Add(CreateExplorerFile(subFileInfo));
        }

        return explorerFolder;
    }

    private ExplorerItem CreateExplorerFile(FileInfo fileInfo)
    {
        return new ExplorerItem
        {
            Name = fileInfo.Name
        };
    }
}
