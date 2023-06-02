// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Composition;

using GZSkinsX.SDK.Appx;

using Windows.ApplicationModel.Core;

namespace GZSkinsX.MainApp;

/// <see cref="IAppxTitleBar"/>
[Shared, Export(typeof(IAppxTitleBar))]
internal sealed class AppxTitleBar : IAppxTitleBar
{
    /// <summary>
    /// 用于获取和设置是否将内容扩展至标题栏
    /// </summary>
    private readonly CoreApplicationViewTitleBar _coreTitleBar;

    /// <inheritdoc/>
    public bool ExtendViewIntoTitleBar
    {
        get => _coreTitleBar.ExtendViewIntoTitleBar;
        set => _coreTitleBar.ExtendViewIntoTitleBar = value;
    }

    /// <summary>
    /// 初始化 <see cref="AppxTitleBar"/> 的实例
    /// </summary>
    [ImportingConstructor]
    public AppxTitleBar()
    {
        _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
    }

    /// <inheritdoc/>
    public void SetTitleBar(Windows.UI.Xaml.UIElement? value)
    => AppxContext.AppxWindow.MainWindow.SetTitleBar(value);
}
