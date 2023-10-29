// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Helpers;
using GZSkinsX.Contracts.Navigation;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace GZSkinsX.Appx.Navigation;

/// <inheritdoc cref="INavigationViewManager"/>
internal sealed class NavigationViewManager : INavigationViewManager
{
    /// <summary>
    /// 提供用于呈现导航视图项的面板，以及页面内容。
    /// </summary>
    private readonly NavigationView _navigationView;

    /// <summary>
    /// 用于呈现页面内容以及实现导航功能。
    /// </summary>
    private readonly Frame _rootFrame;

    /// <summary>
    /// 有关搜索行为的搜索框控件。
    /// </summary>
    private readonly AutoSuggestBox? _searchBoxControl;

    /// <summary>
    /// 默认的搜索框占位文本。
    /// </summary>
    private readonly string? _defaultPlaceholderText;

    /// <summary>
    /// 存放用于接管搜索行为的对象。
    /// </summary>
    private INavigationViewSearchHolder? _searchHolder;

    /// <summary>
    /// 此成员用于帮助 OnNavigated 方法中快速枚举导航视图项。
    /// </summary>
    private NavigationViewItem? _tempNavItem;

    /// <inheritdoc/>
    public bool CanGoBack => _rootFrame.CanGoBack;

    /// <inheritdoc/>
    public bool CanGoForward => _rootFrame.CanGoForward;

    /// <inheritdoc/>
    public object UIObject => _navigationView;

    /// <inheritdoc/>
    public event NavigatedEventHandler? Navigated;

    /// <summary>
    /// 初始化 <see cref="NavigationViewManager"/> 的新实例。
    /// </summary>
    public NavigationViewManager(NavigationView navigationView, bool doNotCreateSearchBox)
    {
        _navigationView = navigationView;

        if (_navigationView.Content is not Frame frame)
            _navigationView.Content = frame = new Frame();

        _rootFrame = frame;
        _rootFrame.Navigated += OnNavigated;
        _navigationView.SelectionChanged += OnNavSelectionChanged;

        if (doNotCreateSearchBox is false)
        {
            string? placeholderText = null;
            if (navigationView is INavigationViewCustomSearchBox customSearchBox)
            {
                _searchBoxControl = customSearchBox.SearchBoxControl;
                placeholderText = customSearchBox.DefaultPlaceholderText;
            }
            else
            {
                _searchBoxControl = _navigationView.AutoSuggestBox ??=
                    new() { QueryIcon = new SegoeFluentIcon("\uE11A") };
            }

            _defaultPlaceholderText = placeholderText ?? ResourceHelper.GetLocalized(
                        "GZSkinsX.Appx.Navigation/Resources/NavigationView_SearchBoxControl_DefaultPlaceholderText");

            if (_searchBoxControl is not null)
            {
                _searchBoxControl.KeyboardAcceleratorPlacementMode = KeyboardAcceleratorPlacementMode.Hidden;
                _searchBoxControl.ItemTemplate = new QueryNavigationViewItemTemplate();
                _searchBoxControl.QuerySubmitted += OnSearchBoxControlQuerySubmitted;
                _searchBoxControl.TextChanged += OnSearchBoxControlTextChanged;

                var ctrlF_Hotkey = new KeyboardAccelerator
                {
                    Key = Windows.System.VirtualKey.F,
                    Modifiers = Windows.System.VirtualKeyModifiers.Control
                };

                ctrlF_Hotkey.Invoked += OnControlFInvoked;
                _searchBoxControl.KeyboardAccelerators.Add(ctrlF_Hotkey);
            }
        }
    }

