// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;

using GZSkinsX.Extensions.CreatorStudio.Contracts.Documents;
using GZSkinsX.Extensions.CreatorStudio.Contracts.Documents.Tabs;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.ContextMenu;

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.Services.Documents.Tabs;

[Shared, Export(typeof(IDocumentTabService))]
internal sealed class DocumentTabService : IDocumentTabService
{
    private readonly IEnumerable<Lazy<IDocumentTabProvider, DocumentTabProviderMetadataAttribute>> _tabProviders;
    private readonly IDocumentService _documentService;

    private readonly Dictionary<Guid, DocumentTabProviderContext> _typedToProvider;

    private readonly MUXC.TabView _mainTabView;
    private MenuFlyout? _contextMenu;

    internal MenuFlyout SafeContextMenu
    {
        get
        {
            _contextMenu ??= AppxContext.ContextMenuService.CreateContextMenu(
                    DocumentTabConstants.DOCUMENT_TAB_CM_GUID,
                    new ContextMenuOptions { Placement = FlyoutPlacementMode.Bottom },
                    (sender, e) =>
                    {
                        if (sender is MenuFlyout menuFlyout &&
                            menuFlyout.Target is MUXC.TabViewItem tabViewItem &&
                            tabViewItem.DataContext is DocumentTabContext context)
                        {
                            return new DocumentTabContextMenuUIContext(tabViewItem, context._tab);
                        }

                        return new DocumentTabContextMenuUIContext(null, null);
                    });

            return _contextMenu;
        }
    }

    public object UIObject => _mainTabView;

    public IDocumentTab? ActiveTab
    {
        get
        {
            if (_mainTabView.SelectedItem is MUXC.TabViewItem tabViewItem &&
                tabViewItem.DataContext is DocumentTabContext context)
            {
                return context._tab;
            }

            return null;
        }
    }

    public IEnumerable<IDocumentTab> DocumentTabs
    {
        get
        {
            foreach (var item in _mainTabView.TabItems)
            {
                if (item is not MUXC.TabViewItem tabViewItem)
                    continue;

                if (tabViewItem.DataContext is not DocumentTabContext context)
                    continue;

                yield return context._tab;
            }
        }
    }

    public event EventHandler<ActiveDocumentTabChangedEventArgs>? ActiveTabChanged;

    public event TypedEventHandler<IDocumentTabService, DocumentTabCollectionChangedEventArgs>? CollectionChanged;

    [ImportingConstructor]
    public DocumentTabService(
        [ImportMany] IEnumerable<Lazy<IDocumentTabProvider, DocumentTabProviderMetadataAttribute>> tabProviders,
        IDocumentService documentService)
    {
        _mainTabView = new MUXC.TabView
        {
            IsAddTabButtonVisible = false,
            TabWidthMode = MUXC.TabViewWidthMode.Equal,
            VerticalAlignment = VerticalAlignment.Stretch,
            KeyboardAcceleratorPlacementMode = KeyboardAcceleratorPlacementMode.Hidden
        };

        _mainTabView.SelectionChanged += OnSelectionChanged;
        _mainTabView.TabCloseRequested += OnTabCloseRequested;

        _tabProviders = tabProviders;
        _documentService = documentService;
        _documentService.CollectionChanged += OnDocumentCollectionChanged;

        _typedToProvider = new Dictionary<Guid, DocumentTabProviderContext>();
        InitializeProviders();
    }

    private void InitializeProviders()
    {
        _typedToProvider.Clear();

        foreach (var item in _tabProviders)
        {
            var typedGuidString = item.Metadata.TypedGuid;
            var b = Guid.TryParse(typedGuidString, out var typedGuid);
            Debug.Assert(b, $"DocumentTabProvider: Couldn't parse TypedGuid property: '{typedGuidString}'");
            if (!b)
                continue;

            _typedToProvider[typedGuid] = new DocumentTabProviderContext(item);
        }
    }

