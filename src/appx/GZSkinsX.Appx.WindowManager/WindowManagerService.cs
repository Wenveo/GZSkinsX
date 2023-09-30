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

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.WindowManager;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace GZSkinsX.Appx.WindowManager;

/// <inheritdoc cref="IWindowManagerService"/>
[Shared, Export(typeof(IWindowManagerService))]
internal sealed class WindowManagerService : IWindowManagerService
{
    /// <summary>
    /// 存放所有已导出的 <see cref="IWindowFrame"/> 类型对象。
    /// </summary>
    private readonly IEnumerable<Lazy<IWindowFrame, WindowFrameMetadataAttribute>> _windowFrames;

    /// <summary>
    /// 使用 <see cref="Guid"/> 作为 Key 并存储所有 <see cref="IWindowFrame"/> 上下文对象。
    /// </summary>
    private readonly Dictionary<Guid, WindowFrameContext> _guidToWindowFrame;

    /// <summary>
    /// 用于导航的内部定义控件。
    /// </summary>
    private readonly Frame _frame;

    /// <inheritdoc/>
    public IWindowFrameContext? FrameContext
    {
        get
        {
            if (_frame.SourcePageType is not { } pageType)
            {
                return null;
            }

            return _guidToWindowFrame.Values.FirstOrDefault(a => a.Metadata.PageType == pageType);
        }
    }

    /// <summary>
    /// 初始化 <see cref="WindowManagerService"/> 的新实例。
    /// </summary>
    [ImportingConstructor]
    public WindowManagerService([ImportMany] IEnumerable<Lazy<IWindowFrame, WindowFrameMetadataAttribute>> windowFrames)
    {
        _windowFrames = windowFrames;

        _frame = new();
        _guidToWindowFrame = new();

        InitializeContext();
    }

    /// <summary>
    /// 初始化上下文对象。
    /// </summary>
    public void InitializeContext()
    {
        if (AppxContext.AppxWindow.MainWindow.Content is not Frame frame || frame != _frame)
        {
            foreach (var elem in _windowFrames)
            {
                var guidString = elem.Metadata.Guid;
                var b = Guid.TryParse(guidString, out var guid);
                Debug.Assert(b, $"WindowManagerService: Couldn't parse Guid property: '{guidString}'");
                if (!b) continue;

                var pageType = elem.Metadata.PageType;
                b = pageType.BaseType != null;
                Debug.Assert(b, $"WindowManagerService: The PageType cannot be null: '{pageType.BaseType}'");
                if (!b) continue;

                b = pageType.BaseType == typeof(Page);
                Debug.Assert(b, $"WindowManagerService: The PageType is not inherite by Page: '{pageType.BaseType}'");
                if (!b) continue;

                _guidToWindowFrame[guid] = new WindowFrameContext(elem);
            }

            _frame.RequestedTheme = AppxContext.ThemeService.CurrentTheme;
            AppxContext.AppxWindow.MainWindow.Content = _frame;
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            NavigateCore(guid, null, null);
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(string guidString, object parameter)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            NavigateCore(guid, parameter, null);
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(string guidString, object parameter, NavigationTransitionInfo infoOverride)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            NavigateCore(guid, parameter, infoOverride);
        }
    }

    /// <inheritdoc/>
    public void NavigateTo(Guid frameGuid)
    {
        NavigateCore(frameGuid, null, null);
    }

    /// <inheritdoc/>
    public void NavigateTo(Guid frameGuid, object parameter)
    {
        NavigateCore(frameGuid, parameter, null);
    }

    /// <inheritdoc/>
    public void NavigateTo(Guid frameGuid, object parameter, NavigationTransitionInfo infoOverride)
    {
        NavigateCore(frameGuid, parameter, infoOverride);
    }

    private async void NavigateCore(Guid frameGuid, object? parameter, NavigationTransitionInfo? infoOverride)
    {
        if (_guidToWindowFrame.TryGetValue(frameGuid, out var context) is false)
        {
            return;
        }

        var windowFrame = context.Value;
        WindowFrameNavigatingEvnetArgs args = new(context, parameter, infoOverride);

        var b = windowFrame is IWindowFrame2 windowFrame2
            ? await windowFrame2.CanNavigateToAsync(args)
            : windowFrame.CanNavigateTo(args);

        AppxContext.LoggingService.LogAlways(
            "GZSkinsX.Appx.WindowManager.WindowManagerService.NavigateCore",
            $"{(b ? "Can" : "Cannot")} navigate to the frame \"{context.Metadata.Guid}\".");

        if (b)
        {
            if (_frame.Navigate(context.Metadata.PageType, args.Parameter,
                args.NavigationTransitionInfo ?? new DrillInNavigationTransitionInfo()))
            {
                AppxContext.LoggingService.LogOkay(
                    "GZSkinsX.Appx.WindowManager.WindowManagerService.NavigateCore",
                    $"Successfully navigate to the frame \"{context.Metadata.Guid}\".");

                _frame.BackStack.Clear();
            }
            else
            {
                AppxContext.LoggingService.LogWarning(
                    "GZSkinsX.Appx.WindowManager.WindowManagerService.NavigateCore",
                    $"Failed to navigate to the frame \"{context.Metadata.Guid}\".");
            }
        }
    }
}
