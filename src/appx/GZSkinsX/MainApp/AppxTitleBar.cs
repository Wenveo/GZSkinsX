// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Composition;

using GZSkinsX.Api.Appx;

using Windows.ApplicationModel.Core;

namespace GZSkinsX.MainApp;

/// <see cref="IAppxTitleBar"/>
[Shared, Export(typeof(IAppxTitleBar))]
internal sealed class AppxTitleBar : IAppxTitleBar
{
    /// <summary>
    /// ���ڻ�ȡ��ǰӦ�ó���������
    /// </summary>
    private readonly IAppxWindow _appxWindow;

    /// <summary>
    /// ���ڻ�ȡ�������Ƿ�������չ��������
    /// </summary>
    private readonly CoreApplicationViewTitleBar _coreTitleBar;

    /// <inheritdoc/>
    public bool ExtendViewIntoTitleBar
    {
        get => _coreTitleBar.ExtendViewIntoTitleBar;
        set => _coreTitleBar.ExtendViewIntoTitleBar = value;
    }

    /// <summary>
    /// ��ʼ�� <see cref="AppxTitleBar"/> ��ʵ��
    /// </summary>
    [ImportingConstructor]
    public AppxTitleBar(IAppxWindow appxWindow)
    {
        _appxWindow = appxWindow;
        _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
    }

    /// <inheritdoc/>
    public void SetTitleBar(Windows.UI.Xaml.UIElement value)
    => _appxWindow.MainWindow.SetTitleBar(value);
}