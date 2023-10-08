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

using GZSkinsX.Contracts.Command;
using GZSkinsX.Contracts.Utilities;

using Microsoft.UI.Xaml.Controls;

using Windows.Foundation.Collections;

namespace GZSkinsX.Appx.Command;

/// <inheritdoc cref="ICommandBarService"/>
[Shared, Export(typeof(ICommandBarService))]
internal sealed class CommandBarService : ICommandBarService
{
    /// <summary>
    /// 用于存储导出的 <see cref="ICommandBarItem"/> 对象以及 <see cref="CommandBarItemContractAttribute"/> 元数据。
    /// </summary>
    internal sealed class CommandBarItemMD(Lazy<ICommandBarItem, CommandBarItemContractAttribute> lazy)
    {
        /// <summary>
        /// 当前上下文中的懒加载对象。
        /// </summary>
        private readonly Lazy<ICommandBarItem, CommandBarItemContractAttribute> _lazy = lazy;

        /// <summary>
        /// 获取当前上下文的 <see cref="ICommandBarItem"/> 对象。
        /// </summary>
        public ICommandBarItem Value => _lazy.Value;

        /// <summary>
        /// 获取当前上下文的 <see cref="CommandBarItemContractAttribute"/> 元数据。
        /// </summary>
        public CommandBarItemContractAttribute Metadata => _lazy.Metadata;
    }

    /// <summary>
    /// 用于存储命令项的组的上下文信息。
    /// </summary>
    internal sealed class CommandBarItemGroupMD(string name, double order)
    {
        /// <summary>
        /// 获取该分组的名称。
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// 获取该分组的排序顺序。
        /// </summary>
        public double Order { get; } = order;

        /// <summary>
        /// 获取该分组中的子菜单项。
        /// </summary>
        public List<CommandBarItemMD> Items { get; } = new List<CommandBarItemMD>();
    }

    /// <summary>
    /// 表示已枚举的所有导出的 <see cref="ICommandBarItem"/> 命令项 和 <see cref="CommandBarItemContractAttribute"/> 元数据的集合。
    /// </summary>
    private readonly IEnumerable<Lazy<ICommandBarItem, CommandBarItemContractAttribute>> _mefItems;

    /// <summary>
    /// 用于存放所有已序列化后的组，并以 <see cref="Guid">OwnerGuid</see> 作为键。
    /// </summary>
    private readonly Dictionary<Guid, Dictionary<string, CommandBarItemGroupMD>> _guidToGroups;

    /// <summary>
    /// 初始化 <see cref="CommandBarService"/> 的新实例。
    /// </summary>
    [ImportingConstructor]
    public CommandBarService([ImportMany] IEnumerable<Lazy<ICommandBarItem, CommandBarItemContractAttribute>> mefItems)
    {
        _mefItems = mefItems;
        _guidToGroups = [];

        InitializeGroups();
    }

    /// <summary>
    /// 通过枚举所有的命令项和元数据的集合进行序列化并分组。
    /// </summary>
    private void InitializeGroups()
    {
        var dict = new Dictionary<Guid, Dictionary<string, CommandBarItemGroupMD>>();
        foreach (var item in _mefItems)
        {
            var ownerGuidString = item.Metadata.OwnerGuid;
            var b = Guid.TryParse(ownerGuidString, out var ownerGuid);
            Debug.Assert(b, $"CommandBarItem: Couldn't parse OwnerGuid property: '{ownerGuidString}'");
            if (!b)
                continue;

            var groupString = item.Metadata.Group ?? "-1.7976931348623157E+308,23DEA732-397F-435A-8496-64D4250056AB";
            b = ItemGroupParser.TryParseGroup(groupString, out var groupName, out var groupOrder);
            Debug.Assert(b, $"CommandBarItem: Couldn't parse Group property: '{groupString}'");
            if (!b)
                continue;

            if (dict.TryGetValue(ownerGuid, out var groupDict) is false)
                dict.Add(ownerGuid, groupDict = new Dictionary<string, CommandBarItemGroupMD>());

            if (groupDict.TryGetValue(groupName, out var itemGroup) is false)
                groupDict.Add(groupName, itemGroup = new CommandBarItemGroupMD(groupName, groupOrder));

            itemGroup.Items.Add(new CommandBarItemMD(item));
        }

        _guidToGroups.Clear();
        foreach (var kv in dict)
        {
            var groupDict = kv.Value.OrderBy(a => a.Value.Order);
            foreach (var pair in groupDict)
            {
                pair.Value.Items.Sort((a, b) => a.Metadata.Order.CompareTo(b.Metadata.Order));
            }

            _guidToGroups.Add(kv.Key, new(groupDict));
        }
    }

    /// <summary>
    /// 通过枚举 <see cref="CommandBarItemMD"/> 集合，以将创建的 UI 元素添加至目标列表。
    /// </summary>
    /// <param name="collection">用于将已创建的 UI 子元素的添加至目标集合的列表。</param>
    /// <param name="items">用于创建 UI 子元素的集合。</param>
    private static void CreateSubItems(IObservableVector<ICommandBarElement> collection, IEnumerable<CommandBarItemMD> items)
    {
        foreach (var item in items)
        {
            var value = item.Value;
            if (value is ICommandBarToggleButton toggleButton)
                collection.Add(new CommandBarToggleButtonContainer(collection, toggleButton).UIObject);
            else if (value is ICommandBarButton button)
                collection.Add(new CommandBarButtonContainer(collection, button).UIObject);
            else if (value is ICommandBarObject uiObject)
                collection.Add(new CommandBarObjectContainer(collection, uiObject).UIObject);
            else
                continue;
        }
    }


    /// <summary>
    /// 内部的用于创建命令栏的核心实现。
    /// </summary>
    /// <param name="ownerGuidString">子命令项所归属的 <see cref="Guid"/> 字符串值。</param>
    /// <returns>已创建的 <see cref="CommandBar"/> 类型实例。</returns>
    private CommandBar CreateCommandBarCore(string ownerGuidString)
    {
        var commandBar = new CommandBar();
        if (Guid.TryParse(ownerGuidString, out var ownerGuid) &&
            _guidToGroups.TryGetValue(ownerGuid, out var itemGroups))
        {
            var primaryNeedSeparator = false;
            var secondaryNeedSeparator = false;

            foreach (var group in itemGroups.Values)
            {
                var primaryCommands = group.Items.Where(item => item.Metadata.Placement is CommandBarItemPlacement.Primary);
                if (primaryCommands.Any())
                {
                    if (primaryNeedSeparator)
                        commandBar.PrimaryCommands.Add(new AppBarSeparator());
                    else
                        primaryNeedSeparator = true;

                    CreateSubItems(commandBar.PrimaryCommands, primaryCommands);
                }

                var secondaryCommands = group.Items.Where(item => item.Metadata.Placement is CommandBarItemPlacement.Secondary);
                if (secondaryCommands.Any())
                {
                    if (secondaryNeedSeparator)
                        commandBar.SecondaryCommands.Add(new AppBarSeparator());
                    else
                        secondaryNeedSeparator = true;

                    CreateSubItems(commandBar.SecondaryCommands, secondaryCommands);
                }
            }
        }

        return commandBar;
    }

    /// <inheritdoc/>
    public CommandBar CreateCommandBar(string ownerGuidString) => CreateCommandBarCore(ownerGuidString);
}