    /// <summary>
    /// 在触发快捷键 Ctrl+F 时调用，并激活搜索框控件。 
    /// </summary>
    private void OnControlFInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        _searchBoxControl?.Focus(FocusState.Keyboard);
    }

    /// <summary>
    /// 在选择建议的导航项时触发的方法。
    /// </summary>
    private async void OnSearchBoxControlQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is QueryNavigationViewItem item)
        {
            await NavigateCoreAsync(item.GuidString, null, null);
        }
    }

    /// <summary>
    /// 在搜索框文本更改后触发的处理方法。
    /// </summary>
    private void OnSearchBoxControlTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason is not AutoSuggestionBoxTextChangeReason.SuggestionChosen)
        {
            var searchHolder = _searchHolder;
            if (searchHolder is not null)
            {
                searchHolder.OnSearchTextChanged(this, sender.Text);
                return;
            }

            var querySplit = sender.Text.Split(" ");
            var suggestions = QueryItems(_navigationView.MenuItems, querySplit)
                .Concat(QueryItems(_navigationView.FooterMenuItems, querySplit));

            if (suggestions.Any() is false)
                sender.ItemsSource = QueryNavigationViewItem.EmptyArray;
            else
                sender.ItemsSource = suggestions;
        }

        static IEnumerable<QueryNavigationViewItem> QueryItems(IList<object> menuItems, string[] tokens)
        {
            foreach (var item in menuItems.OfType<NavigationViewItem>())
            {
                if (NavigationViewItemHelper.GetItemContext(item) is { } ctx &&
                    item is { SelectsOnInvoked: true, Content: string header })
                {
                    foreach (var queryToken in tokens)
                    {
                        if (header.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) is not -1)
                        {
                            if (item.Icon is FontIcon fontIcon)
                            {
                                yield return new(header, fontIcon.Glyph, fontIcon.FontFamily, ctx.Metadata.Guid);
                            }
                            else
                            {
                                yield return new(header, string.Empty, item.FontFamily, ctx.Metadata.Guid);
                            }
                        }
                    }
                }

                foreach (var subItem in QueryItems(item.MenuItems, tokens))
                {
                    yield return subItem;
                }
            }
        }
    }

    /// <summary>
    /// 在导航至目标页面时触发，并同步导航视图中的选择项。
    /// <para>
    /// （当通过后台 Api 进行导航，而不是在 UI 上选择时，需要手动同步导航视图中的选择项。）
    /// </para>
    /// </summary>
    private async void OnNavigated(object sender, NavigationEventArgs e)
    {
        var item = _tempNavItem ?? FindNavItem(e.SourcePageType);
        if (item is not null && NavigationViewItemHelper.GetItemContext(item) is { } ctx)
        {
            await ctx.Value.OnNavigatedToAsync(e);
            _navigationView.SelectedItem = item;

            Navigated?.Invoke(sender, e);
            _tempNavItem = null;
        }

        if (_searchBoxControl is not null)
        {
            if (_rootFrame.Content is INavigationViewSearchHolder searchHolder)
            {
                _searchHolder = searchHolder;
                _searchBoxControl.ClearValue(ItemsControl.ItemsSourceProperty);
                _searchBoxControl.PlaceholderText = searchHolder.GetPlaceholderText();
            }
            else
            {
                _searchHolder = null;
                _searchBoxControl.PlaceholderText = _defaultPlaceholderText;
            }
        }
    }

    /// <summary>
    /// 在 <see cref="NavigationView"/> 中选择导航项时触发，并根据已选择的项进行导航操作。
    /// </summary>
    private void OnNavSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem { SelectsOnInvoked: true } navItem &&
            NavigationViewItemHelper.GetItemContext(navItem) is { } ctx)
        {
            _tempNavItem = navItem;
            _rootFrame.Navigate(ctx.Metadata.PageType);
        }
    }

    /// <summary>
    /// 在导航视图中枚举与指定的 <see cref="Guid"/> 所匹配的项。
    /// </summary>
    /// <param name="guidString">用于匹配导航项的 <see cref="Guid"/> 的字符串值。</param>
    /// <returns>如果查找到匹配的项便会将其返回，否则将返回 <see cref="null"/>。</returns>
    private NavigationViewItem? FindNavItem(string guidString)
    {
        static NavigationViewItem? FindSubNavItem(IEnumerable<object> items, string guidString)
        {
            foreach (var item in items.OfType<NavigationViewItem>())
            {
                if (NavigationViewItemHelper.GetItemContext(item) is { } ctx &&
                    StringComparer.OrdinalIgnoreCase.Equals(ctx.Metadata.Guid, guidString))
                {
                    return item;
                }

                var subItem = FindSubNavItem(item.MenuItems, guidString);
                if (subItem is not null)
                {
                    return subItem;
                }
            }

            return null;
        }

        return FindSubNavItem(_navigationView.MenuItems, guidString)
            ?? FindSubNavItem(_navigationView.FooterMenuItems, guidString);
    }

    /// <summary>
    /// 在导航视图中枚举与指定的页面类型 <see cref="Type"/> 所匹配的项。
    /// </summary>
    /// <param name="pageType">用于匹配导航项的 <see cref="Type"/> 页面类型。</param>
    /// <returns>如果查找到匹配的项便会将其返回，否则将返回 <see cref="null"/>。</returns>
    private NavigationViewItem? FindNavItem(Type pageType)
    {
        static NavigationViewItem? FindSubNavItem(IEnumerable<object> items, Type pageType)
        {
            foreach (var item in items.OfType<NavigationViewItem>())
            {
                if (NavigationViewItemHelper.GetItemContext(item) is { } ctx && ctx.Metadata.PageType == pageType)
                {
                    return item;
                }

                var subItem = FindSubNavItem(item.MenuItems, pageType);
                if (subItem is not null)
                {
                    return subItem;
                }
            }

            return null;
        }

        return FindSubNavItem(_navigationView.MenuItems, pageType)
            ?? FindSubNavItem(_navigationView.FooterMenuItems, pageType);
    }

    /// <summary>
    /// 获取与当前导航视图中的选中项所关联的 <see cref="NavigationItemMD"/> 上下文对象。
    /// </summary>
    /// <returns>如果找到所关联的 <see cref="NavigationItemMD"/> 上下文对象则会返回该实例，否则返回空。</returns>
    private NavigationItemMD? GetCurrentNavItemCtx()
    {
        if (_navigationView.SelectedItem is NavigationViewItem navItem &&
            NavigationViewItemHelper.GetItemContext(navItem) is { } ctx)
        {
            return ctx;
        }

        return null;
    }

    /// <inheritdoc/>
    public bool CanNavigate(NavigationViewNavigateArgs args)
    {
        if (args is { NavItemGuid: { } guidString })
        {
            return FindNavItem(guidString) is { } navItem &&
                   NavigationViewItemHelper.GetItemContext(navItem) != null;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool GoBack()
    {
        if (_rootFrame.CanGoBack)
        {
            var beforeNavItemCtx = GetCurrentNavItemCtx();
            _rootFrame.GoBack();

            beforeNavItemCtx?.Value.OnNavigatedFromAsync().FireAndForget();
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<bool> GoBackAsync()
    {
        if (_rootFrame.CanGoBack)
        {
            var beforeNavItemCtx = GetCurrentNavItemCtx();
            _rootFrame.GoBack();

            if (beforeNavItemCtx is not null)
                await beforeNavItemCtx.Value.OnNavigatedFromAsync();

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

            beforeNavItemCtx?.Value.OnNavigatedFromAsync().FireAndForget();
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<bool> GoForwardAsync()
    {
        if (_rootFrame.CanGoForward)
        {
            var beforeNavItemCtx = GetCurrentNavItemCtx();
            _rootFrame.GoForward();

            if (beforeNavItemCtx is not null)
                await beforeNavItemCtx.Value.OnNavigatedFromAsync();

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public void NavigateToFirstItem()
    {
        var itemToSelect = GetFirstNavItem(_navigationView.MenuItems) ??
                           GetFirstNavItem(_navigationView.FooterMenuItems);

        if (itemToSelect is not null)
        {
            _navigationView.SelectedItem = itemToSelect;
        }

        static NavigationViewItem? GetFirstNavItem(IEnumerable<object> items)
        {
            foreach (var item in items.OfType<NavigationViewItem>())
            {
                if (item.SelectsOnInvoked)
                {
                    return item;
                }

                var subItem = GetFirstNavItem(item.MenuItems);
                if (subItem is not null)
                {
                    return subItem;
                }
            }

            return null;
        }
    }

    /// <inheritdoc/>
    public async void NavigateTo(NavigationViewNavigateArgs args)
    {
        await NavigateCoreAsync(args.NavItemGuid, args.Parameter, args.NavigationTransitionInfo);
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString)
    {
        await NavigateCoreAsync(guidString, null, null);
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString, object parameter)
    {
        await NavigateCoreAsync(guidString, parameter, null);
    }

    /// <inheritdoc/>
    public async void NavigateTo(string guidString, object parameter, NavigationTransitionInfo infoOverride)
    {
        await NavigateCoreAsync(guidString, parameter, infoOverride);
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(NavigationViewNavigateArgs args)
    {
        await NavigateCoreAsync(args.NavItemGuid, args.Parameter, args.NavigationTransitionInfo);
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(string guidString)
    {
        await NavigateCoreAsync(guidString, null, null);
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(string guidString, object parameter)
    {
        await NavigateCoreAsync(guidString, parameter, null);
    }

    /// <inheritdoc/>
    public async Task NavigateToAsync(string guidString, object parameter, NavigationTransitionInfo infoOverride)
    {
        await NavigateCoreAsync(guidString, parameter, infoOverride);
    }

    /// <summary>
    /// 导航操作的核心方法实现。
    /// </summary>
    private async Task NavigateCoreAsync(string guidString, object? parameter, NavigationTransitionInfo? infoOverride)
    {
        var navItem = FindNavItem(guidString);
        if (navItem is not null && NavigationViewItemHelper.GetItemContext(navItem) is { } ctx)
        {
            _tempNavItem = navItem;
            _rootFrame.Tag = guidString;

            var beforeNavItemCtx = GetCurrentNavItemCtx();
            if (_rootFrame.Navigate(ctx.Metadata.PageType, parameter, infoOverride))
            {
                if (beforeNavItemCtx is not null)
                {
                    await beforeNavItemCtx.Value.OnNavigatedFromAsync();
                }
            }
        }
    }
}
