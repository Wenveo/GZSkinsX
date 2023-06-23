// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Themes;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

using Windows.UI.ViewManagement;

namespace GZSkinsX.Themes;

/// <inheritdoc cref="IThemeService"/>
[Shared, Export(typeof(IThemeService))]
internal sealed class ThemeService : IThemeService
{
    /// <summary>
    /// 用于存储主题设置的内部 <see cref="ThemeSettings"/> 实例
    /// </summary>
    private readonly ThemeSettings _themeSettings;

    /// <summary>
    /// 当前线程的调度队列，用于访问和同步 UI 属性
    /// </summary>
    private readonly DispatcherQueue _dispatcherQueue;

    /// <summary>
    /// 用于对 UI 元素设置的访问
    /// </summary>
    private readonly UISettings _uiSettings;

    /// <inheritdoc/>
    public ElementTheme ActualTheme { get; private set; }

    /// <inheritdoc/>
    public ElementTheme CurrentTheme
    {
        get => _themeSettings.CurrentTheme;
        private set => _themeSettings.CurrentTheme = value;
    }

    /// <inheritdoc/>
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// 初始化 <see cref="ThemeService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public ThemeService(ThemeSettings themeSettings)
    {
        _themeSettings = themeSettings;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        _uiSettings = new UISettings();
        _uiSettings.ColorValuesChanged += OnColorValuesChanged;

        ActualTheme = CurrentTheme is not ElementTheme.Default ? CurrentTheme
            : Application.Current.RequestedTheme == ApplicationTheme.Light
                ? ElementTheme.Light : ElementTheme.Dark;
    }

    private void OnColorValuesChanged(UISettings sender, object args)
    {
        UpdateProperties();
    }

    private void UpdateProperties()
    {
        _dispatcherQueue.EnqueueAsync(() =>
        {
            if (AppxContext.AppxWindow.MainWindow.Content is FrameworkElement frameworkElement && CurrentTheme != frameworkElement.ActualTheme)
            {
                CurrentTheme = frameworkElement.RequestedTheme;
                ActualTheme = frameworkElement.ActualTheme;

                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(frameworkElement.ActualTheme, frameworkElement.RequestedTheme));
            }
            else
            {
                var currentAppTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(currentAppTheme, ElementTheme.Default));
            }
        });
    }

    /// <inheritdoc/>
    public async Task<bool> SetElementThemeAsync(ElementTheme newTheme)
    {
        var result = await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (AppxContext.AppxWindow.MainWindow.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = newTheme;
                ActualTheme = frameworkElement.ActualTheme;
                CurrentTheme = newTheme;

                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(frameworkElement.ActualTheme, newTheme));
                return true;
            }

            return false;
        });

        return result;
    }
}
