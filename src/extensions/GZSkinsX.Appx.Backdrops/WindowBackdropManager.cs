// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Appx.Backdrops;

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;

using WinRT;

namespace GZSkinsX.Appx.Backdrops;

/// <inheritdoc cref="IWindowBackdropManager"/>
[Shared, Export(typeof(IWindowBackdropManager))]
internal sealed class WindowBackdropManager : IWindowBackdropManager
{
    /// <summary>
    /// 当前应用程序主窗口
    /// </summary>
    private readonly Window _mainWindow;

    /// <summary>
    /// 用于设置 Backdrop 的目标窗体对象
    /// </summary>
    private readonly ICompositionSupportsSystemBackdrop _target;

    /// <summary>
    /// 当前应用程序的 Backdrop 配置源
    /// </summary>
    private readonly SystemBackdropConfiguration _configurationSource;

    /// <summary>
    /// 用于创建当前应用程序的 DispatcherQueueController，不然将无法设置 Backdrops
    /// </summary>
    private readonly WindowsSystemDispatcherQueueHelper _dispatcherQueueHelper;

    /// <summary>
    /// 用于将窗口背景设置为亚克力材质
    /// </summary>
    private AcrylicSystemBackdrop? _acrylicBackdrop;

    /// <summary>
    /// 用于将窗口背景设置为云母材质
    /// </summary>
    private MicaSystemBackdrop? _micaSystemBackdrop;

    /// <summary>
    /// 当前已应用的背景类型
    /// </summary>
    private BackdropType _currentBackdrop;

    /// <summary>
    /// 获取 <see cref="AcrylicSystemBackdrop"/> 对象的实例，当对象为空时会进行创建
    /// </summary>
    public AcrylicSystemBackdrop AcrylicBackdrop
    {
        get => _acrylicBackdrop ??= new AcrylicSystemBackdrop(_target, _configurationSource);
    }

    /// <summary>
    /// 获取 <see cref="MicaSystemBackdrop"/> 对象的实例，当对象为空时会进行创建
    /// </summary>
    public MicaSystemBackdrop MicaBackdrop
    {
        get => _micaSystemBackdrop ??= new MicaSystemBackdrop(_target, _configurationSource);
    }

    /// <inheritdoc/>
    public BackdropType CurrentBackdrop
    {
        get => _currentBackdrop;
    }

    /// <summary>
    /// 初始化 <see cref="WindowBackdropManager"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public WindowBackdropManager(IAppxWindow appxWindow)
    {
        _mainWindow = appxWindow.MainWindow;
        _target = _mainWindow.As<ICompositionSupportsSystemBackdrop>();
        _configurationSource = new SystemBackdropConfiguration();
        _dispatcherQueueHelper = new WindowsSystemDispatcherQueueHelper();
        _dispatcherQueueHelper.EnsureWindowsSystemDispatcherQueueController();

        _mainWindow.Activated += OnActivated;
        _mainWindow.Closed += OnClosed;

        if (_mainWindow.Content is FrameworkElement elem)
        {
            elem.ActualThemeChanged += OnThemeChanged;
        }

        _currentBackdrop = BackdropType.DefaultColor;
    }

    /// <summary>
    /// 更新配置源中的属性
    /// </summary>
    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
    }

    /// <summary>
    /// 在窗口关闭时移除事件并释放成员对象
    /// </summary>
    private void OnClosed(object sender, WindowEventArgs args)
    {
        if (!args.Handled)
        {
            _mainWindow.Activated -= OnActivated;
            _micaSystemBackdrop?.Dispose();
            _acrylicBackdrop?.Dispose();
        }
    }

    /// <summary>
    /// 更新配置源中的主题
    /// </summary>
    private void OnThemeChanged(FrameworkElement sender, object args)
    {
        _configurationSource.Theme = sender.ActualTheme switch
        {
            ElementTheme.Dark => SystemBackdropTheme.Dark,
            ElementTheme.Light => SystemBackdropTheme.Light,
            _ => SystemBackdropTheme.Default
        };
    }

    /// <inheritdoc/>
    public bool SetBackdrop(BackdropType type)
    {
        CleanUpBackdrop();

        _currentBackdrop = type;
        if (_currentBackdrop is BackdropType.Mica or BackdropType.MicaAlt)
        {
            return MicaBackdrop.Apply(_currentBackdrop == BackdropType.MicaAlt);
        }
        else if (_currentBackdrop is BackdropType.DesktopAcrylic)
        {
            return AcrylicBackdrop.Apply();
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 清除当前背景
    /// </summary>
    private void CleanUpBackdrop()
    {
        if (_currentBackdrop is BackdropType.Mica or BackdropType.MicaAlt)
        {
            _micaSystemBackdrop?.CleanUp();
        }
        else if (_currentBackdrop is BackdropType.DesktopAcrylic)
        {
            _acrylicBackdrop?.CleanUp();
        }
    }
}
