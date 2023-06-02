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
using System.Linq;

using GZSkinsX.SDK.Commands;
using GZSkinsX.SDK.Utilities;
using GZSkinsX.Commands.Impl;

using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Commands;

/// <inheritdoc cref="ICommandBarService"/>
[Shared, Export(typeof(ICommandBarService))]
internal sealed class CommandBarService : ICommandBarService
{
    /// <summary>
    /// 表示已枚举的所有导出的 <see cref="ICommandItem"/> 命令项 和 <see cref="CommandItemMetadataAttribute"/> 元数据的集合
    /// </summary>
    private readonly IEnumerable<Lazy<ICommandItem, CommandItemMetadataAttribute>> _mefItems;

    /// <summary>
    /// 用于存放所有已序列化后的组，并以 <see cref="Guid">OwnerGuid</see> 作为键
    /// </summary>
    private readonly Dictionary<Guid, Dictionary<string, CommandItemGroupContext>> _guidToGroups;

    /// <summary>
    /// 初始化 <see cref="CommandBarService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public CommandBarService([ImportMany] IEnumerable<Lazy<ICommandItem, CommandItemMetadataAttribute>> mefItems)
    {
        _mefItems = mefItems;
        _guidToGroups = new();

        InitializeGroups();
    }

    /// <summary>
    /// 通过枚举所有的命令项和元数据的集合进行序列化并分组
    /// </summary>
    private void InitializeGroups()
    {
        var dict = new Dictionary<Guid, Dictionary<string, CommandItemGroupContext>>();
        foreach (var item in _mefItems)
        {
            var ownerGuidString = item.Metadata.OwnerGuid;
            var b = Guid.TryParse(ownerGuidString, out var ownerGuid);
            Debug.Assert(b, $"CommandItem: Couldn't parse OwnerGuid property: '{ownerGuidString}'");
            if (!b)
                continue;

            var groupString = item.Metadata.Group ?? "-1.7976931348623157E+308,23DEA732-397F-435A-8496-64D4250056AB";
            b = ItemGroupParser.TryParseGroup(groupString, out var groupName, out var groupOrder);
            Debug.Assert(b, $"CommandItem: Couldn't parse Group property: '{groupString}'");
            if (!b)
                continue;

            if (!dict.TryGetValue(ownerGuid, out var groupDict))
                dict.Add(ownerGuid, groupDict = new Dictionary<string, CommandItemGroupContext>());
            if (!groupDict.TryGetValue(groupName, out var itemGroup))
                groupDict.Add(groupName, itemGroup = new CommandItemGroupContext(groupName, groupOrder));

            itemGroup.Items.Add(new CommandItemContext(item));
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
    /// 通过枚举 <see cref="CommandItemContext"/> 集合，以将创建的 UI 元素添加至目标列表
    /// </summary>
    /// <param name="collection">用于将已创建的 UI 子元素的添加至目标集合的列表</param>
    /// <param name="items">用于创建 UI 子元素的集合</param>
    private void CreateSubItems(IObservableVector<ICommandBarElement> collection, IEnumerable<CommandItemContext> items)
    {
        foreach (var item in items)
        {
            var value = item.Value;
            if (value is ICommandToggleButton toggleButton)
                collection.Add(new CommandToggleButtonImpl(toggleButton).UIObject);
            else if (value is ICommandButton button)
                collection.Add(new CommandButtonImpl(button).UIObject);
            else if (value is ICommandObject uiObject)
                collection.Add(new CommandObjectImpl(uiObject).UIObject);
            else
                continue;
        }
    }

    /// <summary>
    /// 内部的用于创建命令栏的核心实现
    /// </summary>
    /// <param name="ownerGuidString">子命令项所归属的 <see cref="Guid"/> 字符串值</param>
    /// <returns>已创建的 <see cref="CommandBar"/> 类型实例</returns>
    private CommandBar CoreceCommandBar(string ownerGuidString)
    {
        var commandBar = new CommandBar();
        if (Guid.TryParse(ownerGuidString, out var ownerGuid) &&
            _guidToGroups.TryGetValue(ownerGuid, out var itemGroups))
        {
            var primaryNeedSeparator = false;
            var secondaryNeedSeparator = false;

            foreach (var group in itemGroups.Values)
            {
                var primaryCommands = group.Items.Where(item => item.Metadata.Placement == CommandPlacement.Primary);
                if (primaryCommands.Any())
                {
                    if (primaryNeedSeparator)
                        commandBar.PrimaryCommands.Add(new AppBarSeparator());
                    else
                        primaryNeedSeparator = true;

                    CreateSubItems(commandBar.PrimaryCommands, primaryCommands);
                }

                var secondaryCommands = group.Items.Where(item => item.Metadata.Placement == CommandPlacement.Secondary);
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
    public CommandBar CreateCommandBar(string ownerGuidString) => CoreceCommandBar(ownerGuidString);
}
