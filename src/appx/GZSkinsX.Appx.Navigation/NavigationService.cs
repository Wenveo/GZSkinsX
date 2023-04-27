// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

using GZSkinsX.Api.MRT;
using GZSkinsX.Api.Navigation;
using GZSkinsX.Appx.Navigation.Controls;
using GZSkinsX.DotNet.Diagnostics;

using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Navigation;

/// <inheritdoc cref="MUXC.NavigationView"/>
internal sealed class NavigationView2 : MUXC.NavigationView
{
    [DebuggerNonUserCode]
    public object GetTemplateChild2(string childName)
    {
        return GetTemplateChild(childName);
    }
}

/// <inheritdoc cref="INavigationService"/>
[Shared, Export(typeof(INavigationService))]
internal sealed class NavigationService : INavigationService
{
    /// <summary>
    /// 存放所有已导出的 <see cref="INavigationGroup"/> 对象实例
    /// </summary>
    private readonly IEnumerable<Lazy<INavigationGroup, NavigationGroupMetadataAttribute>> _mefNavGroups;

    /// <summary>
    /// 存放所有已导出的 <see cref="INavigationItem"/> 对象实例
    /// </summary>
    private readonly IEnumerable<Lazy<INavigationItem, NavigationItemMetadataAttribute>> _mefNavItems;

    /// <summary>
    /// 表示用于获取本地化资源的服务实例
    /// </summary>
    private readonly IMRTCoreService _mrtCoreService;

    /// <summary>
    /// 用于存放序列化后的 <see cref="NavigationItemContext"/> 对象实例
    /// </summary>
    private readonly Dictionary<Guid, List<NavigationItemContext>> _guidToNavItems;

    /// <summary>
    /// 存放所有已创建的 <see cref="MUXC.NavigationViewItem"/> 对象实例
    /// </summary>
    internal readonly Dictionary<Guid, MUXC.NavigationViewItem> _createdNavItems;

    /// <summary>
    /// 存储所有 <see cref="NavigationItemContext"/> 对象实例，并使用 <see cref="Guid"/> 作为键以供快速访问
    /// </summary>
    private readonly Dictionary<Guid, NavigationItemContext> _allNavItemCtx;

    /// <summary>
    /// 用于存放所有已枚举的 <see cref="NavigationGroupContext"/> 对象实例
    /// </summary>
    private readonly List<NavigationGroupContext> _navGroups;

    /// <summary>
    /// 用于切换导航项的内部 <see cref="NavigationView2"/> 对象实例
    /// </summary>
    internal readonly NavigationView2 _navigationViewRoot;

    /// <summary>
    /// 用于呈现页面的内部 <see cref="Frame"/> 对象实例
    /// </summary>
    internal readonly Frame _rootFrame;

    /// <inheritdoc/>
    public event NavigatedEventHandler? Navigated;

    /// <inheritdoc/>
    public bool CanGoBack => _rootFrame.CanGoBack;

    /// <inheritdoc/>
    public bool CanGoForward => _rootFrame.CanGoForward;

    /// <summary>
    /// 初始化 <see cref="NavigationService"/> 的新实例
    /// </summary>
    [ImportingConstructor]
    public NavigationService(
        [ImportMany] IEnumerable<Lazy<INavigationGroup, NavigationGroupMetadataAttribute>> mefNavGroups,
        [ImportMany] IEnumerable<Lazy<INavigationItem, NavigationItemMetadataAttribute>> mefNavItems,
        IMRTCoreService mrtCoreService)
    {
        _mefNavGroups = mefNavGroups;
        _mefNavItems = mefNavItems;
        _mrtCoreService = mrtCoreService;

        _createdNavItems = new();
        _guidToNavItems = new();
        _allNavItemCtx = new();
        _navGroups = new();

        _navigationViewRoot = new();
        _rootFrame = new();

        InitializeNavGroups();
        InitializeNavItems();
        InitializeUIObject();
    }

    /// <summary>
    /// 初始化容器以序列化所有的 <see cref="INavigationGroup"/> 对象实例
    /// </summary>
    private void InitializeNavGroups()
    {
        var hashes = new HashSet<Guid>();
        foreach (var item in _mefNavGroups)
        {
            var guidString = item.Metadata.Guid;
            var b = Guid.TryParse(guidString, out var guid);
            Debug.Assert(b, $"NavGroup: Couldn't parse Guid property: {guidString}");
            if (!b)
                continue;

            b = !hashes.Contains(guid);
            Debug.Assert(b, $"NavGroup: An group with the same GUID already exists");
            if (!b)
                continue;

            _navGroups.Add(new NavigationGroupContext(item));
        }

        _navGroups.Sort((a, b) => a.Metadata.Order.CompareTo(b.Metadata.Order));
    }

