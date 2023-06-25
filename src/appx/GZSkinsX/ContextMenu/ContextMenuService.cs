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
using GZSkinsX.Api.Diagnostics;
using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.Utilities;

using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.ContextMenu;

/// <inheritdoc cref="IContextMenuService"/>
[Shared, Export(typeof(IContextMenuService))]
internal sealed class ContextMenuService : IContextMenuService
{
    /// <summary>
    /// 用于存放所有已枚举的 <see cref="IContextMenuItem"/> 菜单项和 <see cref="ContextMenuItemMetadataAttribute"/> 元数据的集合
    /// </summary>
    private readonly IEnumerable<Lazy<IContextMenuItem, ContextMenuItemMetadataAttribute>> _mefItems;

    /// <summary>
    /// 用于存放所有已序列化后的组，并以 <see cref="System.Guid">OwnerGuid</see> 作为键
    /// </summary>
    private readonly Dictionary<Guid, Dictionary<string, ContextItemGroupContext>> _guidToGroups;

    /// <summary>
    /// 初始化 <see cref="ContextMenuService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public ContextMenuService([ImportMany] IEnumerable<Lazy<IContextMenuItem, ContextMenuItemMetadataAttribute>> mefItems)
    {
        _mefItems = mefItems;
        _guidToGroups = new();

        InitializeGroups();
    }

