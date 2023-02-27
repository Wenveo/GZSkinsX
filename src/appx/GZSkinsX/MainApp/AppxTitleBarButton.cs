// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;

using Windows.UI;
using Windows.UI.ViewManagement;

namespace GZSkinsX.MainApp;

/// <inheritdoc cref="IAppxTitleBarButton"/>
[Shared, Export(typeof(IAppxTitleBarButton))]
internal sealed class AppxTitleBarButton : IAppxTitleBarButton
{
    /// <summary>
    /// 当前应用程序主视图的标题栏
    /// </summary>
    private readonly ApplicationViewTitleBar _titleBar;

    /// <summary>
    /// 初始化 <see cref="AppxTitleBarButton"/> 的新实例
    /// </summary>
    public AppxTitleBarButton()
    {
        _titleBar = ApplicationView.GetForCurrentView().TitleBar;
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
}
