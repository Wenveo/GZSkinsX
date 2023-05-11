// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GZSkinsX.Api.Navigation;
using GZSkinsX.DotNet.Diagnostics;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Navigation;

/// <inheritdoc cref="INavigationViewManager"/>
internal sealed class NavigationViewManager : INavigationViewManager
{
    /// <summary>
    /// 内部的 <see cref="MUXC.NavigationView"/> 成员定义
    /// </summary>
    private readonly MUXC.NavigationView _navigationView;

    /// <summary>
    /// 内部的 <see cref="Frame"/> 成员定义
    /// </summary>
    private readonly Frame _rootFrame;

    /// <summary>
    /// 此成员用于帮助 OnNavigated 方法中快速枚举导航视图项。
    /// <para>
    /// 只有在经过 NavigateCoreAsync 方法，此项才会被赋值，并且随后将会进入 OnNavigated 方法。
    /// </para>
    /// <para>
    /// 由于在 NavigateCoreAsync 方法中已经进行过一次 FindNavItem，
    /// 之后在 OnNavigated 方法中再进行一次 FindNavItem 则显得不合理。
    /// 因此通过此成员 "暂存" 已找到的结果，以供 OnNavigated 方法中使用。
    /// </para>
    /// </summary>
    private MUXC.NavigationViewItem? _navItemFromCore;

    /// <inheritdoc/>
    public bool CanGoBack => _rootFrame.CanGoBack;

    /// <inheritdoc/>
    public bool CanGoForward => _rootFrame.CanGoForward;

    /// <inheritdoc/>
    public MUXC.NavigationView NavigationView => _navigationView;

    /// <inheritdoc/>
    public Frame RootFrame => _rootFrame;

    /// <inheritdoc/>
    public event NavigatedEventHandler? Navigated;

    /// <summary>
    /// 初始化 <see cref="NavigationViewManager"/> 的新实例
    /// </summary>
    public NavigationViewManager(MUXC.NavigationView navigationView)
    {
        _navigationView = navigationView;

        if (_navigationView.Content is not Frame frame)
            _navigationView.Content = frame = new Frame();

        _rootFrame = frame;

        _rootFrame.Navigated += OnNavigated;
        _navigationView.SelectionChanged += OnNavSelectionChanged;
    }

    /// <summary>
    /// 在导航至目标页面时触发，并同步导航视图中的选择项
    /// <para>
    /// （当通过后台 Api 进行导航，而不是在 UI 上选择时，需要手动同步导航视图中的选择项）
    /// </para>
    /// </summary>
    private async void OnNavigated(object sender, NavigationEventArgs e)
    {
        var guid = (Guid)_rootFrame.Tag;
        var item = _navItemFromCore ?? FindNavItem(ref guid);
        if (item is not null)
        {
            var ctx = (NavigationItemContext)item.DataContext;
            await ctx.Value.OnNavigatedToAsync(e);
            _navigationView.SelectedItem = item;

            Navigated?.Invoke(sender, e);
            _navItemFromCore = null;
        }
    }

    /// <summary>
    /// 在 <see cref="MUXC.NavigationView"/> 中选择导航项时触发，并根据已选择的项进行导航操作
    /// </summary>
    private void OnNavSelectionChanged(MUXC.NavigationView sender, MUXC.NavigationViewSelectionChangedEventArgs args)
    {
        var navItem = args.SelectedItem as MUXC.NavigationViewItem;
        if (navItem is null or { SelectsOnInvoked: false } ||
            navItem.DataContext is not NavigationItemContext ctx)
        {
            return;
        }

        _rootFrame.Tag = (Guid)navItem.Tag;
        Debug2.Assert(ctx.Metadata.PageType is not null);
        _rootFrame.Navigate(ctx.Metadata.PageType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    private MUXC.NavigationViewItem? FindSubNavItem(IEnumerable<object> items, ref Guid guid)
    {
        foreach (var item in items.OfType<MUXC.NavigationViewItem>())
        {
            if (item.Tag is Guid itemGuid && itemGuid == guid)
            {
                return item;
            }

            var subItem = FindSubNavItem(item.MenuItems, ref guid);
            if (subItem is not null)
                return subItem;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    private MUXC.NavigationViewItem? FindNavItem(ref Guid guid)
    {
        return FindSubNavItem(_navigationView.MenuItems, ref guid) ?? FindSubNavItem(_navigationView.FooterMenuItems, ref guid);
    }

    /// <summary>
    /// 获取与当前导航视图中的选中项所关联的 <see cref="NavigationItemContext"/> 上下文对象
    /// </summary>
    /// <returns>如果找到所关联的 <see cref="NavigationItemContext"/> 上下文对象则会返回该实例，否则返回空</returns>
    private NavigationItemContext? GetCurrentNavItemCtx()
    {
        if (_navigationView.SelectedItem is MUXC.NavigationViewItem navItem &&
            navItem.DataContext is NavigationItemContext ctx)
        {
            return ctx;
        }

        return null;
    }

    /// <inheritdoc/>
    public bool GoBack()
    {
        if (_rootFrame.CanGoBack)
        {
            var beforeNavItemCtx = GetCurrentNavItemCtx();
            _rootFrame.GoBack();
            beforeNavItemCtx?.Value.OnNavigatedFromAsync();
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool GoForward()
    {
        if (_rootFrame.CanGoForward)
        {
            var beforeNavItemCtx = GetCurrentNavItemCtx();
            _rootFrame.GoForward();
            beforeNavItemCtx?.Value.OnNavigatedFromAsync();
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            await NavigateCoreAsync(guid, null, null);
        }
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString, object parameter)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            await NavigateCoreAsync(guid, parameter, null);
        }
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString, object parameter, NavigationTransitionInfo infoOverride)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            await NavigateCoreAsync(guid, parameter, infoOverride);
        }
    }

    /// <inheritdoc/>
    public async void NavigateTo(Guid navItemGuid)
    {
        await NavigateCoreAsync(navItemGuid, null, null);
    }

    /// <inheritdoc/>
    public async void NavigateTo(Guid navItemGuid, object parameter)
    {
        await NavigateCoreAsync(navItemGuid, parameter, null);
    }

    /// <inheritdoc/>
    public async void NavigateTo(Guid navItemGuid, object parameter, NavigationTransitionInfo infoOverride)
    {
        await NavigateCoreAsync(navItemGuid, parameter, infoOverride);
    }

    /// <summary>
    /// 核心导航方法，包含具体的导航操作实现。大部分导航相关的 Api 都使用此函数进行页面导航
    /// </summary>
    private async Task NavigateCoreAsync(Guid guid, object? parameter, NavigationTransitionInfo? infoOverride)
    {
        var item = FindNavItem(ref guid);
        if (item is null || item.DataContext is not NavigationItemContext ctx ||
            ctx.Metadata.PageType is null)
        {
            return;
        }

        var beforeNavItemCtx = GetCurrentNavItemCtx();
        infoOverride ??= new DrillInNavigationTransitionInfo();

        _rootFrame.Tag = guid;
        _navItemFromCore = item;

        if (_rootFrame.Navigate(ctx.Metadata.PageType, parameter, infoOverride))
        {
            if (beforeNavItemCtx is not null)
            {
                await beforeNavItemCtx.Value.OnNavigatedFromAsync();
            }
        }
    }
}
