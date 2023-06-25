// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Composition;
using System.Threading.Tasks;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Themes;
using GZSkinsX.Api.Toolkit;

using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

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
    /// 用于对高对比度辅助功能设置的访问
    /// </summary>
    private readonly AccessibilitySettings _accessible;

    /// <summary>
    /// 用于对 UI 元素设置的访问
    /// </summary>
    private readonly UISettings _settings;

    /// <inheritdoc/>
    public ElementTheme ActualTheme { get; private set; }

    /// <inheritdoc/>
    public ElementTheme CurrentTheme
    {
        get => _themeSettings.CurrentTheme;
        private set => _themeSettings.CurrentTheme = value;
    }

    /// <inheritdoc/>
    public bool IsHighContrast { get; private set; }

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

        _accessible = new();
        _settings = new();

        _accessible.HighContrastChanged += OnHighContrastChanged;
        _settings.ColorValuesChanged += OnColorValuesChanged;

        ActualTheme = CurrentTheme is not ElementTheme.Default ? CurrentTheme
            : Application.Current.RequestedTheme is ApplicationTheme.Light
                ? ElementTheme.Light : ElementTheme.Dark;
    }

    private void OnHighContrastChanged(AccessibilitySettings sender, object args)
    {
        UpdateProperties();
    }

    private void OnColorValuesChanged(UISettings sender, object args)
    {
        UpdateProperties();
    }

    private void UpdateProperties()
    {
        _dispatcherQueue.EnqueueAsync(() =>
        {
            if (AppxContext.AppxWindow.MainWindow.Content is FrameworkElement frameworkElement)
            {
                if (_accessible.HighContrast && _accessible.HighContrastScheme.IndexOf("white", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    // If our HighContrastScheme is ON & a lighter one, then we should remain in 'Light' theme mode for Monaco Themes Perspective
                    IsHighContrast = false;
                    CurrentTheme = ElementTheme.Light;
                    ActualTheme = ElementTheme.Light;

                }
                else
                {
                    // Otherwise, we just set to what's in the system as we'd expect.
                    IsHighContrast = _accessible.HighContrast;
                    CurrentTheme = frameworkElement.RequestedTheme;
                    ActualTheme = frameworkElement.ActualTheme;
                }
            }
            else
            {
                ActualTheme = Application.Current.RequestedTheme == default ? ElementTheme.Light : ElementTheme.Dark;
                CurrentTheme = ElementTheme.Default;
            }

            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(ActualTheme, CurrentTheme, IsHighContrast));
        });
    }

    /// <inheritdoc/>
    public async Task SetElementThemeAsync(ElementTheme newTheme)
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            CurrentTheme = newTheme;
            if (AppxContext.AppxWindow.MainWindow.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = newTheme;
                ActualTheme = frameworkElement.ActualTheme;
            }
            else
            {
                ActualTheme = Application.Current.RequestedTheme == default ? ElementTheme.Light : ElementTheme.Dark;
            }

            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(ActualTheme, CurrentTheme, IsHighContrast));
        });
    }
}
