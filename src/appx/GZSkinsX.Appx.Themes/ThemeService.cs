// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Composition;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using CommunityToolkit.WinUI;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Themes;

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

using Windows.UI.ViewManagement;

using Windows.Win32;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.WindowsAndMessaging;

namespace GZSkinsX.Appx.Themes;

/// <inheritdoc cref="IThemeService"/>
[Shared, Export(typeof(IThemeService))]
internal sealed class ThemeService : IThemeService
{
    /// <summary>
    /// 用于存储主题设置的内部 <see cref="ThemeSettings"/> 实例。
    /// </summary>
    private readonly ThemeSettings _themeSettings;

    /// <summary>
    /// 当前线程的调度队列，用于访问和同步 UI 属性。
    /// </summary>
    private readonly DispatcherQueue _dispatcherQueue;

    /// <summary>
    /// 用于对 UI 元素设置的访问。
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
    public bool IsHighContrast
    {
        get
        {
            unsafe
            {
                var highContrast = new HIGHCONTRASTW
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(HIGHCONTRASTW))
                };

                var ret = PInvoke.SystemParametersInfo(
                    SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETHIGHCONTRAST,
                    highContrast.cbSize, &highContrast, 0);

                return ret > 0 && (highContrast.dwFlags & HIGHCONTRASTW_FLAGS.HCF_HIGHCONTRASTON) == HIGHCONTRASTW_FLAGS.HCF_HIGHCONTRASTON;
            }
        }
    }

    /// <inheritdoc/>
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// 初始化 <see cref="ThemeService"/> 的新实例。
    /// </summary>
    public ThemeService()
    {
        _themeSettings = AppxContext.Resolve<ThemeSettings>();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        _settings = new();
        _settings.ColorValuesChanged += OnColorValuesChanged;

        ActualTheme = CurrentTheme is not ElementTheme.Default ? CurrentTheme
            : Application.Current.RequestedTheme is ApplicationTheme.Light
                ? ElementTheme.Light : ElementTheme.Dark;

        UXThemeHelper.ApplyThemeForApp(ActualTheme is ElementTheme.Dark);
    }

    private void OnColorValuesChanged(UISettings sender, object args)
    {
        UpdateProperties();
    }

    private async void UpdateProperties()
    {
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (AppxContext.AppxWindow.MainWindow.Content is FrameworkElement frameworkElement)
            {
                ActualTheme = frameworkElement.ActualTheme;
                CurrentTheme = frameworkElement.RequestedTheme;
            }
            else
            {
                ActualTheme = Application.Current.RequestedTheme == default ? ElementTheme.Light : ElementTheme.Dark;
                CurrentTheme = ElementTheme.Default;
            }

            UXThemeHelper.ApplyThemeForApp(ActualTheme is ElementTheme.Dark);
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

            UXThemeHelper.ApplyThemeForApp(ActualTheme is ElementTheme.Dark);
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(ActualTheme, CurrentTheme, IsHighContrast));
        });
    }
}
