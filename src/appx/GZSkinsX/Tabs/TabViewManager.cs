// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Tabs;

using Windows.Foundation;
using Windows.UI.Xaml.Controls;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Tabs;

internal sealed class TabViewManager : ITabViewManager
{
    internal readonly TabViewManagerOptions _options;
    internal readonly MUXC.TabView _mainTabView;
    internal readonly MenuFlyout _contextMenu;

    public object UIObject => _mainTabView;

    public ITabContent? ActiveTab
    {
        get
        {
            if (_mainTabView.SelectedItem is MUXC.TabViewItem tabViewItem &&
                tabViewItem.DataContext is TabContentImpl tabContentImpl)
            {
                return tabContentImpl.Tab;
            }

            return null;
        }
    }

    public IEnumerable<ITabContent> TabContents
    {
        get
        {
            foreach (var item in _mainTabView.TabItems)
            {
                if (item is not MUXC.TabViewItem tabViewItem)
                    continue;

                if (tabViewItem.DataContext is not TabContentImpl tabContentImpl)
                    continue;

                yield return tabContentImpl.Tab;
            }
        }
    }

    public event EventHandler<ActiveTabChangedEventArgs>? ActiveTabChanged;

    public event TypedEventHandler<ITabViewManager, TabCollectionChangedEventArgs>? CollectionChanged;

    public TabViewManager(MUXC.TabView tabView, TabViewManagerOptions options)
    {
        _mainTabView = tabView;
        _options = options;

        _contextMenu = AppxContext.ContextMenuService.CreateContextMenu(
                options.TabViewManagerGuid,
                options.ContextMenuOptions!,
                options.ContextMenuUIContextCallback!);

        if (options.TabViewStyle is not null)
            _mainTabView.Style = options.TabViewStyle;

        _mainTabView.SelectionChanged += OnSelectionChanged;
        _mainTabView.TabCloseRequested += OnTabCloseRequested;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_mainTabView.SelectedItem is MUXC.TabViewItem tabViewItem &&
            tabViewItem.DataContext is TabContentImpl tabContentImpl)
        {
            ActiveTabChanged?.Invoke(this, new ActiveTabChangedEventArgs(tabContentImpl.Tab));
        }
        else
        {
            ActiveTabChanged?.Invoke(this, new ActiveTabChangedEventArgs(null));
        }
    }

    private void OnTabCloseRequested(MUXC.TabView sender, MUXC.TabViewTabCloseRequestedEventArgs args)
    {
        if (args.Tab.DataContext is TabContentImpl tabContentImpl)
        {
            var args2 = new TabContentCloseRequestedEventArgs();
            tabContentImpl.Tab.OnCloseRequested(args2);

            if (!args2.Handled)
            {
                Close(tabContentImpl.Tab);
            }
        }
    }

    public void Add(ITabContent tabContent)
    {
        if (tabContent is null)
        {
            throw new ArgumentNullException(nameof(tabContent));
        }

        var tabContentImpl = new TabContentImpl(this, tabContent);
        _mainTabView.TabItems.Add(tabContentImpl.UIObject);
        tabContentImpl.InternalOnAdded();
        _mainTabView.SelectedIndex = _mainTabView.TabItems.Count - 1;

        CollectionChanged?.Invoke(this, new TabCollectionChangedEventArgs(new[] { tabContent }, null));
    }

    public void Close(ITabContent tabContent)
    {
        if (tabContent is null)
        {
            throw new ArgumentNullException(nameof(tabContent));
        }

        var tabItems = _mainTabView.TabItems;
        var count = tabItems.Count;

        for (var i = 0; i < count; i++)
        {
            if (tabItems[i] is not MUXC.TabViewItem tab)
                continue;

            if (tab.DataContext is not TabContentImpl impl || impl.Tab != tabContent)
                continue;

            if (i == 0 && count > 1)
            {
                /// 如果此时需要被移除的元素正好是处于第 0 位，并且视图中还存在其它的选项卡，
                /// 那么则需要把当前选择项调整至下一位元素，之后再进行移除的操作。
                /// 如果不执行此段代码则会在 Remove 时引发 ArgumentException 异常。

                _mainTabView.SelectedIndex = 1;
            }

            tabItems.RemoveAt(i);
            impl.InternalOnRemoved();

            CollectionChanged?.Invoke(this, new TabCollectionChangedEventArgs(null, new[] { tabContent }));
            break;
        }
    }

    public void CloseActiveTab()
    {
        if (_mainTabView.SelectedItem is MUXC.TabViewItem tab &&
            tab.DataContext is TabContentImpl impl)
        {
            var index = _mainTabView.TabItems.IndexOf(tab);

            _mainTabView.TabItems.RemoveAt(index);
            impl.InternalOnRemoved();

            CollectionChanged?.Invoke(this, new TabCollectionChangedEventArgs(null, new[] { impl.Tab }));

            if (index == 0 && _mainTabView.TabItems.Count > 0)
                _mainTabView.SelectedIndex = 0;
        }
    }

    public void CloseAllButActiveTab()
    {
        var activeTab = ActiveTab;
        if (activeTab is not null)
        {
            var removedItems = new List<ITabContent>();
            var tabItems = _mainTabView.TabItems;

            for (var i = 0; i < tabItems.Count; i++)
            {
                if (tabItems[i] is MUXC.TabViewItem tab)
                {
                    if (tab.DataContext is TabContentImpl impl)
                    {
                        if (impl.Tab == activeTab)
                            continue;

                        impl.InternalOnRemoved();
                        removedItems.Add(impl.Tab);
                    }
                }

                tabItems.RemoveAt(i--);
                break;
            }

            CollectionChanged?.Invoke(this, new TabCollectionChangedEventArgs(null, removedItems.ToArray()));
        }
    }

    public void CloseAllTabs()
    {
        var removedItems = new List<ITabContent>();
        var tabItems = _mainTabView.TabItems;

        for (var i = 0; i < tabItems.Count; i++)
        {
            if (tabItems[i] is not MUXC.TabViewItem tab ||
                tab.DataContext is not TabContentImpl impl)
            {
                continue;
            }

            tabItems.RemoveAt(i--);
            impl.InternalOnRemoved();
            removedItems.Add(impl.Tab);
        }

        CollectionChanged?.Invoke(this, new TabCollectionChangedEventArgs(null, removedItems.ToArray()));
    }

    public void SetActiveTab(int index)
    {
        if (index < 0 || index > _mainTabView.TabItems.Count)
        {
            return;
        }

        _mainTabView.SelectedIndex = index;
    }

    public void SetActiveTab(ITabContent tabContent)
    {
        if (tabContent is null)
        {
            throw new ArgumentNullException(nameof(tabContent));
        }

        var tabItems = _mainTabView.TabItems;
        for (var i = 0; i < tabItems.Count; i++)
        {
            var item = tabItems[i];
            if (item is not MUXC.TabViewItem tabViewItem)
                continue;

            if (tabViewItem.DataContext is not TabContentImpl impl || impl.Tab != tabContent)
                continue;

            _mainTabView.SelectedIndex = i;
            break;
        }
    }
}
