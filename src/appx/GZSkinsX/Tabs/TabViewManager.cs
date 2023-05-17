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
        if (ActiveTabChanged is null)
            return;

        if (_mainTabView.SelectedItem is MUXC.TabViewItem tabViewItem &&
            tabViewItem.DataContext is TabContentImpl tabContentImpl)
        {
            ActiveTabChanged.Invoke(this, new ActiveTabChangedEventArgs(tabContentImpl.Tab));
        }
        else
        {
            ActiveTabChanged.Invoke(this, new ActiveTabChangedEventArgs(null));
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
                try
                {
                    Close(tabContentImpl.Tab);
                }
                catch (ArgumentException)
                {
                    /// 如果鼠标悬停在关闭按钮上，并且一直连续点左键（类似连点器的操作），就会有小概率触发此异常。

                    /// 因为如果有多个 Tab 存在，在你通过点击关闭按钮将前面的一个 Tab 移除后，
                    /// 后面的 Tab 则会滑动回之前被移除的 Tab 的位置。

                    /// 那么这时可能某些人无聊（不是我），或者是其它什么的原因，
                    /// 鼠标就放在关闭按钮上一直左键点击，那前一个 Tab 被移除了下一个就会顶替原来的位置，
                    /// 然后又点到关闭按钮进行移除的操作，就会有小概率会寄。。
                    /// 当然这种情况只有在点击关闭按钮时才会发生，其它的 Api 不受影响。
                }
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

        CollectionChanged?.Invoke(this, new TabCollectionChangedEventArgs(new[] { tabContent }, null));
    }

    public void Close(ITabContent tabContent)
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

            if (tabViewItem.DataContext is not TabContentImpl tabContentImpl ||
                tabContentImpl.Tab != tabContent)
            {
                continue;
            }

            tabItems.RemoveAt(i);
            tabContentImpl.InternalOnRemoved();

            CollectionChanged?.Invoke(this, new TabCollectionChangedEventArgs(null, new[] { tabContent }));

            break;
        }
    }

    public void CloseActiveTab()
    {
        if (_mainTabView.SelectedItem is MUXC.TabViewItem tabViewItem &&
            tabViewItem.DataContext is TabContentImpl tabContentImpl)
        {
            _mainTabView.TabItems.Remove(tabViewItem);
            tabContentImpl.InternalOnRemoved();
            CollectionChanged?.Invoke(this, new TabCollectionChangedEventArgs(null, new[] { tabContentImpl.Tab }));
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
                if (tabItems[i] is MUXC.TabViewItem item)
                {
                    if (item.DataContext is TabContentImpl tabContentImpl)
                    {
                        if (tabContentImpl.Tab == activeTab)
                            continue;

                        tabContentImpl.InternalOnRemoved();
                        removedItems.Add(tabContentImpl.Tab);
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
            if (tabItems[i] is not MUXC.TabViewItem item ||
                item.DataContext is not TabContentImpl tabContentImpl)
            {
                continue;
            }

            tabItems.RemoveAt(i--);
            tabContentImpl.InternalOnRemoved();
            removedItems.Add(tabContentImpl.Tab);
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
