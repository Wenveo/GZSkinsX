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
using System.Linq;

using GZSkinsX.Api.ContextMenu;
using GZSkinsX.DotNet.Diagnostics;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.ContextMenu;

[Shared, Export(typeof(IContextMenuService))]
internal sealed class ContextMenuService : IContextMenuService
{
    private readonly IEnumerable<Lazy<IContextMenuItem, ContextMenuItemMetadataAttribute>> _mefItems;
    private readonly Dictionary<Guid, Dictionary<string, ContextItemGroupContext>> _guidToGroups;

    [ImportingConstructor]
    public ContextMenuService([ImportMany] IEnumerable<Lazy<IContextMenuItem, ContextMenuItemMetadataAttribute>> mefItems)
    {
        _mefItems = mefItems;
        _guidToGroups = new();

        InitializeGroups();
    }

    private void InitializeGroups()
    {
        static bool ParseGroup(string group, out string name, out double order)
        {
            var indexOfSeparator = group.IndexOf(',');
            if (indexOfSeparator == -1 || !double.TryParse(group[..indexOfSeparator++], out order))
            {
                name = string.Empty;
                order = double.NaN;
                return false;
            }

            name = group[indexOfSeparator..];
            return true;
        }

        var dict = new Dictionary<Guid, Dictionary<string, ContextItemGroupContext>>();
        foreach (var item in _mefItems)
        {
            var ownerGuidString = item.Metadata.OwnerGuid;
            var b = Guid.TryParse(ownerGuidString, out var ownerGuid);
            Debug2.Assert(b, $"ContextMenuItem: Couldn't parse OwnerGuid property: '{ownerGuidString}'");
            if (!b)
                continue;

            var guidString = item.Metadata.Guid;
            b = Guid.TryParse(guidString, out var guid);
            Debug2.Assert(b, $"ContextMenuItem: Couldn't parse Guid property: '{guidString}'");
            if (!b)
                continue;

            var groupString = item.Metadata.Group ?? "-1.7976931348623157E+308,9B6619D4-5486-4F6B-A1E0-3BAC663392F5";
            b = ParseGroup(groupString, out var groupName, out var groupOrder);
            Debug2.Assert(b, $"ContextMenuItem: Couldn't parse Group property: '{groupString}'");
            if (!b)
                continue;

            if (!dict.TryGetValue(ownerGuid, out var groupDict))
                dict.Add(ownerGuid, groupDict = new Dictionary<string, ContextItemGroupContext>());
            if (!groupDict.TryGetValue(groupName, out var itemGroup))
                groupDict.Add(groupName, itemGroup = new ContextItemGroupContext(groupName, groupOrder));

            itemGroup.Items.Add(new ContextMenuItemContext(item));
        }

        _guidToGroups.Clear();
        foreach (var kv in dict)
        {
            var groupDict = kv.Value.OrderBy(a => a.Value.Order);
            foreach (var pair in groupDict)
            {
                var hashSet = new HashSet<Guid>();
                var origList = new List<ContextMenuItemContext>(pair.Value.Items);

                pair.Value.Items.Clear();
                foreach (var item in origList)
                {
                    var guid = new Guid(item.Metadata.Guid);
                    if (hashSet.Contains(guid))
                        continue;

                    hashSet.Add(guid);
                    pair.Value.Items.Add(item);
                }

                pair.Value.Items.Sort((a, b) => a.Metadata.Order.CompareTo(b.Metadata.Order));
            }

            _guidToGroups.Add(kv.Key, new(groupDict));
        }
    }

    private void SetCommonUIProperties(MenuFlyoutItemBase uiObject, IContextMenuItem menuItem)
    {
        var hotKey = menuItem.HotKey;
        if (hotKey is not null)
            uiObject.KeyboardAccelerators.Add(new() { Key = hotKey.Key, Modifiers = hotKey.Modifiers });

        var toolTip = menuItem.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(uiObject, toolTip);
    }

    private MenuFlyoutItem CreateMenuItem(IContextMenuItem menuItem)
    {
        var menuFlyoutItem = new MenuFlyoutItem
        {
            Icon = menuItem.Icon,
            Text = menuItem.Header,
            DataContext = menuItem
        };

        SetCommonUIProperties(menuFlyoutItem, menuItem);

        static void OnClick(object sender, RoutedEventArgs e)
        {
            var self = (MenuFlyoutItem)sender;
            var item = (IContextMenuItem)self.DataContext;
            item.OnExecute((IContextMenuUIContext)self.Tag);
        }

        menuFlyoutItem.Click += OnClick;
        return menuFlyoutItem;
    }

    private MenuFlyoutSubItem CreateMenuSubItem(IContextMenuItem menuItem)
    {
        var menuFlyoutSubItem = new MenuFlyoutSubItem
        {
            Icon = menuItem.Icon,
            Text = menuItem.Header,
            DataContext = menuItem
        };

        SetCommonUIProperties(menuFlyoutSubItem, menuItem);

        return menuFlyoutSubItem;
    }

    private ToggleMenuFlyoutItem CreateToggleMenuItem(IContextToggleMenuItem toggleMenuItem)
    {
        var toggleMenuFlyoutItem = new ToggleMenuFlyoutItem
        {
            Icon = toggleMenuItem.Icon,
            Text = toggleMenuItem.Header,
            DataContext = toggleMenuItem
        };

        SetCommonUIProperties(toggleMenuFlyoutItem, toggleMenuItem);

        static void OnClick(object sender, RoutedEventArgs e)
        {
            var self = (ToggleMenuFlyoutItem)sender;
            var item = (IContextToggleMenuItem)self.DataContext;

            var uiContext = (IContextMenuUIContext)self.Tag;
            item.OnToggle(self.IsChecked, uiContext);
            item.OnExecute(uiContext);
        }

        toggleMenuFlyoutItem.Click += OnClick;
        return toggleMenuFlyoutItem;
    }