    private void OnDocumentCollectionChanged(IDocumentService sender, DocumentCollectionChangedEventArgs args)
    {
        if (args.EventType == DocumentCollectionEventType.Add)
        {
            var addedItems = new List<IDocumentTab>();

            var lastItemIndex = -1;
            for (var i = 0; i < args.Documents.Length; i++)
            {
                var doc = args.Documents[i];

                var hasItem = false;
                for (var j = 0; j < _mainTabView.TabItems.Count; j++)
                {
                    var item = _mainTabView.TabItems[j];
                    if (item is not MUXC.TabViewItem tabViewItem)
                        continue;

                    if (tabViewItem.DataContext is not DocumentTabContext context)
                        continue;

                    if (context._tab.Document.Key.Equals(doc.Key))
                    {
                        hasItem = true;
                        if (i == args.Documents.Length - 1)
                        {
                            lastItemIndex = j;
                        }

                        break;
                    }
                }

                if (!hasItem)
                {
                    if (_typedToProvider.TryGetValue(doc.Info.TypedGuid, out var providerContext))
                    {
                        var createdTab = providerContext.Value.Create(doc);
                        var tabContext = new DocumentTabContext(createdTab, SafeContextMenu);

                        _mainTabView.TabItems.Add(tabContext.UIObject);
                        tabContext.InternalOnAdded();
                        addedItems.Add(createdTab);
                    }
                }
            }

            _mainTabView.SelectedIndex = lastItemIndex == -1 ? _mainTabView.TabItems.Count - 1 : lastItemIndex;
            CollectionChanged?.Invoke(this, new DocumentTabCollectionChangedEventArgs(addedItems.ToArray(), null));
        }
        else
        {
            var removedItems = new List<IDocumentTab>();
            var tabItems = _mainTabView.TabItems;
            var count = tabItems.Count;

            foreach (var doc in args.Documents)
            {
                for (var i = 0; i < count; i++)
                {
                    if (tabItems[i] is not MUXC.TabViewItem item)
                        continue;

                    if (item.DataContext is not DocumentTabContext context)
                        continue;

                    if (context._tab.Document.Key.Equals(doc.Key))
                    {
                        if (i == 0 && count > 1)
                        {
                            /// 如果此时需要被移除的元素正好是处于第 0 位，并且视图中还存在其它的选项卡，
                            /// 那么则需要把当前选择项调整至下一位元素，之后再进行移除的操作。
                            /// 如果不执行此段代码则会在 Remove 时引发 ArgumentException 异常。

                            _mainTabView.SelectedIndex = 1;
                        }

                        tabItems.RemoveAt(i);
                        context.InternalOnRemoved();
                        removedItems.Add(context._tab);

                        break;
                    }
                }
            }

            CollectionChanged?.Invoke(this, new DocumentTabCollectionChangedEventArgs(null, removedItems.ToArray()));
        }
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_mainTabView.SelectedItem is MUXC.TabViewItem tabViewItem &&
            tabViewItem.DataContext is DocumentTabContext context)
        {
            ActiveTabChanged?.Invoke(this, new ActiveDocumentTabChangedEventArgs(context._tab));
        }
        else
        {
            ActiveTabChanged?.Invoke(this, new ActiveDocumentTabChangedEventArgs(null));
        }
    }

    private void OnTabCloseRequested(MUXC.TabView sender, MUXC.TabViewTabCloseRequestedEventArgs args)
    {
        if (args.Tab.DataContext is DocumentTabContext context)
        {
            _documentService.Remove(context._tab.Document.Key);
        }
    }

    public void Close(IDocumentTab tab)
    {
        if (tab is null)
        {
            throw new ArgumentNullException(nameof(tab));
        }

        var tabItems = _mainTabView.TabItems;
        var count = tabItems.Count;

        for (var i = 0; i < count; i++)
        {
            if (tabItems[i] is not MUXC.TabViewItem item)
                continue;

            if (item.DataContext is not DocumentTabContext context)
                continue;

            if (context._tab != tab)
                continue;

            _documentService.Remove(context._tab.Document.Key);
            break;
        }
    }

    public void CloseActiveTab()
    {
        if (_mainTabView.SelectedItem is MUXC.TabViewItem item &&
            item.DataContext is DocumentTabContext context)
        {
            _documentService.Remove(context._tab.Document.Key);
        }
    }

    public void CloseAllButThis(IDocumentTab tab)
    {
        if (tab is null)
        {
            throw new ArgumentNullException(nameof(tab));
        }

        var removedKeys = new List<IDocumentKey>();
        var tabItems = _mainTabView.TabItems;
        var count = tabItems.Count;

        for (var i = 0; i < count; i++)
        {
            if (tabItems[i] is not MUXC.TabViewItem item)
                continue;

            if (item.DataContext is not DocumentTabContext context)
                continue;

            if (context._tab == tab)
                continue;

            removedKeys.Add(context._tab.Document.Key);
            break;
        }

        _documentService.Remove(removedKeys);
    }

    public void CloseAllTabs()
    {
        _documentService.Clear();

        if (_mainTabView.TabItems.Count > 0)
        {
            var tabItems = _mainTabView.TabItems;
            var count = tabItems.Count;

            for (var i = 0; i < count; i++)
            {
                if (tabItems[i] is MUXC.TabViewItem item)
                {
                    if (item.DataContext is DocumentTabContext context)
                    {
                        /// ???
                        context.InternalOnRemoved();
                    }
                }

                tabItems.RemoveAt(i--);
            }
        }
    }

    public void SetActiveTab(int index)
    {
        if (index < 0 || index > _mainTabView.TabItems.Count)
        {
            return;
        }

        _mainTabView.SelectedIndex = index;
    }

    public void SetActiveTab(IDocumentTab tab)
    {
        if (tab is null)
        {
            throw new ArgumentNullException(nameof(tab));
        }

        var tabItems = _mainTabView.TabItems;
        for (var i = 0; i < tabItems.Count; i++)
        {
            if (tabItems[i] is not MUXC.TabViewItem item)
                continue;

            if (item.DataContext is not DocumentTabContext context)
                continue;

            if (context._tab != tab)
                continue;

            _mainTabView.SelectedIndex = i;
            break;
        }
    }
}
