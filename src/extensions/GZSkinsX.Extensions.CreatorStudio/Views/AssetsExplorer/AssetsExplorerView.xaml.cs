// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using GZSkinsX.Extensions.CreatorStudio.Models.AssetsExplorer;
using GZSkinsX.Api.Appx;
using GZSkinsX.Api.ContextMenu;
using GZSkinsX.Api.Game;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

using MUXC = Microsoft.UI.Xaml.Controls;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Extensions.CreatorStudio.Views.AssetsExplorer;

public sealed partial class AssetsExplorerView : UserControl
{
    private readonly IGameService _gameService;
    private readonly IContextMenuService _contextMenuService;
    private readonly BackgroundWorker _loadAssetItemsWorker;
    private AssetsExplorerContainer? _rootContainer;

    public AssetsExplorerView()
    {
        _gameService = AppxContext.GameService;
        _contextMenuService = AppxContext.ContextMenuService;

        _loadAssetItemsWorker = new BackgroundWorker();
        _loadAssetItemsWorker.DoWork += Worker_DoWork;
        _loadAssetItemsWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;

        InitializeComponent();
    }

    private void OnMainTreeViewLoaded(object sender, RoutedEventArgs e)
    {
        MainTreeView.Loaded -= OnMainTreeViewLoaded;
        LoadAssetItemsUI();
    }

    private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
    {
        LoadAssetItemsUI();
    }

    private void OnCollapseButtonClick(object sender, RoutedEventArgs e)
    {
        CollapseAllFolders();
    }

    private void CollapseAllFolders()
    {
        foreach (var item in MainTreeView.RootNodes)
        {
            CollapseNode(item);
        }

        static void CollapseNode(MUXC.TreeViewNode treeViewNode)
        {
            if (treeViewNode.HasChildren)
            {
                foreach (var subNode in treeViewNode.Children)
                    CollapseNode(subNode);

                if (treeViewNode.IsExpanded)
                    treeViewNode.IsExpanded = false;
            }
        }
    }

    private async void LoadAssetItemsUI()
    {
        if (LoadingMask.Dispatcher.HasThreadAccess)
        {
            LoadingMask.Visibility = Visibility.Visible;
        }
        else
        {
            await LoadingMask.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.High,
                () => LoadingMask.Visibility = Visibility.Visible);
        }

        _loadAssetItemsWorker.RunWorkerAsync();
    }

    private async void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        await InitializeAssetsExplorerAsync();
    }

    private async void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (LoadingMask.Dispatcher.HasThreadAccess)
        {
            LoadingMask.Visibility = Visibility.Collapsed;
        }
        else
        {
            await LoadingMask.Dispatcher.RunAsync(
            Windows.UI.Core.CoreDispatcherPriority.High,
                () => LoadingMask.Visibility = Visibility.Collapsed);
        }
    }

    private async Task InitializeAssetsExplorerAsync()
    {
        var dataPath = Path.Combine(_gameService.GameData.GameDirectory, "DATA");
        var pluginsPath = Path.Combine(_gameService.GameData.LCUDirectory, "Plugins");

        IEnumerable<FileInfo>? allFiles = null;

        if (Directory.Exists(dataPath))
        {
            allFiles = new DirectoryInfo(dataPath).EnumerateFiles(string.Empty, SearchOption.AllDirectories);
        }

        if (Directory.Exists(pluginsPath))
        {
            var subFiles = new DirectoryInfo(pluginsPath).EnumerateFiles(string.Empty, SearchOption.AllDirectories);
            allFiles = allFiles is not null ? allFiles.Union(subFiles) : subFiles;
        }

        _rootContainer = new AssetsExplorerContainer(new(_gameService.RootDirectory));

        if (allFiles is null)
        {
            return;
        }

        var prefixLength = _gameService.RootDirectory.Length + 1;
        foreach (var item in allFiles)
        {
            var parts = item.FullName[prefixLength..].Split(Path.DirectorySeparatorChar);
            var currentContainer = _rootContainer;

            for (var i = 0; i < parts.Length - 1; i++)
            {
                var childContainer = currentContainer.GetChild<AssetsExplorerContainer>(parts[i]);
                if (childContainer is null)
                {
                    var directory = item.Directory;
                    for (var j = parts.Length - 2; j > i; j--)
                    {
                        directory = directory.Parent;
                    }

                    childContainer = new AssetsExplorerContainer(directory);
                    currentContainer.AddChild(childContainer);
                }
                currentContainer = childContainer;
            }

            currentContainer.AddChild(new AssetsExplorerFile(item));
        }

        if (MainTreeView.Dispatcher.HasThreadAccess)
            LoadNodesFromContainer(_rootContainer);
        else
            await MainTreeView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => LoadNodesFromContainer(_rootContainer));
    }

    private void LoadNodesFromContainer(AssetsExplorerContainer rootConatiner)
    {
        MainTreeView.RootNodes.Clear();
        foreach (var subContainers in rootConatiner.Children.OfType<AssetsExplorerContainer>())
            MainTreeView.RootNodes.Add(CreateItemFromContainer(subContainers));

        foreach (var subFile in rootConatiner.Children.OfType<AssetsExplorerFile>())
            MainTreeView.RootNodes.Add(CreateItemFromFile(subFile));
    }

    private MUXC.TreeViewNode CreateItemFromContainer(AssetsExplorerContainer container)
    {
        var treeViewNode = new MUXC.TreeViewNode { Content = container };
        AutomationProperties.SetName(treeViewNode, container.Name);

        foreach (var subContainers in container.Children.OfType<AssetsExplorerContainer>())
            treeViewNode.Children.Add(CreateItemFromContainer(subContainers));

        foreach (var subFile in container.Children.OfType<AssetsExplorerFile>())
            treeViewNode.Children.Add(CreateItemFromFile(subFile));

        return treeViewNode;
    }

    private MUXC.TreeViewNode CreateItemFromFile(AssetsExplorerFile item)
    {
        var treeViewNode = new MUXC.TreeViewNode() { Content = item };
        AutomationProperties.SetName(treeViewNode, item.Name);
        return treeViewNode;
    }

}
