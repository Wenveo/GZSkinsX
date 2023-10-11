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

using GZSkinsX.Contracts.ContextMenu;
using GZSkinsX.Contracts.Utilities;

using Microsoft.UI.Xaml.Controls.Primitives;

namespace GZSkinsX.Appx.ContextMenu;

/// <inheritdoc cref="IContextMenuService"/>
[Shared, Export(typeof(IContextMenuService))]
internal sealed partial class ContextMenuService : IContextMenuService
{
    /// <summary>
    /// 用于存储子菜单项的组的上下文信息。
    /// </summary>
    private sealed class ContextMenuItemGroupMD(string name, double order)
    {
        /// <summary>
        /// 获取该组的名称。
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// 获取该组的排序顺序。
        /// </summary>
        public double Order { get; } = order;

        /// <summary>
        /// 获取该组中的子菜单项
        /// </summary>
        public List<ContextMenuItemMD> Items { get; } = new List<ContextMenuItemMD>();
    }

    /// <summary>
    /// 用于存储导出的 <see cref="IContextMenuItem"/> 对象以及 <see cref="ContextMenuItemContractAttribute"/> 的元数据类。
    /// </summary>
    private sealed class ContextMenuItemMD(Lazy<IContextMenuItem, ContextMenuItemContractAttribute> lazy)
    {
        /// <summary>
        /// 当前上下文中的懒加载对象。
        /// </summary>
        private readonly Lazy<IContextMenuItem, ContextMenuItemContractAttribute> _lazy = lazy;

        /// <summary>
        /// 获取当前上下文的 <see cref="IContextMenuItem"/> 对象。
        /// </summary>
        public IContextMenuItem Value => _lazy.Value;

        /// <summary>
        /// 获取当前上下文的 <see cref="ContextMenuItemContractAttribute"/> 元数据。
        /// </summary>
        public ContextMenuItemContractAttribute Metadata => _lazy.Metadata;
    }

    /// <summary>
    /// 用于存放所有已枚举的 <see cref="IContextMenuItem"/> 菜单项和 <see cref="ContextMenuItemContractAttribute"/> 元数据的集合。
    /// </summary>
    private readonly IEnumerable<Lazy<IContextMenuItem, ContextMenuItemContractAttribute>> _mefItems;

    /// <summary>
    /// 用于存放所有已序列化后的组，并以 <see cref="Guid">OwnerGuid</see> 作为键。
    /// </summary>
    private readonly Dictionary<Guid, Dictionary<string, ContextMenuItemGroupMD>> _guidToGroups;

    /// <summary>
    /// 初始化 <see cref="ContextMenuService"/> 的新实例。
    /// </summary>
    [ImportingConstructor]
    public ContextMenuService([ImportMany] IEnumerable<Lazy<IContextMenuItem, ContextMenuItemContractAttribute>> mefItems)
    {
        _mefItems = mefItems;
        _guidToGroups = [];

        InitializeGroups();
    }

    /// <summary>
    /// 通过枚举所有的菜单项和元数据的集合进行序列化并分组。
    /// </summary>
    private void InitializeGroups()
    {
        var dict = new Dictionary<Guid, Dictionary<string, ContextMenuItemGroupMD>>();
        foreach (var item in _mefItems)
        {
            var ownerGuidString = item.Metadata.OwnerGuid;
            var b = Guid.TryParse(ownerGuidString, out var ownerGuid);
            Debug.Assert(b, $"ContextMenuItem: Couldn't parse OwnerGuid property: '{ownerGuidString}'");
            if (!b)
                continue;

            var guidString = item.Metadata.Guid;
            b = Guid.TryParse(guidString, out var guid);
            Debug.Assert(b, $"ContextMenuItem: Couldn't parse Guid property: '{guidString}'");
            if (!b)
                continue;

            var groupString = item.Metadata.Group ?? "-1.7976931348623157E+308,9B6619D4-5486-4F6B-A1E0-3BAC663392F5";
            b = ItemGroupParser.TryParseGroup(groupString, out var groupName, out var groupOrder);
            Debug.Assert(b, $"ContextMenuItem: Couldn't parse Group property: '{groupString}'");
            if (!b)
                continue;

            if (dict.TryGetValue(ownerGuid, out var groupDict) is false)
                dict.Add(ownerGuid, groupDict = new Dictionary<string, ContextMenuItemGroupMD>());

            if (groupDict.TryGetValue(groupName, out var itemGroup) is false)
                groupDict.Add(groupName, itemGroup = new ContextMenuItemGroupMD(groupName, groupOrder));

            itemGroup.Items.Add(new ContextMenuItemMD(item));
        }

        _guidToGroups.Clear();
        foreach (var kv in dict)
        {
            var groupDict = kv.Value.OrderBy(a => a.Value.Order);
            foreach (var pair in groupDict)
            {
                var hashSet = new HashSet<Guid>();
                var origList = new List<ContextMenuItemMD>(pair.Value.Items);

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

    /// <inheritdoc/>
    public FlyoutBase CreateCommandBarMenu(string ownerGuidString)
    => CreateCommandBarMenuCore(ownerGuidString, null, null);

    /// <inheritdoc/>
    public FlyoutBase CreateCommandBarMenu(string ownerGuidString, ICommandBarMenuOptions options)
    => CreateCommandBarMenuCore(ownerGuidString, options, null);

    /// <inheritdoc/>
    public FlyoutBase CreateCommandBarMenu(string ownerGuidString, ContextMenuUIContextCallback callback)
    => CreateCommandBarMenuCore(ownerGuidString, null, callback);

    /// <inheritdoc/>
    public FlyoutBase CreateCommandBarMenu(string ownerGuidString, ICommandBarMenuOptions options, ContextMenuUIContextCallback callback)
    => CreateCommandBarMenuCore(ownerGuidString, options, callback);

    /// <inheritdoc/>
    public FlyoutBase CreateContextMenu(string ownerGuidString)
    => CreateContextMenuCore(ownerGuidString, null, null);

    /// <inheritdoc/>
    public FlyoutBase CreateContextMenu(string ownerGuidString, IContextMenuOptions options)
    => CreateContextMenuCore(ownerGuidString, options, null);

    /// <inheritdoc/>
    public FlyoutBase CreateContextMenu(string ownerGuidString, ContextMenuUIContextCallback callback)
    => CreateContextMenuCore(ownerGuidString, null, callback);

    /// <inheritdoc/>
    public FlyoutBase CreateContextMenu(string ownerGuidString, IContextMenuOptions options, ContextMenuUIContextCallback callback)
    => CreateContextMenuCore(ownerGuidString, options, callback);

}
