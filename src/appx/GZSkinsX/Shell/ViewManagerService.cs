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

using GZSkinsX.Api;
using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Shell;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace GZSkinsX.Shell;

/// <inheritdoc cref="IViewManagerService"/>
[Shared, Export(typeof(IViewManagerService))]
internal sealed class ViewManagerService : IViewManagerService
{
    /// <summary>
    /// 当前应用程序主窗口
    /// </summary>
    private readonly IAppxWindow _appxWindow;

    /// <summary>
    /// 存放所有已导出的 <see cref="IViewElement"/> 类型对象
    /// </summary>
    private readonly IEnumerable<Lazy<IViewElement, ViewElementMetadataAttribute>> _viewElements;

    /// <summary>
    /// 使用 <see cref="Guid"/> 作为 Key 并存储所有 <see cref="IViewElement"/> 上下文对象
    /// </summary>
    private readonly Dictionary<Guid, ViewElementContext> _guidToViewElement;

    /// <summary>
    /// 用于导航的内部定义控件
    /// </summary>
    private readonly Frame _frame;

    /// <summary>
    /// 判断当前是否已初始化
    /// </summary>
    private bool _isInitialize;

    /// <inheritdoc/>
    public bool CanGoBack => _frame.CanGoBack;

    /// <inheritdoc/>
    public bool CanGoForward => _frame.CanGoForward;

    /// <inheritdoc/>
    public Frame Frame => _frame;

    /// <summary>
    /// 初始化 <see cref="ViewManagerService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public ViewManagerService(IAppxWindow appxWindow, [ImportMany] IEnumerable<Lazy<IViewElement, ViewElementMetadataAttribute>> viewElements)
    {
        _appxWindow = appxWindow;
        _viewElements = viewElements;

        _frame = new Frame();
        _guidToViewElement = new Dictionary<Guid, ViewElementContext>();

        InitializeContext();
    }

    /// <summary>
    /// 初始化上下文对象
    /// </summary>
    public void InitializeContext()
    {
        if (!_isInitialize)
        {
            foreach (var elem in _viewElements)
            {
                var guidString = elem.Metadata.Guid;
                var b = Guid.TryParse(guidString, out var guid);
                Debug.Assert(b, $"WindowManagerService: Couldn't parse Guid property: '{guidString}'");
                if (!b)
                    continue;

                var pageType = elem.Metadata.PageType;
                b = pageType.BaseType != null;
                Debug.Assert(b, $"WindowManagerService: The PageType is not inherite by Page: '{pageType.BaseType}'");
                if (!b)
                    continue;

                b = pageType.BaseType == typeof(Page);
                Debug.Assert(b, $"WindowManagerService: Invaild PageType: '{pageType.BaseType}'");
                if (!b)
                    continue;

                _guidToViewElement[guid] = new ViewElementContext(elem);
            }

            _appxWindow.MainWindow.Content = _frame;
            _isInitialize = true;
        }
    }

    /// <inheritdoc/>
    public void GoBack()
    {
        if (_frame.CanGoBack)
        {
            _frame.GoBack();
        }
    }

    /// <inheritdoc/>
    public void GoForward()
    {
        if (_frame.CanGoForward)
        {
            _frame.GoForward();
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            NavigateTo(guid);
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(string guidString, object parameter)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            NavigateTo(guid, parameter);
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(string guidString, object parameter, NavigationTransitionInfo infoOverride)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            NavigateTo(guid, parameter, infoOverride);
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(Guid elemGuid)
    {
        if (_guidToViewElement.TryGetValue(elemGuid, out var elem))
        {
            NavigateCore(elem, null, null);
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(Guid elemGuid, object parameter)
    {
        if (_guidToViewElement.TryGetValue(elemGuid, out var elem))
        {
            NavigateCore(elem, parameter, null);
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(Guid elemGuid, object parameter, NavigationTransitionInfo infoOverride)
    {
        if (_guidToViewElement.TryGetValue(elemGuid, out var elem))
        {
            NavigateCore(elem, parameter, infoOverride);
        }
    }

    private async void NavigateCore(ViewElementContext context, object? parameter, NavigationTransitionInfo? infoOverride)
    {
        infoOverride ??= new DrillInNavigationTransitionInfo();
        if (context.Value is IViewElementLoader loader)
        {
            var args = new WindowFrameNavigateEventArgs();
            loader.OnNavigating(args);

            if (args.Handled)
                return;

            _frame.Navigate(context.Metadata.PageType, parameter, infoOverride);
            loader.OnInitialize((Page)_frame.Content);
        }
        else if (context.Value is IViewElementLoaderAsync loaderAsync)
        {
            var args = new WindowFrameNavigateEventArgs();
            await loaderAsync.OnNavigatingAsync(args);

            if (args.Handled)
                return;

            _frame.Navigate(context.Metadata.PageType, parameter, infoOverride);
            await loaderAsync.OnInitializeAsync((Page)_frame.Content);
        }
        else
        {
            _frame.Navigate(context.Metadata.PageType, parameter, infoOverride);
        }
    }
}
