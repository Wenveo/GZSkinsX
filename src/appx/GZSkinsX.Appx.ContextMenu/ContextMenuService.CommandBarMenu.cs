// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.Input;

using GZSkinsX.Contracts.ContextMenu;

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace GZSkinsX.Appx.ContextMenu;

partial class ContextMenuService
{
    /// <summary>
    /// 互斥的菜单项到可切换状态的菜单项的包装转换实现。
    /// </summary>
    /// <param name="radioMenuItem">需要包装的菜单项。</param>
    private sealed class RadioMenuItemAsToggleMenuItem(IContextRadioMenuItem radioMenuItem) : IContextToggleMenuItem
    {
        /// <inheritdoc/>
        public string? Header => radioMenuItem.Header;

        /// <inheritdoc/>
        public IconElement? Icon => radioMenuItem.Icon;

        /// <inheritdoc/>
        public IEnumerable<KeyboardAccelerator> KeyboardAccelerators => radioMenuItem.KeyboardAccelerators;

        /// <inheritdoc/>
        public string? KeyboardAcceleratorTextOverride => radioMenuItem.KeyboardAcceleratorTextOverride;

        /// <inheritdoc/>
        public object? ToolTip => radioMenuItem.ToolTip;

        /// <inheritdoc/>
        public bool IsChecked(IContextMenuUIContext context) => radioMenuItem.IsChecked(context);

        /// <inheritdoc/>
        public bool IsEnabled(IContextMenuUIContext context) => radioMenuItem.IsEnabled(context);

        /// <inheritdoc/>
        public bool IsVisible(IContextMenuUIContext context) => radioMenuItem.IsVisible(context);

        /// <inheritdoc/>
        public void OnClick(bool isChecked, IContextMenuUIContext context) => radioMenuItem.OnClick(isChecked, context);

