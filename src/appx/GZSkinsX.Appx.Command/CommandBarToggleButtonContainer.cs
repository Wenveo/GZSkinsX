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

using GZSkinsX.Contracts.Command;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Utilities;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace GZSkinsX.Appx.Command;

/// <summary>
/// 命令栏的开关按钮包装实现。
/// </summary>
internal sealed class CommandBarToggleButtonContainer : ICommandBarItemContainer<AppBarToggleButton>
{
    /// <summary>
    /// 当前 UI 元素所处的父容器集合对象。
    /// </summary>
    private readonly IList<ICommandBarElement> _parentContainer;

    /// <summary>
    /// 存放和托管的接口元素用于包装相关实现。
    /// </summary>
    private readonly ICommandBarToggleButton _commandBarToggleButton;

    /// <summary>
    /// 存放与命令栏所关联的 UI 上下文。
    /// </summary>
    private readonly ICommandBarUIContext? _uiContext;

    /// <summary>
    /// 内部管理的 UI 对象成员。
    /// </summary>
    private readonly AppBarToggleButton _appBarToggleButton;

    /// <summary>
    /// 表示该 UI 对象是否已载入。
    /// </summary>
    private bool _hasLoaded;

    /// <inheritdoc/>
    public AppBarToggleButton UIObject => _appBarToggleButton;

    /// <summary>
    /// 初始化 <see cref="CommandBarToggleButtonContainer"/> 的新实例。
    /// </summary>
    public CommandBarToggleButtonContainer(IList<ICommandBarElement> parentContainer,
        ICommandBarToggleButton commandBarToggleButton, ICommandBarUIContext? uiContext)
    {
        _uiContext = uiContext;
        _parentContainer = parentContainer;
        _commandBarToggleButton = commandBarToggleButton;
        _appBarToggleButton = new AppBarToggleButton();
        _appBarToggleButton.Loaded += OnLoaded;
        _appBarToggleButton.Unloaded += OnUnloaded;
        _uiContext = uiContext;
    }

    /// <summary>
    /// 触发按钮点击的事件行为。
    /// </summary>
    private void OnClick()
    {
        _commandBarToggleButton.OnClick(_uiContext);
    }

    /// <summary>
    /// 在切换至选中状态触发的事件行为。
    /// </summary>
    private void OnChecked(object sender, RoutedEventArgs e)
    {
        _commandBarToggleButton.OnChecked(_uiContext);
    }

    /// <summary>
    /// 在切换至未选中状态触发的事件行为。
    /// </summary>
    private void OnUnchecked(object sender, RoutedEventArgs e)
    {
        _commandBarToggleButton.OnUnchecked(_uiContext);
    }

    /// <summary>
    /// 在 UI 元素载入时触发的事件行为。
    /// </summary>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_hasLoaded is false)
        {
            _commandBarToggleButton.OnInitialize(_uiContext);
            _hasLoaded = true;
        }

        _appBarToggleButton.Command = new RelayCommand(OnClick);
        _appBarToggleButton.Checked += OnChecked;
        _appBarToggleButton.Unchecked += OnUnchecked;
        _commandBarToggleButton.OnLoaded(_uiContext);

        var notifyPropertyChanged = _commandBarToggleButton as INotifyPropertyChanged;
        if (notifyPropertyChanged is not null)
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;

