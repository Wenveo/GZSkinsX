// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.ComponentModel;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using GZSkinsX.Api.AccessCache;
using GZSkinsX.Api.Appx;
using GZSkinsX.Api.ContextMenu;
using GZSkinsX.Api.Controls;
using GZSkinsX.Api.CreatorStudio.AssetsExplorer;
using GZSkinsX.Api.Game;
using GZSkinsX.Api.MRT;
using GZSkinsX.Api.Themes;

using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.AssetsExplorer;

[Shared, Export(typeof(IAssetsExplorerService))]
internal sealed class AssetsExplorerService : IAssetsExplorerService
{
    private static readonly SolidColorBrush s_loading_dark_background = new(Color.FromArgb(0x0D, 0xFF, 0xFF, 0xFF));
    private static readonly SolidColorBrush s_loading_light_background = new(Color.FromArgb(0x72, 0xF3, 0xF3, 0xF3));

    private readonly IFutureAccessService _futureAccessService;
    private readonly IMRTCoreService _mrtCoreService;
    private readonly IGameService _gameService;
    private readonly IContextMenuService _contextMenuService;

    private readonly BackgroundWorker _loadAssetItemsWorker;
    private readonly MUXC.TreeView _treeView;
    private readonly Button _refreshButton;
    private readonly Button _collapseButton;
    private readonly Border _loading;
    private readonly Grid _rootGrid;

    private AssetsExplorerContainer? _rootContainer;

    internal FrameworkElement UIObject => _rootGrid;

    public IAssetsExplorerItem? SelectedItem => (IAssetsExplorerItem?)_treeView.SelectedNode?.Content;

    public event EventHandler<AssetsExplorerItemInvokedEventArgs>? ItemInvoked;

    public AssetsExplorerService()
    {
        _mrtCoreService = AppxContext.MRTCoreService;
        _futureAccessService = AppxContext.FutureAccessService;
        _gameService = AppxContext.ServiceLocator.Resolve<IGameService>();
        _contextMenuService = AppxContext.ServiceLocator.Resolve<IContextMenuService>();

        _loading = new();
        _rootGrid = new();
        _treeView = new();
        _refreshButton = new();
        _collapseButton = new();
        _loadAssetItemsWorker = new();

        InitializeUIObject();
        InitializeEvents();
    }

    private void InitializeUIObject()
    {
        _treeView.ContextFlyout =
            _contextMenuService.CreateContextFlyout(MenuItemConstants.TREEVIEW_GUID, null,
            (sender, e) =>
            {
                var menuFlyout = (MenuFlyout)sender;
                return new ContextMenuUIContext(menuFlyout.Target, "123456");
            });

        _treeView.Padding = new Thickness(0, 0, 12, 0);
        _treeView.ItemContainerTransitions = new TransitionCollection
        {
            new PaneThemeTransition { Edge = EdgeTransitionLocation.Top }
        };
        _treeView.Resources = new TreeViewNodeItemTemplate();
        _treeView.ItemTemplate = (DataTemplate)_treeView.Resources["AssetsExplorerItem_ItemTemplate"];

        _refreshButton.Padding = new Thickness(6);
        _refreshButton.Content = new Viewbox
        {
            Width = 12d,
            Height = 12d,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = new SegoeFluentIcon { Glyph = "\uE72C" }
        };

        _collapseButton.Padding = new Thickness(6);
        _collapseButton.Content = new Viewbox
        {
            Width = 12d,
            Height = 12d,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = new SegoeFluentIcon { Glyph = "\uF165" }
        };

        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4d };
        stackPanel.Children.Add(_refreshButton);
        stackPanel.Children.Add(_collapseButton);

