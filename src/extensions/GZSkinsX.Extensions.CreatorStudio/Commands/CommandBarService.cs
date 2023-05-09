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

using GZSkinsX.Api.CreatorStudio.Commands;
using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.Utilities;

using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace GZSkinsX.Extensions.CreatorStudio.Commands;

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
    /// 通过指定的 <see cref="ICommandButton"/> 类型对象创建一个新的 <see cref="AppBarButton"/> 实例
    /// </summary>
    /// <param name="item">用于创建 UI 元素的命令按钮</param>
    /// <param name="parent">需要设置的父命令栏元素实例</param>
    /// <returns>返回已创建的 <see cref="AppBarButton"/> 实例</returns>
    private AppBarButton CreateAppBarButton(ICommandButton item, CommandBar parent)
    {
        var appBarButton = new AppBarButton { DataContext = item };
        CommandBarElementHelper.SetParentCommandBar(appBarButton, parent);

        var displayName = item.DisplayName!;
        if (!string.IsNullOrEmpty(displayName))
            AutomationProperties.SetName(appBarButton, appBarButton.Label = ResourceHelper.GetResxLocalizedOrDefault(displayName));

        var toolTip = item.ToolTip;
        if (toolTip is not null)
        {
            if (toolTip is string str)
                ToolTipService.SetToolTip(appBarButton, ResourceHelper.GetResxLocalizedOrDefault(str));
            else
                ToolTipService.SetToolTip(appBarButton, toolTip);
        }

        var iconElement = item.Icon;
        if (iconElement is not null)
            appBarButton.Icon = iconElement;

        var commandHotKey = item.ShortcutKey;
        if (commandHotKey is not null)
            appBarButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = commandHotKey.Key, Modifiers = commandHotKey.Modifiers });

        static void OnClick(object sender, RoutedEventArgs e)
        {
            var self = (AppBarButton)sender;
            var item = (ICommandButton)self.DataContext;

            item.OnClick(sender, e);

            var parent = CommandBarElementHelper.GetParentCommandBar(self);
            if (parent is not null)
            {
                UpdateUIState(parent);
            }
        }

        appBarButton.Click += OnClick;

        return appBarButton;
    }

    /// <summary>
    /// 通过指定的 <see cref="ICommandToggleButton"/> 类型对象创建一个新的 <see cref="AppBarToggleButton"/> 实例
    /// </summary>
    /// <param name="item">用于创建 UI 元素的命令切换按钮</param>
    /// <param name="parent">需要设置的父命令栏元素实例</param>
    /// <returns>已创建的 <see cref="AppBarToggleButton"/> 实例</returns>
    private AppBarToggleButton CreateAppBarToggleButton(ICommandToggleButton item, CommandBar parent)
    {
        var appBarToggleButton = new AppBarToggleButton { DataContext = item };
        CommandBarElementHelper.SetParentCommandBar(appBarToggleButton, parent);

        var displayName = item.DisplayName!;
        if (!string.IsNullOrEmpty(displayName))
            AutomationProperties.SetName(appBarToggleButton, appBarToggleButton.Label = ResourceHelper.GetResxLocalizedOrDefault(displayName));

        var toolTip = item.ToolTip;
        if (toolTip is not null)
        {
            if (toolTip is string str)
                ToolTipService.SetToolTip(appBarToggleButton, ResourceHelper.GetResxLocalizedOrDefault(str));
            else
                ToolTipService.SetToolTip(appBarToggleButton, toolTip);
        }

        var iconElement = item.Icon;
        if (iconElement is not null)
            appBarToggleButton.Icon = iconElement;

        var commandHotKey = item.ShortcutKey;
        if (commandHotKey is not null)
            appBarToggleButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = commandHotKey.Key, Modifiers = commandHotKey.Modifiers });

        static void OnClick(object sender, RoutedEventArgs e)
        {
            var self = (AppBarToggleButton)sender;
            var item = (ICommandToggleButton)self.DataContext;

            item.OnClick(sender, e);
        }

        static void OnChecked(object sender, RoutedEventArgs e)
        {
            var self = (AppBarToggleButton)sender;
            var item = (ICommandToggleButton)self.DataContext;

            item.OnChecked(sender, e);

            var parent = CommandBarElementHelper.GetParentCommandBar(self);
            if (parent is not null)
            {
                UpdateUIState(parent);
            }
        }

        static void OnUnchecked(object sender, RoutedEventArgs e)
        {
            var self = (AppBarToggleButton)sender;
            var item = (ICommandToggleButton)self.DataContext;

            item.OnUnchecked(sender, e);

            var parent = CommandBarElementHelper.GetParentCommandBar(self);
            if (parent is not null)
            {
                UpdateUIState(parent);
            }
        }

        appBarToggleButton.Click += OnClick;
        appBarToggleButton.Checked += OnChecked;
        appBarToggleButton.Unchecked += OnUnchecked;

        return appBarToggleButton;
    }

    /// <summary>
    /// 通过指定的 <see cref="ICommandObject"/> 类型对象创建一个新的 <see cref="AppBarElementContainer"/> 实例
    /// </summary>
    /// <param name="item">用于创建 UI 元素的自定义命令对象</param>
    /// <returns>已创建的 <see cref="AppBarElementContainer"/> 实例</returns>
    private AppBarElementContainer CreateElementContainer(ICommandObject item)
    {
        var elementContainer = new AppBarElementContainer
        {
            DataContext = item,
            VerticalContentAlignment = VerticalAlignment.Center,
        };

        var uiObject = item.UIObject;
        if (uiObject is not null)
            elementContainer.Content = uiObject;

        return elementContainer;
    }

    /// <summary>
    /// 通过枚举 <see cref="CommandItemContext"/> 集合，以将创建的 UI 元素添加至目标列表
    /// </summary>
    /// <param name="collection">用于将已创建的 UI 子元素的添加至目标集合的列表</param>
    /// <param name="parent">需要设置的父命令栏元素实例</param>
    /// <param name="items">用于创建 UI 子元素的集合</param>
    /// <returns>已创建的 UI 子元素的数量</returns>
    private int CreateSubItems(IObservableVector<ICommandBarElement> collection, CommandBar parent, IEnumerable<CommandItemContext> items)
    {
        var count = 0;
        foreach (var item in items)
        {
            var value = item.Value;
            if (value is ICommandToggleButton toggleButton)
                collection.Add(CreateAppBarToggleButton(toggleButton, parent));
            else if (value is ICommandButton button)
                collection.Add(CreateAppBarButton(button, parent));
            else if (value is ICommandObject uiObject)
                collection.Add(CreateElementContainer(uiObject));
            else
                continue;

            count++;
        }

        return count;
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
                if (primaryNeedSeparator)
                    commandBar.PrimaryCommands.Add(new AppBarSeparator());
                if (secondaryNeedSeparator)
                    commandBar.SecondaryCommands.Add(new AppBarSeparator());

                primaryNeedSeparator = CreateSubItems(commandBar.PrimaryCommands, commandBar,
                    group.Items.Where(item => item.Metadata.Placement == CommandPlacement.Primary)) > 0;

                secondaryNeedSeparator = CreateSubItems(commandBar.SecondaryCommands, commandBar,
                    group.Items.Where(item => item.Metadata.Placement == CommandPlacement.Secondary)) > 0;
            }
        }

        commandBar.Loaded += OnLoaded;
        return commandBar;
    }

    /// <summary>
    /// 更新目标命令栏中的所有子元素的 UI 状态
    /// </summary>
    /// <param name="targetElement">需要更新 UI 状态的目标元素</param>
    private static void UpdateUIState(CommandBar targetElement)
    {
        static void UpdateSubItems(IEnumerable<ICommandBarElement> items)
        {
            foreach (var item in items)
            {
                if (item.GetType() == typeof(AppBarSeparator))
                    continue;

                if (item is Control control && control.DataContext is ICommandButton commandButton)
                {
                    control.IsEnabled = commandButton.IsEnabled();
                    control.Visibility = BoolToVisibilityConvert.ToVisibility(commandButton.IsVisible());

                    var toggleButton = control as AppBarToggleButton;
                    if (toggleButton is not null)
                    {
                        toggleButton.IsChecked = ((ICommandToggleButton)commandButton).IsChecked();
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        UpdateSubItems(targetElement.PrimaryCommands);
        UpdateSubItems(targetElement.SecondaryCommands);
    }

    /// <summary>
    /// 用于在菜单项首次加载时，更新所有子元素的 UI 状态
    /// </summary>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ((CommandBar)sender).Loaded -= OnLoaded;
        UpdateUIState((CommandBar)sender);
    }

    /// <inheritdoc/>
    public CommandBar CreateCommandBar(string ownerGuidString)
    => CoreceCommandBar(ownerGuidString);
}
