// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using GZSkinsX.Contracts.Command;
using GZSkinsX.Contracts.Utilities;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Command;

/// <summary>
/// 命令栏的自定义元素包装实现。
/// </summary>
internal sealed class CommandBarObjectContainer : ICommandBarItemContainer<AppBarElementContainer>
{
    /// <summary>
    /// 当前 UI 元素所处的父容器集合对象。
    /// </summary>
    private readonly IList<ICommandBarElement> _parentContainer;

    /// <summary>
    /// 存放和托管的接口元素用于包装相关实现。
    /// </summary>
    private readonly ICommandBarObject _commandBarObject;

    /// <summary>
    /// 存放与命令栏所关联的 UI 上下文。
    /// </summary>
    private readonly ICommandBarUIContext _uiContext;

    /// <summary>
    /// 内部管理的 UI 对象成员。
    /// </summary>
    private readonly AppBarElementContainer _appBarElementContainer;

    /// <summary>
    /// 表示该 UI 对象是否已载入。
    /// </summary>
    private bool _hasLoaded;

    /// <inheritdoc/>
    public AppBarElementContainer UIObject => _appBarElementContainer;

    /// <summary>
    /// 初始化 <see cref="CommandBarObjectContainer"/> 的新实例。
    /// </summary>
    public CommandBarObjectContainer(IList<ICommandBarElement> parentContainer,
        ICommandBarObject commandBarObject, ICommandBarUIContext uiContext)
    {
        _uiContext = uiContext;
        _parentContainer = parentContainer;
        _commandBarObject = commandBarObject;
        _appBarElementContainer = new AppBarElementContainer { DataContext = this };
        _appBarElementContainer.DispatcherQueue.TryEnqueue(EnsureInitialize);
    }

    /// <summary>
    /// 确认是否已初始化。
    /// </summary>
    private void EnsureInitialize()
    {
        if (_hasLoaded is false)
        {
            _commandBarObject.OnInitialize(_uiContext);

            _hasLoaded = true;
            UpdateUIState();
        }
    }

    /// <inheritdoc/>
    public void OnLoaded()
    {
        EnsureInitialize();

        _commandBarObject.OnLoaded(_uiContext);

        var notifyPropertyChanged = _commandBarObject as INotifyPropertyChanged;
        if (notifyPropertyChanged is not null)
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;

        UpdateUIState();
    }

    /// <inheritdoc/>
    public void OnUnloaded()
    {
        _commandBarObject.OnUnloaded(_uiContext);

        var notifyPropertyChanged = _commandBarObject as INotifyPropertyChanged;
        if (notifyPropertyChanged is not null)
            notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;

        ClearUIState();
    }

    /// <summary>
    /// 监听属性更改并同步更新至 UI 属性。
    /// </summary>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var result = _appBarElementContainer.DispatcherQueue.TryEnqueue(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(ICommandBarObject.IsEnabled):
                    UpdateIsEnabled();
                    break;

                case nameof(ICommandBarObject.IsVisible):
                    UpdateIsVisible();
                    break;

                case nameof(ICommandBarObject.UIObject):
                    UpdateUIObject();
                    break;

                default:
                    break;
            }
        });

        Debug.Assert(result, "Failed to enqueue the operation!");
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中是否启用。
    /// </summary>
    private void UpdateIsEnabled()
    {
        _appBarElementContainer.IsEnabled = _commandBarObject.IsEnabled;
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中是否可见。
    /// </summary>
    private void UpdateIsVisible()
    {
        _appBarElementContainer.Visibility = BoolToVisibilityConvert.ToVisibility(_commandBarObject.IsVisible);
        _parentContainer.UpdateAllSeparatorsVisibily();
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中显示的对象。
    /// </summary>
    private void UpdateUIObject()
    {
        _appBarElementContainer.Content = _commandBarObject.UIObject;
    }

    /// <summary>
    /// 更新此按钮元素在 UI 中的状态。
    /// </summary>
    private void UpdateUIState()
    {
        UpdateIsEnabled();
        UpdateIsVisible();
        UpdateUIObject();
    }

    /// <summary>
    /// 清除/还原所有的 UI 状态。
    /// </summary>
    private void ClearUIState()
    {
        _appBarElementContainer.ClearValue(Control.IsEnabledProperty);
        _appBarElementContainer.ClearValue(UIElement.VisibilityProperty);
        _appBarElementContainer.ClearValue(ContentControl.ContentProperty);
    }
}