        /// <inheritdoc/>
        public void OnExecute(IContextMenuUIContext context) => radioMenuItem.OnExecute(context);
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象创建一个新的 <see cref="AppBarButton"/> 实例。
    /// </summary>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    /// <returns>已创建的 <see cref="AppBarButton"/> 实例。</returns>
    private static AppBarButton CreateAppBarButton(
        IContextMenuItem menuItem,
        IContextMenuUIContext uiContext,
        Action? closeFlyoutAction = null)
    {
        var uiObject = new AppBarButton
        {
            Command = new RelayCommand(() =>
            {
                closeFlyoutAction?.Invoke();
                menuItem.OnExecute(uiContext);
            }),
            IsEnabled = menuItem.IsEnabled(uiContext)
        };

        var icon = menuItem.Icon;
        if (icon is not null)
            uiObject.Icon = icon;

        var header = menuItem.Header;
        if (header is not null)
            AutomationProperties.SetName(uiObject, uiObject.Label = header);

        var toolTip = menuItem.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(uiObject, toolTip);

        foreach (var keyboardAccelerator in menuItem.KeyboardAccelerators.OfType<KeyboardAccelerator>())
            uiObject.KeyboardAccelerators.Add(keyboardAccelerator);

        var textOverride = menuItem.KeyboardAcceleratorTextOverride;
        if (string.IsNullOrWhiteSpace(textOverride) is false)
            uiObject.KeyboardAcceleratorTextOverride = textOverride;

        return uiObject;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象创建一个新的 <see cref="AppBarToggleButton"/> 实例。
    /// </summary>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    /// <returns>已创建的 <see cref="AppBarToggleButton"/> 实例。</returns>
    private static AppBarToggleButton CreateAppBarToggleButton(
        IContextToggleMenuItem menuItem,
        IContextMenuUIContext uiContext,
        Action? closeFlyoutAction = null)
    {
        var uiObject = new AppBarToggleButton
        {
            IsEnabled = menuItem.IsEnabled(uiContext),
            IsChecked = menuItem.IsChecked(uiContext)
        };
        uiObject.Command = new RelayCommand(() =>
        {
            closeFlyoutAction?.Invoke();
            menuItem.OnClick(uiObject.IsChecked is true, uiContext);
            menuItem.OnExecute(uiContext);
        });

        var icon = menuItem.Icon;
        if (icon is not null)
            uiObject.Icon = icon;

        var header = menuItem.Header;
        if (header is not null)
            AutomationProperties.SetName(uiObject, uiObject.Label = header);

        var toolTip = menuItem.ToolTip;
        if (toolTip is not null)
            ToolTipService.SetToolTip(uiObject, toolTip);

        foreach (var keyboardAccelerator in menuItem.KeyboardAccelerators.OfType<KeyboardAccelerator>())
            uiObject.KeyboardAccelerators.Add(keyboardAccelerator);

        var textOverride = menuItem.KeyboardAcceleratorTextOverride;
        if (string.IsNullOrWhiteSpace(textOverride) is false)
            uiObject.KeyboardAcceleratorTextOverride = textOverride;

        return uiObject;
    }

    /// <summary>
    /// 通过传入的 <see cref="IContextMenuItem"/> 对象和 <see cref="ContextMenuItemContractAttribute"/> 元数据创建一个新的 UI 对象实例。
    /// </summary>
    /// <param name="menuItem">用于创建的上下文菜单项。</param>
    /// <param name="metadata">与上下文菜单项相关联的元数据。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    /// <returns>已创建的 UI 对象实例。</returns>
    private ICommandBarElement CreateCommandBarItem(
        IContextMenuItem menuItem, ContextMenuItemContractAttribute metadata,
        IContextMenuUIContext uiContext, ICommandBarMenuOptions? options = null,
        Action? closeFlyoutAction = null)
    {
        if (menuItem is IContextMenuItemProvider provider)
        {
            var menuFlyout = new MenuFlyout();
            menuFlyout.Opening += (s, e) =>
            {
                if (options is not null && options.SubMenuOptions is not null)
                    ApplyOptionsForContextMenu(menuFlyout, options.SubMenuOptions);

                if (menuFlyout.Items.Count > 0)
                    menuFlyout.Items.Clear();

                // 从目标方法的返回值中枚举上下文菜单项集合并创建。
                foreach (var subItem in provider.CreateSubItems())
                {
                    if (subItem.IsEmpty)
                        menuFlyout.Items.Add(new MenuFlyoutSeparator());
                    else
                        menuFlyout.Items.Add(CreateContextMenuItem(
                            subItem.ContextMenuItem, subItem.Metadata, uiContext, closeFlyoutAction));
                }

                if (menuFlyout.Items.Count is 0)
                    menuFlyout.Hide();
            };

            // 释放对图标和提示的引用
            menuFlyout.Closed += (s, e) => DisposeReferences(menuFlyout.Items);

            var appBarButton = CreateAppBarButton(menuItem, uiContext);
            appBarButton.Flyout = menuFlyout;
            return appBarButton;
        }
        else if (menuItem is IContextRadioMenuItem radioMenuItem)
        {
            return CreateAppBarToggleButton(new RadioMenuItemAsToggleMenuItem(radioMenuItem), uiContext, closeFlyoutAction);
        }
        else if (menuItem is IContextToggleMenuItem toggleMenuItem)
        {
            return CreateAppBarToggleButton(toggleMenuItem, uiContext, closeFlyoutAction);
        }
        else if (Guid.TryParse(metadata.Guid, out var guid) &&
                _guidToGroups.TryGetValue(guid, out var subItemGroup))
        {
            var menuFlyout = new MenuFlyout();
            menuFlyout.Opening += (s, e) =>
            {
                if (menuFlyout.Items.Count > 0)
                    menuFlyout.Items.Clear();

                if (options is not null && options.SubMenuOptions is not null)
                    ApplyOptionsForContextMenu(menuFlyout, options.SubMenuOptions);

                CreateMenuSubItems(subItemGroup, menuFlyout.Items, uiContext, closeFlyoutAction);

                if (menuFlyout.Items.Count is 0)
                    menuFlyout.Hide();
            };

            // 释放对图标和提示的引用
            menuFlyout.Closed += (s, e) => DisposeReferences(menuFlyout.Items);

            var appBarButton = CreateAppBarButton(menuItem, uiContext);
            appBarButton.Flyout = menuFlyout;
            return appBarButton;
        }
        else
        {
            return CreateAppBarButton(menuItem, uiContext, closeFlyoutAction);
        }
    }

    /// <summary>
    /// 通过枚举的 <see cref="ContextMenuItemGroupMD"/> 组的集合中的子项，以对目标列表创建和添加 UI 子元素。
    /// </summary>
    /// <param name="itemGroups">用于创建 UI 子元素的组的集合。</param>
    /// <param name="primaryCommands">用于将已创建的 UI 子元素的添加至目标集合的主列表。</param>
    /// <param name="secondaryCommands">用于将已创建的 UI 子元素的添加至目标集合的次要列表。</param>
    /// <param name="uiContext">上下文菜单所关联的 UI 上下文内容。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <param name="closeFlyoutAction">关闭浮出控件的行为。</param>
    private void CreateCommandBarSubItems(Dictionary<string, ContextMenuItemGroupMD> itemGroups,
        IList<ICommandBarElement> primaryCommands, IList<ICommandBarElement> secondaryCommands,
        IContextMenuUIContext uiContext, ICommandBarMenuOptions? options = null, Action? closeFlyoutAction = null)
    {
        var primaryNeedSeparator = false;
        var secondaryNeedSeparator = false;

        foreach (var group in itemGroups.Values)
        {
            var visibleElements = group.Items.Where(item => item.Value.IsVisible(uiContext));
            foreach (var item in visibleElements.Where(item => item.Value is not ISecondaryCommandBarItem))
            {
                if (primaryNeedSeparator)
                    primaryCommands.Add(new AppBarSeparator());
                else
                    primaryNeedSeparator = true;

                primaryCommands.Add(
                    CreateCommandBarItem(item.Value, item.Metadata,
                        uiContext, options, closeFlyoutAction));
            }

            foreach (var item in visibleElements.Where(item => item.Value is ISecondaryCommandBarItem))
            {
                if (secondaryNeedSeparator)
                    secondaryCommands.Add(new AppBarSeparator());
                else
                    secondaryNeedSeparator = true;

                secondaryCommands.Add(
                    CreateCommandBarItem(item.Value, item.Metadata,
                        uiContext, options, closeFlyoutAction));
            }
        }
    }

    /// <summary>
    /// 根据传入的 <see cref="IContextMenuOptions"/> 配置选项应用至目标 <see cref="CommandBarFlyout"/> UI 对象。
    /// </summary>
    private static void ApplyOptionsForCommandBarMenu(CommandBarFlyout commandBarFlyout, ICommandBarMenuOptions options)
    {
        ApplyOptionsForContextMenu(commandBarFlyout, options);
        commandBarFlyout.AlwaysExpanded = options.AlwaysExpanded;
    }

    /// <summary>
    /// 内部的创建上下文命令菜单方法的核心实现。
    /// </summary>
    /// <param name="ownerGuidString">子菜单项所归属的 <see cref="Guid"/> 字符串值。</param>
    /// <param name="options">需要应用到 UI 上下文菜单上的属性配置选项。</param>
    /// <param name="callback">目标 UI 上下文的回调委托。</param>
    /// <returns>已创建的 <see cref="CommandBarFlyout"/> 类型实例。</returns>
    private CommandBarFlyout CreateCommandBarMenuCore(
        string ownerGuidString,
        ICommandBarMenuOptions? options = null,
        ContextMenuUIContextCallback? callback = null)
    {
        var commandBarFlyout = new CommandBarFlyout();
        if (Guid.TryParse(ownerGuidString, out var guid) &&
            _guidToGroups.TryGetValue(guid, out var groups))
        {
            commandBarFlyout.Opening += (s, e) =>
            {
                // 应用上下文菜单的配置选项
                if (options is not null)
                    ApplyOptionsForCommandBarMenu(commandBarFlyout, options);

                // 定义对列表的引用变量
                var primaryCommands = commandBarFlyout.PrimaryCommands;
                var secondaryCommands = commandBarFlyout.SecondaryCommands;

                // 清空列表中的所有元素
                if (primaryCommands.Count > 0)
                    primaryCommands.Clear();

                if (secondaryCommands.Count > 0)
                    secondaryCommands.Clear();

                // 重新创建新的菜单项列表
                CreateCommandBarSubItems(groups, primaryCommands, secondaryCommands, callback is not null ?
                    callback(commandBarFlyout, e) : new ContextMenuUIContext(commandBarFlyout, e), options, () => commandBarFlyout.Hide());

                // 如果没有元素则不应显示，否则将会显示一块空白的上下文菜单。
                if (primaryCommands.Count is 0 && secondaryCommands.Count is 0)
                    commandBarFlyout.Hide();
            };

            commandBarFlyout.Closed += (s, e) =>
            {
                // 释放对图标和提示的引用
                DisposeReferences(commandBarFlyout.PrimaryCommands);
                DisposeReferences(commandBarFlyout.SecondaryCommands);
            };
        }
        else
        {
            if (options is not null)
            {
                // 如果没有枚举到菜单项，并且上下文菜单的配置选项不为空，则添加默认的应用配置选项的行为。
                commandBarFlyout.Opening += (s, e) => ApplyOptionsForCommandBarMenu(commandBarFlyout, options);
            }
        }

        return commandBarFlyout;
    }

    /// <summary>
    /// 释放对上下文命令菜单项中的资源引用。
    /// </summary>
    /// <param name="items">需要释放的上下文命令菜单项的元素集合。</param>
    private static void DisposeReferences(IEnumerable<ICommandBarElement> elements)
    {
        foreach (var item in elements)
        {
            if (item is AppBarButton appBarButton)
            {
                appBarButton.Icon = null;
                ToolTipService.SetToolTip(appBarButton, null);
            }
            else if (item is AppBarToggleButton appBarToggleButton)
            {
                appBarToggleButton.Icon = null;
                ToolTipService.SetToolTip(appBarToggleButton, null);
            }
        }
    }
}
