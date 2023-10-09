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
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Utilities;

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

using Windows.Foundation.Metadata;

namespace GZSkinsX.Appx.ContextMenu;

/// <inheritdoc cref="IContextMenuService"/>
[Shared, Export(typeof(IContextMenuService))]
internal sealed class ContextMenuService : IContextMenuService
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

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 上下文菜单项并为指定的 <see cref="MenuFlyoutItem"/> 设置通用的 UI 属性。
    /// </summary>
    /// <param name="uiObject">需要设置 UI 属性的对象。</param>
    /// <param name="itemContext">传入的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    private static void SetCommonUIProperties(MenuFlyoutItem uiObject, IContextMenuItem itemContext, IContextMenuUIContext uiContext)
    {
        var icon = itemContext.Icon;
        if (icon is not null)
            uiObject.Icon = icon;

        var header = itemContext.Header;
        if (header is not null)
            AutomationProperties.SetName(uiObject, uiObject.Text = ResourceHelper.GetResxLocalizedOrDefault(header));

        var toolTip = itemContext.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(uiObject, toolTip is string str ?
                ResourceHelper.GetResxLocalizedOrDefault(str) : toolTip);

        foreach (var keyboardAccelerator in itemContext.KeyboardAccelerators.OfType<KeyboardAccelerator>())
            uiObject.KeyboardAccelerators.Add(keyboardAccelerator);

        var textOverride = itemContext.KeyboardAcceleratorTextOverride;
        if (string.IsNullOrWhiteSpace(textOverride) is false)
            uiObject.KeyboardAcceleratorTextOverride = ResourceHelper.GetResxLocalizedOrDefault(textOverride);

        uiObject.IsEnabled = itemContext.IsEnabled(uiContext);
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象创建一个新的 <see cref="MenuFlyoutItem"/> 实例。
    /// </summary>
    /// <param name="menuItem">传入的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <returns>返回已创建的 <see cref="MenuFlyoutItem"/> 实例。</returns>
    private static MenuFlyoutItem CreateMenuItem(IContextMenuItem menuItem, IContextMenuUIContext uiContext)
    {
        var menuFlyoutItem = new MenuFlyoutItem();
        SetCommonUIProperties(menuFlyoutItem, menuItem, uiContext);
        menuFlyoutItem.Click += (s, e) => menuItem.OnExecute(uiContext);
        return menuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象创建一个新的 <see cref="MenuFlyoutSubItem"/> 实例。
    /// </summary>
    /// <param name="menuItem">传入的上下文菜单项。</param>
    /// <returns>已创建的 <see cref="MenuFlyoutSubItem"/> 实例。</returns>
    private static MenuFlyoutSubItem CreateMenuSubItem(IContextMenuItem menuItem)
    {
        var menuFlyoutSubItem = new MenuFlyoutSubItem();

        var icon = menuItem.Icon;
        if (icon is not null)
            menuFlyoutSubItem.Icon = icon;

        var header = menuItem.Header;
        if (header is not null)
            AutomationProperties.SetName(menuFlyoutSubItem, menuFlyoutSubItem.Text = ResourceHelper.GetResxLocalizedOrDefault(header));

        var toolTip = menuItem.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(menuFlyoutSubItem, toolTip is string str ?
                ResourceHelper.GetResxLocalizedOrDefault(str) : toolTip);

        foreach (var keyboardAccelerator in menuItem.KeyboardAccelerators.OfType<KeyboardAccelerator>())
            menuFlyoutSubItem.KeyboardAccelerators.Add(keyboardAccelerator);

        return menuFlyoutSubItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextToggleMenuItem"/> 对象创建一个新的 <see cref="ToggleMenuFlyoutItem"/> 实例。
    /// </summary>
    /// <param name="toggleMenuItem">传入的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <returns>已创建的 <see cref="ToggleMenuFlyoutItem"/> 实例。</returns>
    private static ToggleMenuFlyoutItem CreateToggleMenuItem(IContextToggleMenuItem toggleMenuItem, IContextMenuUIContext uiContext)
    {
        var toggleMenuFlyoutItem = new ToggleMenuFlyoutItem
        {
            IsChecked = toggleMenuItem.IsChecked(uiContext)
        };
        SetCommonUIProperties(toggleMenuFlyoutItem, toggleMenuItem, uiContext);

        toggleMenuFlyoutItem.Click += (s, e) =>
        {
            toggleMenuItem.OnClick(toggleMenuFlyoutItem.IsChecked, uiContext);
            toggleMenuItem.OnExecute(uiContext);
        };

        return toggleMenuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextRadioMenuItem"/> 对象创建一个新的 <see cref="RadioMenuFlyoutItem"/> 实例。
    /// </summary>
    /// <param name="radioMenuItem">传入的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <returns>已创建的 <see cref="RadioMenuFlyoutItem"/> 实例。</returns>
    private static RadioMenuFlyoutItem CreateRadioMenuItem(IContextRadioMenuItem radioMenuItem, IContextMenuUIContext uiContext)
    {
        var radioMenuFlyoutItem = new RadioMenuFlyoutItem
        {
            IsChecked = radioMenuItem.IsChecked(uiContext)
        };
        SetCommonUIProperties(radioMenuFlyoutItem, radioMenuItem, uiContext);

        radioMenuFlyoutItem.Click += (s, e) =>
        {
            radioMenuItem.OnClick(radioMenuFlyoutItem.IsChecked, uiContext);
            radioMenuItem.OnExecute(uiContext);
        };

        var groupName = radioMenuItem.GroupName;
        if (groupName is not null)
            radioMenuFlyoutItem.GroupName = groupName;

        return radioMenuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象和 <see cref="ContextMenuItemContractAttribute"/> 元数据创建一个新的 <see cref="MenuFlyoutItemBase"/> 类型实例。
    /// </summary>
    /// <param name="item">传入的上下文菜单项。</param>
    /// <param name="metadata">与上下文菜单项相关联的元数据。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <returns>已创建的 <see cref="MenuFlyoutItemBase"/> 类型实例。</returns>
    private MenuFlyoutItemBase CreateContextMenuItem(IContextMenuItem item,
        ContextMenuItemContractAttribute metadata, IContextMenuUIContext uiContext)
    {
        if (Guid.TryParse(metadata.Guid, out var guid) &&
            _guidToGroups.TryGetValue(guid, out var subItemGroup))
        {
            var menuFlyoutSubItem = CreateMenuSubItem(item);
            CreateMenuSubItems(subItemGroup, menuFlyoutSubItem.Items, uiContext);
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
                    menuFlyoutSubItem2.Items.Add(CreateContextMenuItem(
                        subItem.ContextMenuItem, subItem.Metadata, uiContext));
            }

            return menuFlyoutSubItem2;
        }
        else if (item is IContextRadioMenuItem radioMenuItem)
        {
            return CreateRadioMenuItem(radioMenuItem, uiContext);
        }
        else if (item is IContextToggleMenuItem toggleMenuItem)
        {
            return CreateToggleMenuItem(toggleMenuItem, uiContext);
        }
        else
        {
            return CreateMenuItem(item, uiContext);
        }
    }

    /// <summary>
    /// 通过枚举的 <see cref="ContextMenuItemGroupMD"/> 组的集合中的子项，以对目标列表 <paramref name="collection"/> 创建和添加 UI 子元素。
    /// </summary>
    /// <param name="groups">用于创建 UI 子元素的组的集合。</param>
    /// <param name="collection">用于将已创建的 UI 子元素的添加至目标集合的列表。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    private void CreateMenuSubItems(Dictionary<string, ContextMenuItemGroupMD> groups, IList<MenuFlyoutItemBase> collection, IContextMenuUIContext uiContext)
    {
        var needSeparator = false;
        foreach (var itemGroup in groups.Values)
        {
            if (needSeparator)
                collection.Add(new MenuFlyoutSeparator());
            else
                needSeparator = true;

            foreach (var item in itemGroup.Items)
            {
                if (item.Value.IsVisible(uiContext))
                {
                    collection.Add(CreateContextMenuItem(item.Value, item.Metadata, uiContext));
                }
            }
        }
    }

    /// <summary>
    /// 根据传入的 <see cref="ContextMenuOptions"/> 配置选项应用至目标 <see cref="MenuFlyout"/> UI 对象。
    /// </summary>
    private static void ApplyOptionsForMenuFlyout(MenuFlyout menuFlyout, ContextMenuOptions options)
    {
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 1))
        {
            menuFlyout.Placement = options.Placement;
            menuFlyout.MenuFlyoutPresenterStyle = options.MenuFlyoutPresenterStyle;
        }

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3))
        {
            menuFlyout.AllowFocusOnInteraction = options.AllowFocusOnInteraction;
            menuFlyout.AllowFocusWhenDisabled = options.AllowFocusWhenDisabled;
            menuFlyout.LightDismissOverlayMode = options.LightDismissOverlayMode;
        }

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4))
        {
            menuFlyout.OverlayInputPassThroughElement = options.OverlayInputPassThroughElement;
        }

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            menuFlyout.AreOpenCloseAnimationsEnabled = options.AreOpenCloseAnimationsEnabled;
            menuFlyout.ShowMode = options.ShowMode;
        }

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
        {
            menuFlyout.ShouldConstrainToRootBounds = options.ShouldConstrainToRootBounds;
        }
    }

    /// <summary>
    /// 内部的创建上下文菜单方法的核心实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="Guid"/> 字符串值。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <param name="callback">目标 UI 上下文的回调委托。</param>
    /// <returns>已创建的 <see cref="MenuFlyout"/> 类型实例。</returns>
    private MenuFlyout CreateContextMenuCore(string ownerGuidString, ContextMenuOptions? options = null, ContextMenuUIContextCallback? callback = null)
    {
        var menuFlyout = new MenuFlyout();
        if (Guid.TryParse(ownerGuidString, out var guid) &&
            _guidToGroups.TryGetValue(guid, out var groups))
        {
            menuFlyout.Opening += (s, e) =>
            {
                // 应用上下文菜单的配置选项
                if (options is not null)
                    ApplyOptionsForMenuFlyout(menuFlyout, options);

                // 清空列表中的所有元素
                if (menuFlyout.Items.Count > 0)
                    menuFlyout.Items.Clear();

                // 重新创建新的菜单项列表
                CreateMenuSubItems(groups, menuFlyout.Items, callback is not null
                    ? callback(menuFlyout, e) : new ContextMenuUIContext(menuFlyout, e));

                // 如果没有元素则不应显示，否则将会显示一块空白的上下文菜单。
                if (menuFlyout.Items.Count == 0)
                    menuFlyout.Hide();
            };

            menuFlyout.Closed += (s, e) =>
            {
                // 释放对图标和提示的引用
                foreach (var item in menuFlyout.Items)
                {
                    if (item is MenuFlyoutItem menuFlyoutItem)
                    {
                        menuFlyoutItem.Icon = null;
                    }
                    else if (item is MenuFlyoutSubItem menuFlyoutSubItem)
                    {
                        menuFlyoutSubItem.Icon = null;
                    }

                    ToolTipService.SetToolTip(item, null);
                }
            };
        }
        else
        {
            if (options is not null)
            {
                // 如果没有枚举到菜单项，并且上下文菜单的配置选项不为空，则添加默认的应用配置选项的行为。
                menuFlyout.Opening += (s, e) => ApplyOptionsForMenuFlyout(menuFlyout, options);
            }
        }

        return menuFlyout;
    }

    /// <inheritdoc/>
    public MenuFlyout CreateContextMenu(string ownerGuidString)
    => CreateContextMenuCore(ownerGuidString, null, null);

    /// <inheritdoc/>
    public MenuFlyout CreateContextMenu(string ownerGuidString, ContextMenuOptions options)
    => CreateContextMenuCore(ownerGuidString, options, null);

    /// <inheritdoc/>
    public MenuFlyout CreateContextMenu(string ownerGuidString, ContextMenuUIContextCallback callback)
    => CreateContextMenuCore(ownerGuidString, null, callback);

    /// <inheritdoc/>
    public MenuFlyout CreateContextMenu(string ownerGuidString, ContextMenuOptions options, ContextMenuUIContextCallback callback)
    => CreateContextMenuCore(ownerGuidString, options, callback);
}