        UpdateUIState();
    }

    /// <summary>
    /// 在 UI 元素从父对象中移除触发的事件行为。
    /// </summary>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _appBarToggleButton.Command = null;
        _appBarToggleButton.Checked -= OnChecked;
        _appBarToggleButton.Unchecked -= OnUnchecked;
        _commandBarToggleButton.OnUnloaded(_uiContext);

        var notifyPropertyChanged = _commandBarToggleButton as INotifyPropertyChanged;
        if (notifyPropertyChanged is not null)
            notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;

        ClearUIState();
    }

    /// <summary>
    /// 监听属性更改并同步更新至 UI 属性。
    /// </summary>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var result = _appBarToggleButton.DispatcherQueue.TryEnqueue(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(ICommandBarToggleButton.DisplayName):
                    UpdateDisplayName();
                    break;

                case nameof(ICommandBarToggleButton.Icon):
                    UpdateIcon();
                    break;

                case nameof(ICommandBarToggleButton.IsChecked):
                    UpdateIsChecked();
                    break;

                case nameof(ICommandBarToggleButton.IsEnabled):
                    UpdateIsEnabled();
                    break;

                case nameof(ICommandBarToggleButton.IsVisible):
                    UpdateIsVisible();
                    break;

                case nameof(ICommandBarToggleButton.KeyboardAccelerators):
                    UpdateKeyboardAccelerators();
                    break;

                case nameof(ICommandBarToggleButton.KeyboardAcceleratorTextOverride):
                    UpdateKeyboardAcceleratorTextOverride();
                    break;

                case nameof(ICommandBarToggleButton.ToolTip):
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
        var displayName = _commandBarToggleButton.DisplayName;
        var localizedName = ResourceHelper.GetResxLocalizedOrDefault(displayName);
        AutomationProperties.SetName(_appBarToggleButton, _appBarToggleButton.Label = localizedName);
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的图标。
    /// </summary>
    private void UpdateIcon()
    {
        _appBarToggleButton.Icon = _commandBarToggleButton.Icon;
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中是否为选中状态。
    /// </summary>
    private void UpdateIsChecked()
    {
        _appBarToggleButton.IsChecked = _commandBarToggleButton.IsChecked;
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中是否启用。
    /// </summary>
    private void UpdateIsEnabled()
    {
        _appBarToggleButton.IsEnabled = _commandBarToggleButton.IsEnabled;
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中是否可见。
    /// </summary>
    private void UpdateIsVisible()
    {
        _appBarToggleButton.Visibility = BoolToVisibilityConvert.ToVisibility(_commandBarToggleButton.IsVisible);
        _parentContainer.UpdateAllSeparatorsVisibily();
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中所绑定的键盘快捷键。
    /// </summary>
    private void UpdateKeyboardAccelerators()
    {
        var collection = _appBarToggleButton.KeyboardAccelerators;
        var keyboardAccelerators = _commandBarToggleButton.KeyboardAccelerators.OfType<KeyboardAccelerator>();

        collection.Clear();
        foreach (var accelerator in keyboardAccelerators)
            collection.Add(accelerator);
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的键盘快捷键替代字符串。
    /// </summary>
    private void UpdateKeyboardAcceleratorTextOverride()
    {
        var textOverride = _commandBarToggleButton.KeyboardAcceleratorTextOverride;
        _appBarToggleButton.KeyboardAcceleratorTextOverride = ResourceHelper.GetResxLocalizedOrDefault(textOverride);
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的提示信息。
    /// </summary>
    private void UpdateToolTip()
    {
        var toolTip = _commandBarToggleButton.ToolTip;
        ToolTipService.SetToolTip(_appBarToggleButton, toolTip is string str ?
            ResourceHelper.GetResxLocalizedOrDefault(str) : toolTip);
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的状态。
    /// </summary>
    private void UpdateUIState()
    {
        UpdateDisplayName();
        UpdateIcon();
        UpdateIsChecked();
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
        _appBarToggleButton.ClearValue(AutomationProperties.NameProperty);
        _appBarToggleButton.ClearValue(AppBarToggleButton.LabelProperty);
        _appBarToggleButton.ClearValue(AppBarToggleButton.IconProperty);
        _appBarToggleButton.ClearValue(ToggleButton.IsCheckedProperty);
        _appBarToggleButton.ClearValue(Control.IsEnabledProperty);
        _appBarToggleButton.ClearValue(UIElement.VisibilityProperty);
        _appBarToggleButton.ClearValue(ToolTipService.ToolTipProperty);

        _appBarToggleButton.KeyboardAccelerators.Clear();
        _appBarToggleButton.ClearValue(AppBarToggleButton.KeyboardAcceleratorTextOverrideProperty);
    }
}
