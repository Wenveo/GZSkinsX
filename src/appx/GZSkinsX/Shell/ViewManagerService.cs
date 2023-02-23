// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;
using System.Diagnostics;

using GZSkinsX.Api.Shell;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace GZSkinsX.Shell;

/// <summary>
/// 视图管理服务的内部接口定义
/// </summary>
internal interface IViewManagerServiceImpl : IViewManagerService
{
    /// <summary>
    /// 初始化当前内部 <see cref="Frame"/> 对象
    /// </summary>
    /// <returns>内部对象实例，且仅会存在一个实例对象</returns>
    Frame InitializeFrame();
}


/// <inheritdoc cref="IViewManagerService"/>
[Shared]
[Export(typeof(IViewManagerServiceImpl)), Export(typeof(IViewManagerService))]
internal sealed class ViewManagerService : IViewManagerServiceImpl
{
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

    /// <summary>
    /// 初始化 <see cref="ViewManagerService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public ViewManagerService([ImportMany] IEnumerable<Lazy<IViewElement, ViewElementMetadataAttribute>> viewElements)
    {
        _viewElements = viewElements;

        _frame = new Frame();
        _guidToViewElement = new Dictionary<Guid, ViewElementContext>();
    }

    /// <inheritdoc/>
    public Frame InitializeFrame()
    {
        if (!_isInitialize)
        {
            foreach (var elem in _viewElements)
            {
                var guidString = elem.Metadata.Guid;
                var b = Guid.TryParse(guidString, out var guid);
                Debug.Assert(b, $"ViewManagerService: Couldn't parse Guid property: '{guidString}'");
                if (!b)
                    continue;

                var pageType = elem.Metadata.PageType;
                b = pageType.BaseType != null;
                Debug.Assert(b, $"ViewManagerService: The PageType is not inherite by Page: '{pageType.BaseType}'");
                if (!b)
                    continue;

                b = pageType.BaseType == typeof(Page);
                Debug.Assert(b, $"ViewManagerService: Invaild PageType: '{pageType.BaseType}'");
                if (!b)
                    continue;

                _guidToViewElement[guid] = new ViewElementContext(elem);
            }

            _isInitialize = true;
        }

        return _frame;
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
            var b = _frame.Navigate(elem.Metadata.PageType);
            if (b && elem.Value is IViewElementLoader controller)
            {
                controller.OnInitialized((Page)_frame.Content);
            }
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(Guid elemGuid, object parameter)
    {
        if (_guidToViewElement.TryGetValue(elemGuid, out var elem))
        {
            var b = _frame.Navigate(elem.Metadata.PageType, parameter);
            if (b && elem.Value is IViewElementLoader controller)
            {
                controller.OnInitialized((Page)_frame.Content);
            }
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(Guid elemGuid, object parameter, NavigationTransitionInfo infoOverride)
    {
        if (_guidToViewElement.TryGetValue(elemGuid, out var elem))
        {
            var b = _frame.Navigate(elem.Metadata.PageType, parameter, infoOverride);
            if (b && elem.Value is IViewElementLoader controller)
            {
                controller.OnInitialized((Page)_frame.Content);
            }
        }
    }
}
