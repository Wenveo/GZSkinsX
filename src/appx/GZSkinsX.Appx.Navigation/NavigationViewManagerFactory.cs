// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;

using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Navigation;
using GZSkinsX.Contracts.Utilities;

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Navigation;

/// <inheritdoc cref="INavigationViewManagerFactory"/>
[Shared, Export(typeof(INavigationViewManagerFactory))]
internal sealed class NavigationViewManagerFactory : INavigationViewManagerFactory
{
    /// <summary>
    /// 存放所有已导出的 <see cref="INavigationItem"/> 对象实例。
    /// </summary>
    private readonly IEnumerable<Lazy<INavigationItem, NavigationItemContractAttribute>> _mefNavItems;

    /// <summary>
    /// 用于存放所有已序列化后的组，并以 <see cref="Guid">OwnerGuid</see> 作为键。
    /// </summary>
    private readonly Dictionary<Guid, Dictionary<string, NavigationItemGroupMD>> _guidToGroups;

    /// <summary>
    /// 初始化 <see cref="NavigationViewManagerFactory"/> 的新实例。
    /// </summary>
    [ImportingConstructor]
    public NavigationViewManagerFactory([ImportMany] IEnumerable<Lazy<INavigationItem, NavigationItemContractAttribute>> mefNavItems)
    {
        _mefNavItems = mefNavItems;
        _guidToGroups = [];
        InitializeGroups();
    }

    /// <summary>
    /// 通过枚举所有的菜单项和元数据的集合进行序列化并分组。
    /// </summary>
    private void InitializeGroups()
    {
        var dict = new Dictionary<Guid, Dictionary<string, NavigationItemGroupMD>>();
        foreach (var item in _mefNavItems)
        {
            var ownerGuidString = item.Metadata.OwnerGuid;
            var b = Guid.TryParse(ownerGuidString, out var ownerGuid);
            Debug.Assert(b, $"NavigationItem: Couldn't parse OwnerGuid property: '{ownerGuidString}'");
            if (!b)
                continue;

            var guidString = item.Metadata.Guid;
            b = Guid.TryParse(guidString, out var guid);
            Debug.Assert(b, $"NavigationItem: Couldn't parse Guid property: '{guidString}'");
            if (!b)
                continue;

            var groupString = item.Metadata.Group ?? "-1.7976931348623157E+308,8BE6B202-33BB-4C25-ADC0-1638EBFFE1D9";
            b = ItemGroupParser.TryParseGroup(groupString, out var groupName, out var groupOrder);
            Debug.Assert(b, $"NavigationItem: Couldn't parse Group property: '{groupString}'");
            if (!b)
                continue;

            if (!dict.TryGetValue(ownerGuid, out var groupDict))
                dict.Add(ownerGuid, groupDict = new Dictionary<string, NavigationItemGroupMD>());
            if (!groupDict.TryGetValue(groupName, out var itemGroup))
                groupDict.Add(groupName, itemGroup = new NavigationItemGroupMD(groupName, groupOrder));

            itemGroup.Items.Add(new NavigationItemMD(item));
        }

        _guidToGroups.Clear();
        foreach (var kv in dict)
        {
            var groupDict = kv.Value.OrderBy(a => a.Value.Order);
            foreach (var pair in groupDict)
            {
                var hashSet = new HashSet<Guid>();
                var origList = new List<NavigationItemMD>(pair.Value.Items);

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

    /// <summary>
    /// 通过解析 <see cref="NavigationItemMD"/> 上下文对象并创建 <see cref="NavigationViewItem"/> 对象实例。
    /// </summary>
    /// <param name="context">需要解析的 <see cref="NavigationItemMD"/> 上下文对象。</param>
    /// <returns>已创建的 <see cref="NavigationViewItem"/> 对象实例。</returns>
    private NavigationViewItem CreateNavItemUIObject(NavigationItemMD context)
    {
        var navItem = new NavigationViewItem();
        NavigationViewItemHelper.SetItemContext(navItem, context);

        var icon = context.Value.Icon;
        if (icon is not null)
            navItem.Icon = icon;

        var header = context.Metadata.Header;
        if (header is not null)
            navItem.Content = ResourceHelper.GetResxLocalizedOrDefault(header);

        if (context.Metadata.PageType is null)
            navItem.SelectsOnInvoked = false;

        if (Guid.TryParse(context.Metadata.Guid, out var guid) &&
            _guidToGroups.TryGetValue(guid, out var groupDict))
        {
            foreach (var pair in groupDict)
            {
                foreach (var item in pair.Value.Items)
                {
                    navItem.MenuItems.Add(CreateNavItemUIObject(item));
                }
            }
        }

        return navItem;
    }

    /// <summary>
    /// 内部的用于创建和返回 <see cref="INavigationViewManager"/> 实例的核心实现。
    /// </summary>
    /// <param name="ownerGuidString">导航项所归属的 <see cref="Guid"/> 字符串值。</param>
    /// <param name="options">用于创建导航视图管理的配置选项。</param>
    /// <returns>已创建的 <see cref="NavigationViewManager"/> 类型实例。</returns>
    private INavigationViewManager NavigationViewManagerCore(string ownerGuidString, NavigationViewManagerOptions? options)
    {
        var navigationView = options?.Target ?? new NavigationView();
        if (Guid.TryParse(ownerGuidString, out var ownerGuid) &&
            _guidToGroups.TryGetValue(ownerGuid, out var itemGroups))
        {
            var needSeparator = false;
            var container = navigationView.MenuItems;
            foreach (var group in itemGroups.Values.Select(group => group.Items.Where(item => item.Metadata.Placement is not NavigationItemPlacement.Footer)))
            {
                if (!group.Any())
                    continue;

                if (needSeparator)
                    container.Add(new NavigationViewItemSeparator());
                else
                    needSeparator = true;

                foreach (var item in group)
                    container.Add(CreateNavItemUIObject(item));
            }

            needSeparator = false;
            container = navigationView.FooterMenuItems;
            foreach (var group in itemGroups.Values.Select(group => group.Items.Where(item => item.Metadata.Placement is NavigationItemPlacement.Footer)))
            {
                if (!group.Any())
                    continue;

                if (needSeparator)
                    container.Add(new NavigationViewItemSeparator());
                else
                    needSeparator = true;

                foreach (var item in group)
                    container.Add(CreateNavItemUIObject(item));
            }
        }

        return new NavigationViewManager(navigationView, options?.DoNotCreateSearchBox is true);
    }

    /// <inheritdoc/>
    public INavigationViewManager CreateNavigationViewManager(string ownerGuidString)
    => NavigationViewManagerCore(ownerGuidString, null);

    /// <inheritdoc/>
    public INavigationViewManager CreateNavigationViewManager(string ownerGuidString, NavigationViewManagerOptions options)
    => NavigationViewManagerCore(ownerGuidString, options);
}
