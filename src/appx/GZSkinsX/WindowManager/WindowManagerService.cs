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

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.WindowManager;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace GZSkinsX.WindowManager;

/// <inheritdoc cref="IWindowManagerService"/>
[Shared, Export(typeof(IWindowManagerService))]
internal sealed class WindowManagerService : IWindowManagerService
{
    /// <summary>
    /// ��ǰӦ�ó���������
    /// </summary>
    private readonly IAppxWindow _appxWindow;

    /// <summary>
    /// ��������ѵ����� <see cref="IWindowFrame"/> ���Ͷ���
    /// </summary>
    private readonly IEnumerable<Lazy<IWindowFrame, WindowFrameMetadataAttribute>> _viewElements;

    /// <summary>
    /// ʹ�� <see cref="Guid"/> ��Ϊ Key ���洢���� <see cref="IWindowFrame"/> �����Ķ���
    /// </summary>
    private readonly Dictionary<Guid, WindowFrameContext> _guidToViewElement;

    /// <summary>
    /// ���ڵ������ڲ�����ؼ�
    /// </summary>
    private readonly Frame _frame;

    /// <summary>
    /// ��ʼ�� <see cref="WindowManagerService"/> ����ʵ��
    /// </summary>
    [ImportingConstructor]
    public WindowManagerService(IAppxWindow appxWindow, [ImportMany] IEnumerable<Lazy<IWindowFrame, WindowFrameMetadataAttribute>> viewElements)
    {
        _appxWindow = appxWindow;
        _viewElements = viewElements;

        _frame = new Frame();
        _guidToViewElement = new Dictionary<Guid, WindowFrameContext>();

        InitializeContext();
    }

    /// <summary>
    /// ��ʼ�������Ķ���
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

                _guidToViewElement[guid] = new WindowFrameContext(elem);
            }

            _appxWindow.MainWindow.Content = _frame;
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

    private async void NavigateCore(WindowFrameContext context, object? parameter, NavigationTransitionInfo? infoOverride)
    {
        infoOverride ??= new DrillInNavigationTransitionInfo();
        var args = new WindowFrameNavigateEventArgs();

        var element = context.Value;
        await element.OnNavigatingAsync(args);
        if (args.Handled)
            return;

        _frame.Navigate(context.Metadata.PageType, parameter, infoOverride);
        await element.OnInitializeAsync((Page)_frame.Content);

        _frame.BackStack.Clear();
    }
}