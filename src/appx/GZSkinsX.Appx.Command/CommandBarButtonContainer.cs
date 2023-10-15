// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

using CommunityToolkit.Mvvm.Input;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Command;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Utilities;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace GZSkinsX.Appx.Command;

/// <summary>
/// 命令栏的按钮包装实现。
/// </summary>
internal sealed class CommandBarButtonContainer : ICommandBarItemContainer<AppBarButton>
{
    /// <summary>
    /// 当前 UI 元素所处的父容器集合对象。
    /// </summary>
    private readonly IList<ICommandBarElement> _parentContainer;

    /// <summary>
    /// 存放和托管的接口元素用于包装相关实现。
    /// </summary>
    private readonly ICommandBarButton _commandBarButton;

    /// <summary>
    /// 内部管理的 UI 对象成员。
    /// </summary>
    private readonly AppBarButton _appBarButton;

    /// <summary>
    /// 表示该 UI 对象是否已载入。
    /// </summary>
    private bool _hasLoaded;

    /// <inheritdoc/>
    public AppBarButton UIObject => _appBarButton;

    /// <summary>
    /// 初始化 <see cref="CommandBarButtonContainer"/> 的新实例。
    /// </summary>
    public CommandBarButtonContainer(IList<ICommandBarElement> parentContainer, ICommandBarButton commandBarButton)
    {
        _parentContainer = parentContainer;
        _commandBarButton = commandBarButton;
        _appBarButton = new AppBarButton();
        _appBarButton.Loaded += OnLoaded;
        _appBarButton.Unloaded += OnUnloaded;

        FixChevronPanelVisibility();
    }

    /// <summary>
    /// 修复在添加了 Flyout 元素后，不显示 '>' 箭头（展开）图标的问题。
    /// </summary>
    private void FixChevronPanelVisibility()
    {
        // Bug in https://github.com/microsoft/microsoft-ui-xaml/blob/4c50e610e537aca92afc950c4be1ffb60c2f99d5/dev/CommonStyles/AppBarButton_themeresources.xaml#L375
        // WASDK 1.4 & WinUI 3

        _appBarButton.Resources ??= new();
        _appBarButton.Resources.Add("AppBarButtonHasFlyoutChevronVisibility", Visibility.Visible);
    }

    /// <summary>
    /// 触发按钮点击的事件行为。
    /// </summary>
    private void OnClick()
    {
        _commandBarButton.OnClick();
    }

    /// <summary>
    /// 在 UI 元素载入时触发的事件行为。
    /// </summary>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_hasLoaded is false)
        {
            _commandBarButton.OnInitialize();

            var buttonContextMenu = _commandBarButton as ICommandBarButtonContextMenu;
            if (buttonContextMenu is not null && string.IsNullOrWhiteSpace(buttonContextMenu.ContextMenuGuid) is false)
                _appBarButton.Flyout = AppxContext.ContextMenuService.CreateContextMenu(buttonContextMenu.ContextMenuGuid);

            _hasLoaded = true;
        }

        _appBarButton.Command = new RelayCommand(OnClick);

        var notifyPropertyChanged = _commandBarButton as INotifyPropertyChanged;
        if (notifyPropertyChanged is not null)
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;

        UpdateUIState();
    }

    /// <summary>
    /// 在 UI 元素从父对象中移除触发的事件行为。
    /// </summary>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _appBarButton.Command = null;

        var notifyPropertyChanged = _commandBarButton as INotifyPropertyChanged;
        if (notifyPropertyChanged is not null)
            notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;

        ClearUIState();
    }

    /// <summary>
    /// 监听属性更改并同步更新至 UI 属性。
    /// </summary>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var result = _appBarButton.DispatcherQueue.TryEnqueue(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(ICommandBarButton.DisplayName):
                    UpdateDisplayName();
                    break;

                case nameof(ICommandBarButton.Icon):
                    UpdateIcon();
                    break;

                case nameof(ICommandBarButton.IsEnabled):
                    UpdateIsEnabled();
                    break;

                case nameof(ICommandBarButton.IsVisible):
                    UpdateIsVisible();
                    break;

                case nameof(ICommandBarButton.KeyboardAccelerators):
                    UpdateKeyboardAccelerators();
                    break;

                case nameof(ICommandBarButton.KeyboardAcceleratorTextOverride):
                    UpdateKeyboardAcceleratorTextOverride();
                    break;

                case nameof(ICommandBarButton.ToolTip):
                    UpdateToolTip();
                    break;

                default:
                    break;
            }
        });

        Debug.Assert(result, "Failed to enqueue the operation!");
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的显示名称。
    /// </summary>
    private void UpdateDisplayName()
    {
        var displayName = _commandBarButton.DisplayName;
        var localizedName = ResourceHelper.GetResxLocalizedOrDefault(displayName);
        AutomationProperties.SetName(_appBarButton, _appBarButton.Label = localizedName);
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的图标。
    /// </summary>
    private void UpdateIcon()
    {
        _appBarButton.Icon = _commandBarButton.Icon;
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中是否启用。
    /// </summary>
    private void UpdateIsEnabled()
    {
        _appBarButton.IsEnabled = _commandBarButton.IsEnabled;
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中是否可见。
    /// </summary>
    private void UpdateIsVisible()
    {
        _appBarButton.Visibility = BoolToVisibilityConvert.ToVisibility(_commandBarButton.IsVisible);
        _parentContainer.UpdateAllSeparatorsVisibily();
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中所绑定的键盘快捷键。
    /// </summary>
    private void UpdateKeyboardAccelerators()
    {
        var collection = _appBarButton.KeyboardAccelerators;
        var keyboardAccelerators = _commandBarButton.KeyboardAccelerators.OfType<KeyboardAccelerator>();

        collection.Clear();
        foreach (var accelerator in keyboardAccelerators)
            collection.Add(accelerator);
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的键盘快捷键替代字符串。
    /// </summary>
    private void UpdateKeyboardAcceleratorTextOverride()
    {
        var textOverride = _commandBarButton.KeyboardAcceleratorTextOverride;
        _appBarButton.KeyboardAcceleratorTextOverride = ResourceHelper.GetResxLocalizedOrDefault(textOverride);
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的提示信息。
    /// </summary>
    private void UpdateToolTip()
    {
        var toolTip = _commandBarButton.ToolTip;
        ToolTipService.SetToolTip(_appBarButton, toolTip is string str ?
            ResourceHelper.GetResxLocalizedOrDefault(str) : toolTip);
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的状态。
    /// </summary>
    private void UpdateUIState()
    {
        UpdateDisplayName();
        UpdateIcon();
        UpdateIsEnabled();
        UpdateIsVisible();
        UpdateKeyboardAccelerators();
        UpdateKeyboardAcceleratorTextOverride();
        UpdateToolTip();
    }

    /// <summary>
    /// 清除/还原所有的 UI 状态。
    /// </summary>
    private void ClearUIState()
    {
        _appBarButton.ClearValue(AutomationProperties.NameProperty);
        _appBarButton.ClearValue(AppBarButton.LabelProperty);
        _appBarButton.ClearValue(AppBarButton.IconProperty);
        _appBarButton.ClearValue(Control.IsEnabledProperty);
        _appBarButton.ClearValue(UIElement.VisibilityProperty);
        _appBarButton.ClearValue(ToolTipService.ToolTipProperty);

        _appBarButton.KeyboardAccelerators.Clear();
        _appBarButton.ClearValue(AppBarButton.KeyboardAcceleratorTextOverrideProperty);
    }
}
