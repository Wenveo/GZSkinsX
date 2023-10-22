// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using CommunityToolkit.Mvvm.Input;

using GZSkinsX.Contracts.ContextMenu;

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

using Windows.Foundation.Metadata;

namespace GZSkinsX.Appx.ContextMenu;

partial class ContextMenuService
{
    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 上下文菜单项并为指定的 <see cref="MenuFlyoutItem"/> 设置通用的 UI 属性。
    /// </summary>
    /// <param name="uiObject">需要设置 UI 属性的对象。</param>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    private static void SetCommonUIProperties(
        MenuFlyoutItem uiObject,
        IContextMenuItem menuItem,
        IContextMenuUIContext uiContext)
    {
        var icon = menuItem.Icon;
        if (icon is not null)
            uiObject.Icon = icon;

        var header = menuItem.Header;
        if (header is not null)
            AutomationProperties.SetName(uiObject, uiObject.Text = header);

        var toolTip = menuItem.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(uiObject, toolTip);

        foreach (var keyboardAccelerator in menuItem.KeyboardAccelerators.OfType<KeyboardAccelerator>())
            uiObject.KeyboardAccelerators.Add(keyboardAccelerator);

        var textOverride = menuItem.KeyboardAcceleratorTextOverride;
        if (string.IsNullOrWhiteSpace(textOverride) is false)
            uiObject.KeyboardAcceleratorTextOverride = textOverride;

        uiObject.IsEnabled = menuItem.IsEnabled(uiContext);
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象创建一个新的 <see cref="MenuFlyoutItem"/> 实例。
    /// </summary>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    /// <returns>已创建的 <see cref="MenuFlyoutItem"/> 实例。</returns>
    private static MenuFlyoutItem CreateMenuItem(
        IContextMenuItem menuItem,
        IContextMenuUIContext uiContext,
        Action? closeFlyoutAction = null)
    {
        var menuFlyoutItem = new MenuFlyoutItem()
        {
            Command = new RelayCommand(() =>
            {
                closeFlyoutAction?.Invoke();
                menuItem.OnExecute(uiContext);
            })
        };
        SetCommonUIProperties(menuFlyoutItem, menuItem, uiContext);

        return menuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象创建一个新的 <see cref="MenuFlyoutSubItem"/> 实例。
    /// </summary>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <returns>已创建的 <see cref="MenuFlyoutSubItem"/> 实例。</returns>
    private static MenuFlyoutSubItem CreateMenuSubItem(IContextMenuItem menuItem)
    {
        var menuFlyoutSubItem = new MenuFlyoutSubItem();

        var icon = menuItem.Icon;
        if (icon is not null)
            menuFlyoutSubItem.Icon = icon;

        var header = menuItem.Header;
        if (header is not null)
            AutomationProperties.SetName(menuFlyoutSubItem, menuFlyoutSubItem.Text = header);

        var toolTip = menuItem.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(menuFlyoutSubItem, toolTip);

        foreach (var keyboardAccelerator in menuItem.KeyboardAccelerators.OfType<KeyboardAccelerator>())
            menuFlyoutSubItem.KeyboardAccelerators.Add(keyboardAccelerator);

        return menuFlyoutSubItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextToggleMenuItem"/> 对象创建一个新的 <see cref="ToggleMenuFlyoutItem"/> 实例。
    /// </summary>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    /// <returns>已创建的 <see cref="ToggleMenuFlyoutItem"/> 实例。</returns>
    private static ToggleMenuFlyoutItem CreateToggleMenuItem(
        IContextToggleMenuItem menuItem,
        IContextMenuUIContext uiContext,
        Action? closeFlyoutAction = null)
    {
        var toggleMenuFlyoutItem = new ToggleMenuFlyoutItem
        {
            IsChecked = menuItem.IsChecked(uiContext)
        };
        toggleMenuFlyoutItem.Command = new RelayCommand(() =>
        {
            closeFlyoutAction?.Invoke();
            menuItem.OnClick(toggleMenuFlyoutItem.IsChecked, uiContext);
            menuItem.OnExecute(uiContext);
        });
        SetCommonUIProperties(toggleMenuFlyoutItem, menuItem, uiContext);

        return toggleMenuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextRadioMenuItem"/> 对象创建一个新的 <see cref="RadioMenuFlyoutItem"/> 实例。
    /// </summary>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    /// <returns>已创建的 <see cref="RadioMenuFlyoutItem"/> 实例。</returns>
    private static RadioMenuFlyoutItem CreateRadioMenuItem(
        IContextRadioMenuItem menuItem,
        IContextMenuUIContext uiContext,
        Action? closeFlyoutAction = null)
    {
        var radioMenuFlyoutItem = new RadioMenuFlyoutItem
        {
            IsChecked = menuItem.IsChecked(uiContext)
        };
        radioMenuFlyoutItem.Command = new RelayCommand(() =>
        {
            closeFlyoutAction?.Invoke();
            menuItem.OnClick(radioMenuFlyoutItem.IsChecked, uiContext);
            menuItem.OnExecute(uiContext);
        });
        SetCommonUIProperties(radioMenuFlyoutItem, menuItem, uiContext);

        return radioMenuFlyoutItem;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象和 <see cref="ContextMenuItemContractAttribute"/> 元数据创建一个新的 UI 对象实例。
    /// </summary>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <param name="metadata">与上下文菜单项相关联的元数据。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    /// <returns>已创建的 UI 对象实例。</returns>
    private MenuFlyoutItemBase CreateContextMenuItem(
        IContextMenuItem menuItem, ContextMenuItemContractAttribute metadata,
        IContextMenuUIContext uiContext, Action? closeFlyoutAction = null)
    {
        if (menuItem is IContextMenuItemProvider provider)
        {
            var menuFlyoutSubItem2 = CreateMenuSubItem(menuItem);
            foreach (var subItem in provider.CreateSubItems())
            {
                if (subItem.IsEmpty)
                    menuFlyoutSubItem2.Items.Add(new MenuFlyoutSeparator());
                else
                    menuFlyoutSubItem2.Items.Add(CreateContextMenuItem(
                        subItem.ContextMenuItem, subItem.Metadata, uiContext, closeFlyoutAction));
            }

            return menuFlyoutSubItem2;
        }
        else if (menuItem is IContextRadioMenuItem radioMenuItem)
        {
            return CreateRadioMenuItem(radioMenuItem, uiContext, closeFlyoutAction);
        }
        else if (menuItem is IContextToggleMenuItem toggleMenuItem)
        {
            return CreateToggleMenuItem(toggleMenuItem, uiContext, closeFlyoutAction);
        }
        else
        {
            if (Guid.TryParse(metadata.Guid, out var guid) &&
                _guidToGroups.TryGetValue(guid, out var subItemGroup))
            {
                var menuFlyoutSubItem = CreateMenuSubItem(menuItem);
                CreateMenuSubItems(subItemGroup, menuFlyoutSubItem.Items, uiContext, closeFlyoutAction);
                return menuFlyoutSubItem;
            }
            else
            {
                return CreateMenuItem(menuItem, uiContext, closeFlyoutAction);
            }
        }
    }

    /// <summary>
    /// 通过枚举的 <see cref="ContextMenuItemGroupMD"/> 组的集合中的子项，以对目标列表创建和添加 UI 子元素。
    /// </summary>
    /// <param name="groups">用于创建 UI 子元素的组的集合。</param>
    /// <param name="collection">用于将已创建的 UI 子元素的添加至目标集合的列表。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    private void CreateMenuSubItems(
        Dictionary<string, ContextMenuItemGroupMD> groups, IList<MenuFlyoutItemBase> collection,
        IContextMenuUIContext uiContext, Action? closeFlyoutAction = null)
    {
        var needSeparator = false;
        foreach (var itemGroup in groups.Values)
        {
            if (needSeparator)
                collection.Add(new MenuFlyoutSeparator());
            else
                needSeparator = true;

            foreach (var item in CollectionsMarshal.AsSpan(itemGroup.Items))
            {
                if (item.Value.IsVisible(uiContext))
                {
                    collection.Add(CreateContextMenuItem(item.Value, item.Metadata, uiContext, closeFlyoutAction));
                }
            }
        }
    }

    /// <summary>
    /// 根据传入的 <see cref="IContextMenuOptions"/> 配置选项应用至目标 <see cref="FlyoutBase"/> UI 对象。
    /// </summary>
    private static void ApplyOptionsForContextMenu(FlyoutBase flyout, IContextMenuOptions options)
    {
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 1))
        {
            flyout.Placement = options.Placement;
        }

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3))
        {
            flyout.AllowFocusOnInteraction = options.AllowFocusOnInteraction;
            flyout.AllowFocusWhenDisabled = options.AllowFocusWhenDisabled;
            flyout.LightDismissOverlayMode = options.LightDismissOverlayMode;
        }

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4))
        {
            flyout.OverlayInputPassThroughElement = options.OverlayInputPassThroughElement;
        }

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            flyout.AreOpenCloseAnimationsEnabled = options.AreOpenCloseAnimationsEnabled;
            flyout.ShowMode = options.ShowMode;
        }

        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8))
        {
            flyout.ShouldConstrainToRootBounds = options.ShouldConstrainToRootBounds;
        }
    }

    /// <summary>
    /// 内部的创建上下文菜单方法的核心实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="Guid"/> 字符串值。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <param name="callback">目标 UI 上下文的回调委托。</param>
    /// <returns>已创建的 <see cref="MenuFlyout"/> 类型实例。</returns>
    private MenuFlyout CreateContextMenuCore(
        string ownerGuidString,
        IContextMenuOptions? options = null,
        ContextMenuUIContextCallback? callback = null)
    {
        var menuFlyout = new MenuFlyout();
        if (Guid.TryParse(ownerGuidString, out var guid) &&
            _guidToGroups.TryGetValue(guid, out var groups))
        {
            menuFlyout.Opening += (s, e) =>
            {
                // 应用上下文菜单的配置选项
                if (options is not null)
                    ApplyOptionsForContextMenu(menuFlyout, options);

                // 清空列表中的所有元素
                if (menuFlyout.Items.Count > 0)
                    menuFlyout.Items.Clear();

                // 重新创建新的菜单项列表
                CreateMenuSubItems(groups, menuFlyout.Items, callback is not null
                    ? callback(menuFlyout, e) : new ContextMenuUIContext(menuFlyout, e));

                // 如果没有元素则不应显示，否则将会显示一块空白的上下文菜单。
                if (menuFlyout.Items.Count is 0)
                    menuFlyout.Hide();
            };

            menuFlyout.Closed += (s, e) =>
            {
                // 释放对图标和提示的引用
                DisposeReferences(menuFlyout.Items);
            };
        }
        else
        {
            if (options is not null)
            {
                // 如果没有枚举到菜单项，并且上下文菜单的配置选项不为空，则添加默认的应用配置选项的行为。
                menuFlyout.Opening += (s, e) => ApplyOptionsForContextMenu(menuFlyout, options);
            }
        }

        return menuFlyout;
    }

    /// <summary>
    /// 释放对上下文菜单项中的资源引用。
    /// </summary>
    /// <param name="items">需要释放的上下文菜单项集合。</param>
    private static void DisposeReferences(IEnumerable<MenuFlyoutItemBase> items)
    {
        foreach (var item in items)
        {
            if (item is MenuFlyoutItem menuFlyoutItem)
            {
                menuFlyoutItem.Icon = null;
            }
            else if (item is MenuFlyoutSubItem menuFlyoutSubItem)
            {
                menuFlyoutSubItem.Icon = null;
                DisposeReferences(menuFlyoutSubItem.Items);
            }

            ToolTipService.SetToolTip(item, null);
        }
    }
}
