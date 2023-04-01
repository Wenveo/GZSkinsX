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
using System.Threading.Tasks;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.WindowManager;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace GZSkinsX.WindowManager;

/// <inheritdoc cref="IWindowManagerService"/>
[Shared, Export(typeof(IWindowManagerService))]
internal sealed class WindowManagerService : IWindowManagerService
{
    /// <summary>
    /// 当前应用程序主窗口
    /// </summary>
    private readonly IAppxWindow _appxWindow;

    /// <summary>
    /// 存放所有已导出的 <see cref="IWindowFrame"/> 类型对象
    /// </summary>
    private readonly IEnumerable<Lazy<IWindowFrame, WindowFrameMetadataAttribute>> _viewElements;

    /// <summary>
    /// 使用 <see cref="Guid"/> 作为 Key 并存储所有 <see cref="IWindowFrame"/> 上下文对象
    /// </summary>
    private readonly Dictionary<Guid, WindowFrameContext> _guidToWindowFrame;

    /// <summary>
    /// 用于导航的内部定义控件
    /// </summary>
    private readonly Frame _frame;

    /// <summary>
    /// 初始化 <see cref="WindowManagerService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public WindowManagerService(IAppxWindow appxWindow, [ImportMany] IEnumerable<Lazy<IWindowFrame, WindowFrameMetadataAttribute>> viewElements)
    {
        _appxWindow = appxWindow;
        _viewElements = viewElements;

        _frame = new();
        _guidToWindowFrame = new();

        InitializeContext();
    }

    /// <summary>
    /// 初始化上下文对象
    /// </summary>
    public void InitializeContext()
    {
        if (_appxWindow.MainWindow.Content is not Frame frame || frame != _frame)
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

                _guidToWindowFrame[guid] = new WindowFrameContext(elem);
            }

            _frame.Navigated += OnNavigated;
            _appxWindow.MainWindow.Content = _frame;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private async void OnNavigated(object sender, NavigationEventArgs e)
    {
        var frameGuid = (Guid)_frame.Tag;
        if (_guidToWindowFrame.TryGetValue(frameGuid, out var context))
        {
            await context.Value.OnNavigateToAsync(e);
        }
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            await NavigateCoreAsync(guid, null, null);
        }
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString, object parameter)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            await NavigateCoreAsync(guid, parameter, null);
        }
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString, object parameter, NavigationTransitionInfo infoOverride)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            await NavigateCoreAsync(guid, parameter, infoOverride);
        }
    }

    /// <inheritdoc/>
    public async void NavigateTo(Guid frameGuid)
    {
        await NavigateCoreAsync(frameGuid, null, null);
    }

    /// <inheritdoc/>
    public async void NavigateTo(Guid frameGuid, object parameter)
    {
        await NavigateCoreAsync(frameGuid, parameter, null);
    }

    /// <inheritdoc/>
    public async void NavigateTo(Guid frameGuid, object parameter, NavigationTransitionInfo infoOverride)
    {
        await NavigateCoreAsync(frameGuid, parameter, infoOverride);
    }

    private WindowFrameContext? GetCurrentWindowFrameContext()
    {
        if (_frame.Tag is Guid frameGuid && _guidToWindowFrame.TryGetValue(frameGuid, out var context))
        {
            return context;
        }

        return null;
    }

    private async Task NavigateCoreAsync(Guid frameGuid, object? parameter, NavigationTransitionInfo? infoOverride)
    {
        if (_guidToWindowFrame.TryGetValue(frameGuid, out var context) is false)
        {
            return;
        }

        var beforeFrameContext = GetCurrentWindowFrameContext();
        infoOverride ??= new DrillInNavigationTransitionInfo();
        var args = new WindowFrameNavigateEventArgs();

        var element = context.Value;
        await element.OnNavigatingAsync(args);
        if (args.Handled)
            return;

        _frame.Tag = frameGuid;
        _frame.Navigate(context.Metadata.PageType, parameter, infoOverride);
        _frame.BackStack.Clear();

        if (beforeFrameContext is not null)
        {
            await beforeFrameContext.Value.OnNavigateFromAsync();
        }
    }
}