    /// <summary>
    /// 通过枚举所有的菜单项和元数据的集合进行序列化并分组
    /// </summary>
    private void InitializeGroups()
    {
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
            b = ItemGroupParser.TryParseGroup(groupString, out var groupName, out var groupOrder);
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

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 上下文菜单项并为指定的 <see cref="MenuFlyoutItem"/> 设置通用的 UI 属性
    /// </summary>
    /// <param name="uiObject">需要设置 UI 属性的对象</param>
    /// <param name="menuItem">传入的上下文菜单项</param>
    private void SetCommonUIProperties(MenuFlyoutItem uiObject, IContextMenuItem menuItem)
    {
        var icon = menuItem.Icon;
        if (icon is not null)
            uiObject.Icon = icon;

        var header = menuItem.Header;
        if (header is not null)
            AutomationProperties.SetName(uiObject, uiObject.Text = ResourceHelper.GetResxLocalizedOrDefault(header));

        var shortcutKey = menuItem.ShortcutKey;
        if (shortcutKey is not null)
            uiObject.KeyboardAccelerators.Add(shortcutKey);

        var toolTip = menuItem.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(uiObject, toolTip);
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象创建一个新的 <see cref="MenuFlyoutItem"/> 实例
    /// </summary>
    /// <param name="menuItem">传入的上下文菜单项</param>
    /// <returns>返回已创建的 <see cref="MenuFlyoutItem"/> 实例</returns>
    private MenuFlyoutItem CreateMenuItem(IContextMenuItem menuItem)
    {
        static void OnClick(object sender, RoutedEventArgs e)
        {
            var self = (MenuFlyoutItem)sender;
            var item = (IContextMenuItem)self.DataContext;
            item.OnExecute((IContextMenuUIContext)self.Tag);
        }

        var menuFlyoutItem = new MenuFlyoutItem { DataContext = menuItem };
        SetCommonUIProperties(menuFlyoutItem, menuItem);
        menuFlyoutItem.Click += OnClick;

        return menuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象创建一个新的 <see cref="MenuFlyoutSubItem"/> 实例
    /// </summary>
    /// <param name="menuItem">传入的上下文菜单项</param>
    /// <returns>已创建的 <see cref="MenuFlyoutSubItem"/> 实例</returns>
    private MenuFlyoutSubItem CreateMenuSubItem(IContextMenuItem menuItem)
    {
        var menuFlyoutSubItem = new MenuFlyoutSubItem { DataContext = menuItem };

        var icon = menuItem.Icon;
        if (icon is not null)
            menuFlyoutSubItem.Icon = icon;

        var header = menuItem.Header;
        if (header is not null)
            AutomationProperties.SetName(menuFlyoutSubItem, menuFlyoutSubItem.Text = ResourceHelper.GetResxLocalizedOrDefault(header));

        var shortcutKey = menuItem.ShortcutKey;
        if (shortcutKey is not null)
            menuFlyoutSubItem.KeyboardAccelerators.Add(shortcutKey);

        var toolTip = menuItem.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(menuFlyoutSubItem, toolTip);

        return menuFlyoutSubItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextToggleMenuItem"/> 对象创建一个新的 <see cref="ToggleMenuFlyoutItem"/> 实例
    /// </summary>
    /// <param name="toggleMenuItem">传入的上下文菜单项</param>
    /// <returns>已创建的 <see cref="ToggleMenuFlyoutItem"/> 实例</returns>
    private ToggleMenuFlyoutItem CreateToggleMenuItem(IContextToggleMenuItem toggleMenuItem)
    {
        static void OnClick(object sender, RoutedEventArgs e)
        {
            var self = (ToggleMenuFlyoutItem)sender;
            var item = (IContextToggleMenuItem)self.DataContext;

            var uiContext = (IContextMenuUIContext)self.Tag;
            item.OnClick(self.IsChecked, uiContext);
            item.OnExecute(uiContext);
        }

        var toggleMenuFlyoutItem = new ToggleMenuFlyoutItem { DataContext = toggleMenuItem };
        SetCommonUIProperties(toggleMenuFlyoutItem, toggleMenuItem);
        toggleMenuFlyoutItem.Click += OnClick;

        return toggleMenuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextRadioMenuItem"/> 对象创建一个新的 <see cref="MUXC.RadioMenuFlyoutItem"/> 实例
    /// </summary>
    /// <param name="radioMenuItem">传入的上下文菜单项</param>
    /// <returns>已创建的 <see cref="MUXC.RadioMenuFlyoutItem"/> 实例</returns>
    private MUXC.RadioMenuFlyoutItem CreateRadioMenuItem(IContextRadioMenuItem radioMenuItem)
    {
        static void OnClick(object sender, RoutedEventArgs e)
        {
            var self = (MUXC.RadioMenuFlyoutItem)sender;
            var item = (IContextRadioMenuItem)self.DataContext;

            var uiContext = (IContextMenuUIContext)self.Tag;
            item.OnClick(self.IsChecked, uiContext);
            item.OnExecute(uiContext);
        }

        var radioMenuFlyoutItem = new MUXC.RadioMenuFlyoutItem { DataContext = radioMenuItem };
        SetCommonUIProperties(radioMenuFlyoutItem, radioMenuItem);
        radioMenuFlyoutItem.Click += OnClick;

        var groupName = radioMenuItem.GroupName;
        if (groupName is not null)
            radioMenuFlyoutItem.GroupName = groupName;

        return radioMenuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象和 <see cref="ContextMenuItemMetadataAttribute"/> 元数据创建一个新的 <see cref="MenuFlyoutItemBase"/> 类型实例
    /// </summary>
    /// <param name="item">传入的上下文菜单项</param>
    /// <param name="metadata">与上下文菜单项相关联的元数据</param>
    /// <returns>已创建的 <see cref="MenuFlyoutItemBase"/> 类型实例</returns>
    private MenuFlyoutItemBase CreateContextMenuItem(IContextMenuItem item, ContextMenuItemMetadataAttribute metadata)
    {
        if (Guid.TryParse(metadata.Guid, out var guid) &&
            _guidToGroups.TryGetValue(guid, out var subItemGroup))
        {
            var menuFlyoutSubItem = CreateMenuSubItem(item);
            CreateMenuSubItems(subItemGroup, menuFlyoutSubItem.Items);
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

    /// <summary>
    /// 通过枚举的 <see cref="ContextItemGroupContext"/> 组的集合中的子项，以对目标列表 <paramref name="collection"/> 创建和添加 UI 子元素
    /// </summary>
    /// <param name="groups">用于创建 UI 子元素的组的集合</param>
    /// <param name="collection">用于将已创建的 UI 子元素的添加至目标集合的列表</param>
    private void CreateMenuSubItems(Dictionary<string, ContextItemGroupContext> groups, IList<MenuFlyoutItemBase> collection)
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

    /// <summary>
    /// 根据传入的 <see cref="ContextMenuOptions"/> 配置选项应用至目标 <see cref="MenuFlyout"/> UI 对象
    /// </summary>
    private void ApplyOptionsForMenuFlyout(MenuFlyout menuFlyout, ContextMenuOptions options)
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
    /// 内部的创建上下文菜单方法的通用实现
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="System.Guid"/> 字符串值</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项</param>
    /// <param name="coerceValueCallback">目标 UI 上下文的回调委托</param>
    /// <returns>已创建的 <see cref="MenuFlyout"/> 类型实例</returns>
    private MenuFlyout CoreceContextMenu(string ownerGuidString, ContextMenuOptions? options = null, CoerceContextMenuUIContextCallback? coerceValueCallback = null)
    {
        static void OnOpening(object sender, object e)
        {
            static void InitializeSubItem(MenuFlyoutItemBase item, IContextMenuUIContext uiContext)
            {
                if (item.GetType() == typeof(MenuFlyoutSeparator))
                    return;

                var menuItem = (IContextMenuItem)item.DataContext;

                item.IsEnabled = menuItem.IsEnabled(uiContext);
                item.Visibility = BoolToVisibilityConvert.ToVisibility(menuItem.IsVisible(uiContext));

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
            CreateMenuSubItems(groups, menuFlyout.Items);
            ContextMenuFlyoutHelper.SetCoerceValueCallback(menuFlyout, coerceValueCallback);
            menuFlyout.Opening += OnOpening;
        }

        if (options is not null)
            ApplyOptionsForMenuFlyout(menuFlyout, options);

        return menuFlyout;
    }

    /// <inheritdoc/>
    public MenuFlyout CreateContextMenu(string ownerGuidString)
    => CoreceContextMenu(ownerGuidString, null, null);

    /// <inheritdoc/>
    public MenuFlyout CreateContextMenu(string ownerGuidString, ContextMenuOptions options)
    => CoreceContextMenu(ownerGuidString, options, null);

    /// <inheritdoc/>
    public MenuFlyout CreateContextMenu(string ownerGuidString, CoerceContextMenuUIContextCallback coerceValueCallback)
    => CoreceContextMenu(ownerGuidString, null, coerceValueCallback);

    /// <inheritdoc/>
    public MenuFlyout CreateContextMenu(string ownerGuidString, ContextMenuOptions options, CoerceContextMenuUIContextCallback coerceValueCallback)
    => CoreceContextMenu(ownerGuidString, options, coerceValueCallback);
}
