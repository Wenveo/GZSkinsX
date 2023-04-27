// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Linq;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Navigation;
using GZSkinsX.Appx.Navigation.Controls;
using GZSkinsX.DotNet.Diagnostics;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.Navigation;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NavigationRootPage : Page
{
    private readonly NavigationService _navigationService;
    private readonly IAppxTitleBar _appxTitleBar;

    public NavigationRootPage()
    {
        var resolver = AppxContext.ServiceLocator;

        _appxTitleBar = AppxContext.AppxTitleBar;
        _navigationService = (NavigationService)resolver.Resolve<INavigationService>();

        InitializeComponent();
    }

    /// <summary>
    /// 在加载时向 NavigationView 模板内部注入自定义标题栏元素
    /// </summary>
    private void OnNavLoaded(object sender, RoutedEventArgs e)
    {
        var navViewRoot = sender as NavigationView2;
        Debug2.Assert(navViewRoot is not null);
        navViewRoot.Loaded -= OnNavLoaded;

        if (navViewRoot.GetTemplateChild2("TopNavArea") is StackPanel topNavArea &&
            topNavArea.Parent is Grid topRootGrid)
        {
            const string TopNavTitleBar = "TopNavTitleBar";
            var navTitleBar = topRootGrid.Children.FirstOrDefault(child =>
            {
                // 查找是否已经在模板中注入了自定义标题栏
                if (child is FrameworkElement frameworkElement)
                {
                    return frameworkElement.Name == TopNavTitleBar;
                }

                return false;
            });

            if (navTitleBar is null)
            {
                // 设置背景色为空，不然会挡住自定义的标题栏
                topNavArea.Background = null;
                navTitleBar = new CustomizeNavTitleBar
                {
                    Name = TopNavTitleBar,
                    HorizontalAlignment = topNavArea.HorizontalAlignment,
                    VerticalAlignment = topNavArea.VerticalAlignment
                };

                // 设置 UI 元素在显示层中的 Z 轴顺序
                Canvas.SetZIndex(topNavArea, 2);
                Canvas.SetZIndex(navTitleBar, 1);

                topRootGrid.Children.Add(navTitleBar);
            }

            _appxTitleBar.SetTitleBar(navTitleBar);
        }
        else
        {
            // 添加默认操作，以防万一
            _appxTitleBar.SetTitleBar(AppTitleBar);
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        _navigationService._navigationViewRoot.Loaded += OnNavLoaded;
        contentPresenter.Content = _navigationService._navigationViewRoot;

        base.OnNavigatedTo(e);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        contentPresenter.Content = null;
        _appxTitleBar.SetTitleBar(null);

        base.OnNavigatedFrom(e);
    }
}