    /// <summary>
    /// 初始化容器以序列化所有的 <see cref="INavigationItem"/> 对象实例
    /// </summary>
    private void InitializeNavItems()
    {
        foreach (var item in _mefNavItems)
        {
            var ownerGuidString = item.Metadata.OwnerGuid;
            var b = Guid.TryParse(ownerGuidString, out var ownerGuid);
            Debug.Assert(b, $"NavItem: Couldn't parse OwnerGuid property: {ownerGuidString}");
            if (!b)
                continue;

            var guidString = item.Metadata.Guid;
            b = Guid.TryParse(guidString, out var guid);
            Debug.Assert(b, $"NavItem: Couldn't parse Guid property: {guidString}");
            if (!b)
                continue;

            b = !string.IsNullOrEmpty(item.Metadata.Header);
            Debug.Assert(b, $"NavItem: Header is null or empty");
            if (!b)
                continue;

            if (!_guidToNavItems.TryGetValue(ownerGuid, out var list))
                _guidToNavItems.Add(ownerGuid, list = new List<NavigationItemContext>());
            list.Add(new NavigationItemContext(item));
        }

        foreach (var list in _guidToNavItems.Values)
        {
            var hashes = new HashSet<Guid>();
            var origin = new List<NavigationItemContext>(list);
            list.Clear();
            foreach (var group in origin)
            {
                var guid = new Guid(group.Metadata.Guid);
                if (hashes.Contains(guid))
                    continue;

                hashes.Add(guid);
                list.Add(group);

                _allNavItemCtx.Add(guid, group);
            }

            list.Sort((a, b) => a.Metadata.Order.CompareTo(b.Metadata.Order));
        }
    }

    /// <summary>
    /// 初始化内部的 UI 元素对象
    /// </summary>
    private void InitializeUIObject()
    {
        _rootFrame.Navigated += OnNavigated;
        _navigationViewRoot.Content = _rootFrame;
        _navigationViewRoot.PaneDisplayMode = MUXC.NavigationViewPaneDisplayMode.Top;
        _navigationViewRoot.IsBackButtonVisible = MUXC.NavigationViewBackButtonVisible.Collapsed;
        _navigationViewRoot.IsTabStop = false;
        _navigationViewRoot.IsSettingsVisible = false;
        _navigationViewRoot.IsTitleBarAutoPaddingEnabled = false;
        _navigationViewRoot.PaneHeader = CreatePaneHeader();

        var navPaneCustomContent = new CustomizeNavPaneContent(this);
        _navigationViewRoot.PaneCustomContent = navPaneCustomContent;
        _navigationViewRoot.SelectionChanged += OnNavSelectionChanged;
        _navigationViewRoot.Resources.Add("TopNavigationViewTopNavGridMargin", new Thickness(4, 0, 188, 0));

        InitializeNavView(_navigationViewRoot);
    }

    /// <summary>
    /// 创建应用于 <see cref="MUXC.NavigationView.PaneHeader"/> 的内容（应用标题）
    /// </summary>
    private TextBlock CreatePaneHeader()
    {
        return new TextBlock
        {
            Margin = new(14, 0, 8, 0),
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = FontWeights.ExtraBold,
            IsHitTestVisible = false,
            Text = GetLocalizedOrDefault("resx:GZSkinsX.Appx.Navigation/Resources/MainAppx_Title")
        };
    }

    /// <summary>
    /// 为导航视图 <see cref="MUXC.NavigationView"/> 创建和添加子项
    /// </summary>
    /// <param name="navigationView">需要进行初始化的 <see cref="MUXC.NavigationView"/> 对象实例</param>
    private void InitializeNavView(MUXC.NavigationView navigationView)
    {
        var needSeparator = false;
        foreach (var group in _navGroups)
        {
            var container = navigationView.MenuItems;
            var guid = new Guid(group.Metadata.Guid);
            if (_guidToNavItems.TryGetValue(guid, out var navItems))
            {
                if (navItems.Count == 0)
                    continue;

                if (needSeparator)
                    container.Add(new MUXC.NavigationViewItemSeparator());
                else
                    needSeparator = true;

                foreach (var item in navItems)
                    container.Add(CreateNavItemUIObject(item));
            }
        }
    }