    private MUXC.RadioMenuFlyoutItem CreateRadioMenuItem(IContextRadioMenuItem radioMenuItem)
    {
        var radioMenuFlyoutItem = new MUXC.RadioMenuFlyoutItem
        {
            Icon = radioMenuItem.Icon,
            Text = radioMenuItem.Header,
            GroupName = radioMenuItem.GroupName ?? string.Empty,
            DataContext = radioMenuItem,
        };

        SetCommonUIProperties(radioMenuFlyoutItem, radioMenuItem);

        static void OnClick(object sender, RoutedEventArgs e)
        {
            var self = (MUXC.RadioMenuFlyoutItem)sender;
            var item = (IContextRadioMenuItem)self.DataContext;

            var uiContext = (IContextMenuUIContext)self.Tag;
            item.OnClick(self.IsChecked, uiContext);
            item.OnExecute(uiContext);
        }

        radioMenuFlyoutItem.Click += OnClick;
        return radioMenuFlyoutItem;
    }

    private MenuFlyoutItemBase CreateContextMenuItem(IContextMenuItem item, ContextMenuItemMetadataAttribute metadata)
    {
        if (Guid.TryParse(metadata.Guid, out var guid) &&
            _guidToGroups.TryGetValue(guid, out var subItemGroup))
        {
            var menuFlyoutSubItem = CreateMenuSubItem(item);
            CreateSubItems(subItemGroup, menuFlyoutSubItem.Items);
            return menuFlyoutSubItem;
        }
        else if (item is IContextMenuItemProvider provider)
        {
            var menuFlyoutSubItem2 = CreateMenuSubItem(item);
            foreach (var subItem in provider.CreateSubItems())
            {
                if (subItem.IsEmpty)
                    menuFlyoutSubItem2.Items.Add(new MenuFlyoutSeparator());
                else
                    menuFlyoutSubItem2.Items.Add(CreateContextMenuItem(subItem.ContextMenuItem, subItem.Metadata));
            }

            return menuFlyoutSubItem2;
        }
        else if (item is IContextRadioMenuItem radioMenuItem)
        {
            return CreateRadioMenuItem(radioMenuItem);
        }
        else if (item is IContextToggleMenuItem toggleMenuItem)
        {
            return CreateToggleMenuItem(toggleMenuItem);
        }
        else
        {
            return CreateMenuItem(item);
        }
    }

    private void CreateSubItems(Dictionary<string, ContextItemGroupContext> groups, IList<MenuFlyoutItemBase> collection)
    {
        var needSeparator = false;
        foreach (var itemGroup in groups.Values)
        {
            if (needSeparator)
                collection.Add(new MenuFlyoutSeparator());
            else
                needSeparator = true;

            foreach (var item in itemGroup.Items)
                collection.Add(CreateContextMenuItem(item.Value, item.Metadata));
        }
    }

    public MenuFlyout CreateContextFlyout(string ownerGuidString, CoerceContextMenuUIContextCallback? coerceValueCallback = null)
    {
        static void OnOpening(object sender, object e)
        {
            static void InitializeSubItem(MenuFlyoutItemBase item, IContextMenuUIContext uiContext)
            {
                if (item.GetType() == typeof(MenuFlyoutSeparator))
                    return;

                var menuItem = (IContextMenuItem)item.DataContext;

                item.IsEnabled = menuItem.IsEnabled(uiContext);
                item.Visibility = Bool2Visibility(menuItem.IsVisible(uiContext));

                if (item is MenuFlyoutItem menuFlyoutItem)
                {
                    menuFlyoutItem.Tag = uiContext;

                    if (item is ToggleMenuFlyoutItem toggleMenuFlyoutItem)
                    {
                        var toggleMenuItem = (IContextToggleMenuItem)menuFlyoutItem.DataContext;
                        toggleMenuFlyoutItem.IsChecked = toggleMenuItem.IsChecked(uiContext);
                    }
                    else if (item is MUXC.RadioMenuFlyoutItem radioMenuFlyoutItem)
                    {
                        var toggleMenuItem = (IContextRadioMenuItem)menuFlyoutItem.DataContext;
                        radioMenuFlyoutItem.IsChecked = toggleMenuItem.IsChecked(uiContext);
                    }
                    else
                    {
                        return;
                    }
                }
                else if (item is MenuFlyoutSubItem menuFlyoutSubItem)
                {
                    foreach (var subItem in menuFlyoutSubItem.Items)
                    {
                        InitializeSubItem(subItem, uiContext);
                    }
                }
                else
                {
                    return;
                }
            }

            var self = (MenuFlyout)sender;
            var callback = ContextMenuFlyoutHelper.GetCoerceValueCallback(self);
            var uiContext = callback is not null ? callback(sender, e) : new ContextMenuUIContext(sender, e);
            foreach (var item in self.Items)
            {
                InitializeSubItem(item, uiContext);
            }
        }

        var menuFlyout = new MenuFlyout();
        if (Guid.TryParse(ownerGuidString, out var guid) &&
            _guidToGroups.TryGetValue(guid, out var groups))
        {
            CreateSubItems(groups, menuFlyout.Items);
            ContextMenuFlyoutHelper.SetCoerceValueCallback(menuFlyout, coerceValueCallback);

            menuFlyout.Opening += OnOpening;
        }

        return menuFlyout;
    }

    private static Visibility Bool2Visibility(bool value)
    {
        return value ? Visibility.Visible : Visibility.Collapsed;
    }
}