        var titleBlock = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = FontWeights.SemiBold,
            Text = "Assets Explorer"
        };

        var topArea = new Grid { Margin = new Thickness(10, 10, 10, 0) };
        topArea.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        topArea.ColumnDefinitions.Add(new ColumnDefinition { });
        topArea.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        Grid.SetColumn(stackPanel, 2);
        Grid.SetColumn(titleBlock, 0);

        topArea.Children.Add(titleBlock);
        topArea.Children.Add(stackPanel);

        _loading.Visibility = Visibility.Collapsed;
        _loading.Background = GetLoadingBackground(AppxContext.ThemeService.ActualTheme);
        _loading.Child = new MUXC.ProgressRing
        {
            IsIndeterminate = true,
            Height = 32d,
            Width = 32d
        };

        _rootGrid.RowSpacing = 12d;
        _rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        _rootGrid.RowDefinitions.Add(new RowDefinition { });

        Grid.SetRow(topArea, 0);
        Grid.SetRow(_treeView, 1);
        Grid.SetRowSpan(_loading, 2);

        _rootGrid.Children.Add(topArea);
        _rootGrid.Children.Add(_treeView);
        _rootGrid.Children.Add(_loading);
    }

    private void InitializeEvents()
    {
        _treeView.Loaded += OnTreeViewLoaded;
        _treeView.ItemInvoked += OnTreeViewItemInvoked;
        _refreshButton.Click += OnRefreshButtonClick;
        _collapseButton.Click += OnCollapseButtonClick;

        _loadAssetItemsWorker.DoWork += Worker_DoWork;
        _loadAssetItemsWorker.RunWorkerCompleted += Worker_RunWorkerCompleted;

        AppxContext.ThemeService.ThemeChanged += OnThemeChanged;
    }

    private void OnTreeViewLoaded(object sender, RoutedEventArgs e)
    {
        _treeView.Loaded -= OnTreeViewLoaded;
        LoadAssetItemsUI();
    }

    private void OnTreeViewItemInvoked(MUXC.TreeView sender, MUXC.TreeViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is MUXC.TreeViewNode node)
        {
            ItemInvoked?.Invoke(this, new AssetsExplorerItemInvokedEventArgs((IAssetsExplorerItem)node.Content));
        }
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
        foreach (var item in _treeView.RootNodes)
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

    private void OnThemeChanged(object sender, ThemeChangedEventArgs args)
    {
        _loading.Background = GetLoadingBackground(args.ActualTheme);
    }

    private async void LoadAssetItemsUI()
    {
        if (_loading.Dispatcher.HasThreadAccess)
        {
            _loading.Visibility = Visibility.Visible;
        }
        else
        {
            await _loading.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.High,
                () => _loading.Visibility = Visibility.Visible);
        }

        _loadAssetItemsWorker.RunWorkerAsync();
    }

    private async void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        await InitializeAssetsExplorerAsync();
    }

    private async void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (_loading.Dispatcher.HasThreadAccess)
        {
            _loading.Visibility = Visibility.Collapsed;
        }
        else
        {
            await _loading.Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.High,
                () => _loading.Visibility = Visibility.Collapsed);
        }
    }

    private async Task InitializeAssetsExplorerAsync()
    {
        var rootDirectory = new DirectoryInfo(_gameService.RootDirectory);

        var dataFolder = Path.Combine(_gameService.GameData.GameDirectory, "DATA").ToLower();
        var pluginsFolder = Path.Combine(_gameService.GameData.LCUDirectory, "Plugins").ToLower();

        var allFiles = rootDirectory.EnumerateFiles("*", SearchOption.AllDirectories)
            .Where(file =>
            {
                var fullPath = file.FullName.ToLower();
                return fullPath.Contains(dataFolder) || fullPath.Contains(pluginsFolder);
            })
            .OrderBy(file => file.FullName);

        var filteredFiles = allFiles.Where(a => IsSupportedFileType(a.Extension));
        var rootPathLength = rootDirectory.FullName.Length + 1; // + DirectorySeparatorChar

        _rootContainer = new AssetsExplorerContainer(rootDirectory);
        foreach (var item in allFiles)
        {
            var parts = item.FullName[rootPathLength..].Split(Path.DirectorySeparatorChar);
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

        void LoadNodesFromContainer(AssetsExplorerContainer rootConatiner)
        {
            _treeView.RootNodes.Clear();
            foreach (var subContainers in rootConatiner.Children.OfType<AssetsExplorerContainer>())
                _treeView.RootNodes.Add(CreateItemFromContainer(subContainers));

            foreach (var subFile in rootConatiner.Children.OfType<AssetsExplorerFile>())
                _treeView.RootNodes.Add(CreateItemFromFile(subFile));
        }

        if (_treeView.Dispatcher.HasThreadAccess)
            LoadNodesFromContainer(_rootContainer);
        else
            await _treeView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => LoadNodesFromContainer(_rootContainer));
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

    private static bool IsSupportedFileType(string fileNameWithExtension) => fileNameWithExtension switch
    {
        ".cfg" or ".ini" or ".json" or ".yaml" or ".wad" or ".client" => true,
        _ => false
    };

    private static SolidColorBrush GetLoadingBackground(ElementTheme actualTheme)
    {
        return actualTheme == ElementTheme.Dark
            ? s_loading_dark_background
            : s_loading_light_background;
    }
}