    /// <summary>
    /// 在 <see cref="MUXC.NavigationView"/> 中选择导航项时触发，并根据已选择的项进行导航操作
    /// </summary>
    private void OnNavSelectionChanged(MUXC.NavigationView sender, MUXC.NavigationViewSelectionChangedEventArgs args)
    {
        var navItem = args.SelectedItem as MUXC.NavigationViewItem;
        if (navItem is null or { SelectsOnInvoked: false })
        {
            return;
        }

        var guid = (Guid)navItem.Tag;
        if (_allNavItemCtx.TryGetValue(guid, out var context))
        {
            _rootFrame.Tag = guid;
            Debug2.Assert(context.Metadata.PageType is not null);
            _rootFrame.Navigate(context.Metadata.PageType);
        }
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
        if (_allNavItemCtx.TryGetValue(guid, out var ctx) is false ||
            _createdNavItems.TryGetValue(guid, out var item) is false)
        {
            return;
        }

        await ctx.Value.OnNavigatedToAsync(e);
        _navigationViewRoot.SelectedItem = item;

        Navigated?.Invoke(sender, e);
    }

    /// <summary>
    /// 通过解析 <see cref="NavigationItemContext"/> 上下文对象并创建 <see cref="MUXC.NavigationViewItem"/> 对象实例
    /// </summary>
    /// <param name="context">需要解析的 <see cref="NavigationItemContext"/> 上下文对象</param>
    /// <returns>已创建的 <see cref="MUXC.NavigationViewItem"/> 对象实例</returns>
    private MUXC.NavigationViewItem CreateNavItemUIObject(NavigationItemContext context)
    {
        var guid = new Guid(context.Metadata.Guid);
        var navItem = new MUXC.NavigationViewItem
        {
            Tag = guid,
            Icon = context.Value.Icon,
            Content = GetLocalizedOrDefault(context.Metadata.Header),
            SelectsOnInvoked = context.Metadata.PageType is not null
        };

        AutomationProperties.SetName(navItem, navItem.Content as string);

        if (_guidToNavItems.TryGetValue(guid, out var navItems))
        {
            foreach (var item in navItems)
            {
                var subItem = CreateNavItemUIObject(item);
                navItem.MenuItems.Add(subItem);
            }
        }

        _createdNavItems.Add(guid, navItem);
        return navItem;
    }

    /// <summary>
    /// 根据传入具有特定的标识符的资源键的名称以获取本地化资源
    /// </summary>
    /// <param name="resourceKey">需要获取的本地化的资源的键</param>
    /// <returns>如果传入的 <paramref name="resourceKey"/> 包含特定的标识符则会获取本地化的资源，否则将会返回原对象</returns>
    internal string GetLocalizedOrDefault(string resourceKey)
    {
        if (resourceKey.StartsWith("resx:"))
        {
            return _mrtCoreService.MainResourceMap.GetString(resourceKey[5..]);
        }
        else
        {
            return resourceKey;
        }
    }

    /// <summary>
    /// 获取与当前导航视图中的选中项所关联的 <see cref="NavigationItemContext"/> 上下文对象
    /// </summary>
    /// <returns>如果找到所关联的 <see cref="NavigationItemContext"/> 上下文对象则会返回该实例，否则返回空</returns>
    private NavigationItemContext? GetCurrentNavItemCtx()
    {
        if (_navigationViewRoot.SelectedItem is MUXC.NavigationViewItem selectedItem
            && _allNavItemCtx.TryGetValue((Guid)selectedItem.Tag, out var ctx))
        {
            return ctx;
        }

        return null;
    }

    /// <inheritdoc/>
    public bool GoBack()
    {
        if (CanGoBack)
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
        if (CanGoForward)
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
    internal async Task NavigateCoreAsync(Guid guid, object? parameter, NavigationTransitionInfo? infoOverride)
    {
        if (_allNavItemCtx.TryGetValue(guid, out var context) is false)
        {
            return;
        }

        var beforeNavItemCtx = GetCurrentNavItemCtx();
        infoOverride ??= new DrillInNavigationTransitionInfo();

        _rootFrame.Tag = guid;
        Debug2.Assert(context.Metadata.PageType is not null);
        if (_rootFrame.Navigate(context.Metadata.PageType, parameter, infoOverride))
        {
            if (beforeNavItemCtx is not null)
            {
                await beforeNavItemCtx.Value.OnNavigatedFromAsync();
            }
        }
    }
}
