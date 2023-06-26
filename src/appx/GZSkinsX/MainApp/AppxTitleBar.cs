// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.Api.Appx;

using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace GZSkinsX.MainApp;

/// <see cref="IAppxTitleBar"/>
[Shared, Export(typeof(IAppxTitleBar))]
internal sealed class AppxTitleBar : IAppxTitleBar
{
    /// <summary>
    /// 用于获取和设置是否将内容扩展至标题栏
    /// </summary>
    private readonly CoreApplicationViewTitleBar _coreTitleBar;

    /// <summary>
    /// 当前应用程序主视图的标题栏
    /// </summary>
    private readonly ApplicationViewTitleBar _titleBar;

    /// <summary>
    /// 初始化 <see cref="AppxTitleBar"/> 的实例
    /// </summary>
    [ImportingConstructor]
    public AppxTitleBar()
    {
        _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
        _titleBar = ApplicationView.GetForCurrentView().TitleBar;
    }

    /// <inheritdoc/>
    public bool ExtendViewIntoTitleBar
    {
        get => _coreTitleBar.ExtendViewIntoTitleBar;
        set => _coreTitleBar.ExtendViewIntoTitleBar = value;
    }

    /// <inheritdoc/>
    public Color? InactiveForegroundColor
    {
        get => _titleBar.InactiveForegroundColor;
        set => _titleBar.InactiveForegroundColor = value;
    }

    /// <inheritdoc/>
    public Color? InactiveBackgroundColor
    {
        get => _titleBar.InactiveBackgroundColor;
        set => _titleBar.InactiveBackgroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ForegroundColor
    {
        get => _titleBar.ForegroundColor;
        set => _titleBar.ForegroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ButtonPressedForegroundColor
    {
        get => _titleBar.ButtonPressedForegroundColor;
        set => _titleBar.ButtonPressedForegroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ButtonPressedBackgroundColor
    {
        get => _titleBar.ButtonPressedBackgroundColor;
        set => _titleBar.ButtonPressedBackgroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ButtonInactiveForegroundColor
    {
        get => _titleBar.ButtonInactiveForegroundColor;
        set => _titleBar.ButtonInactiveForegroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ButtonInactiveBackgroundColor
    {
        get => _titleBar.ButtonInactiveBackgroundColor;
        set => _titleBar.ButtonInactiveBackgroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ButtonHoverForegroundColor
    {
        get => _titleBar.ButtonHoverForegroundColor;
        set => _titleBar.ButtonHoverForegroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ButtonHoverBackgroundColor
    {
        get => _titleBar.ButtonHoverBackgroundColor;
        set => _titleBar.ButtonHoverBackgroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ButtonForegroundColor
    {
        get => _titleBar.ButtonForegroundColor;
        set => _titleBar.ButtonForegroundColor = value;
    }

    /// <inheritdoc/>
    public Color? ButtonBackgroundColor
    {
        get => _titleBar.ButtonBackgroundColor;
        set => _titleBar.ButtonBackgroundColor = value;
    }

    /// <inheritdoc/>
    public Color? BackgroundColor
    {
        get => _titleBar.BackgroundColor;
        set => _titleBar.BackgroundColor = value;
    }

    /// <inheritdoc/>
    public void SetTitleBar(UIElement? value)
    => AppxContext.AppxWindow.MainWindow.SetTitleBar(value);
}
