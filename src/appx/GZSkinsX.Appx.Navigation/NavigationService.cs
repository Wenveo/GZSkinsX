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
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using GZSkinsX.Api.Navigation;
using GZSkinsX.DotNet.Diagnostics;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;

using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Navigation;

/// <inheritdoc cref="INavigationService"/>
[Shared, Export(typeof(INavigationService))]
internal sealed class NavigationService : INavigationService
{
    /// <summary>
    /// 
    /// </summary>
    private readonly IEnumerable<Lazy<INavigationGroup, NavigationGroupMetadataAttribute>> _mefNavGroups;

    /// <summary>
    /// 
    /// </summary>
    private readonly IEnumerable<Lazy<INavigationItem, NavigationItemMetadataAttribute>> _mefNavItems;

    /// <summary>
    /// 
    /// </summary>
    private readonly Dictionary<Guid, List<NavigationItemContext>> _guidToNavItems;

    /// <summary>
    /// 
    /// </summary>
    private readonly Dictionary<Guid, MUXC.NavigationViewItem> _createdNavItems;

    /// <summary>
    /// 
    /// </summary>
    private readonly Dictionary<Guid, NavigationItemContext> _allNavItemCtx;

    /// <summary>
    /// 
    /// </summary>
    private readonly List<NavigationGroupContext> _navGroups;

    /// <summary>
    /// 
    /// </summary>
    internal readonly MUXC.NavigationView _navigationViewRoot;

    /// <summary>
    /// 
    /// </summary>
    internal readonly Frame _rootFrame;

    /// <inheritdoc/>
    public event NavigatedEventHandler? Navigated;

    /// <inheritdoc/>
    public bool CanGoBack => _rootFrame.CanGoBack;

    /// <inheritdoc/>
    public bool CanGoForward => _rootFrame.CanGoForward;

    /// <summary>
    /// 
    /// </summary>
    [ImportingConstructor]
    public NavigationService(
        [ImportMany] IEnumerable<Lazy<INavigationGroup, NavigationGroupMetadataAttribute>> mefNavGroups,
        [ImportMany] IEnumerable<Lazy<INavigationItem, NavigationItemMetadataAttribute>> mefNavItems)
    {
        _mefNavGroups = mefNavGroups;
        _mefNavItems = mefNavItems;

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
    /// 
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
    /// 
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
    /// 
    /// </summary>
    private void InitializeUIObject()
    {
        Canvas.SetZIndex(_navigationViewRoot, 0);
        _navigationViewRoot.Content = _rootFrame;
        _navigationViewRoot.AutoSuggestBox = CreateAutoSuggestBox();
        _navigationViewRoot.DisplayModeChanged += OnNavDisplayModeChanged;
        _navigationViewRoot.SelectionChanged += OnNavSelectionChanged;
        _navigationViewRoot.IsBackButtonVisible = MUXC.NavigationViewBackButtonVisible.Auto;
        _navigationViewRoot.IsTabStop = false;
        _navigationViewRoot.IsSettingsVisible = false;
        _navigationViewRoot.IsTitleBarAutoPaddingEnabled = false;
        _navigationViewRoot.Resources.MergedDictionaries.Add(new()
        {
            Source = new Uri("ms-appx:///GZSkinsX.Appx.Navigation/Styles/Default.xaml")
        });

        _rootFrame.Style = (Style)_navigationViewRoot.Resources["MainFrameStyle"];
        _rootFrame.Navigated += OnNavigated;

        InitializeNavView(_navigationViewRoot);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="navigationView"></param>
    private void InitializeNavView(MUXC.NavigationView navigationView)
    {
        var needSeparator = false;
        foreach (var group in _navGroups)
        {
            var container = group.Metadata.Placement == NavigationItemPlacement.Footer
                ? navigationView.FooterMenuItems
                : navigationView.MenuItems;

            var guid = new Guid(group.Metadata.Guid);
            if (_guidToNavItems.TryGetValue(guid, out var navItems))
            {
                if (navItems.Count == 0)
                    continue;

                if (needSeparator)
                    container.Add(new MUXC.NavigationViewItemSeparator());
                else
                    needSeparator = true;

                if (!string.IsNullOrEmpty(group.Metadata.Header))
                    container.Add(new MUXC.NavigationViewItemHeader { Content = group.Metadata.Header });

                foreach (var item in navItems)
                    container.Add(CreateNavItemUIObject(item));
            }
        }
    }

    private readonly Vector3 _smallTopIndent = new(0f, 0f, 0f);
    private readonly Vector3 _largeTopIndent = new(0f, 42f, 0f);
    private readonly Thickness _emptyPadding = new(0d, 0d, 0d, 0d);
    private readonly Thickness _framePadding = new(0d, 42d, 0d, 0d);
    private readonly CornerRadius _defaultCR = new(8d, 0d, 0d, 0d);
    private readonly CornerRadius _minimalCR = new(0d, 0d, 0d, 0d);

    /// <summary>
    /// 
    /// </summary>
    private void OnNavDisplayModeChanged(MUXC.NavigationView sender, MUXC.NavigationViewDisplayModeChangedEventArgs args)
    {
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            _rootFrame.TranslationTransition ??= new Vector3Transition();
            if (sender.DisplayMode == MUXC.NavigationViewDisplayMode.Minimal)
            {
                _rootFrame.CornerRadius = _minimalCR;
                _rootFrame.Translation = _smallTopIndent;
                _rootFrame.Padding = _framePadding;
            }
            else
            {
                _rootFrame.CornerRadius = _defaultCR;
                _rootFrame.Translation = _largeTopIndent;
                _rootFrame.Padding = _emptyPadding;
            }
        }
        else
        {
            if (sender.DisplayMode == MUXC.NavigationViewDisplayMode.Minimal)
            {
                _rootFrame.CornerRadius = _minimalCR;
                _rootFrame.Margin = _emptyPadding;
                _rootFrame.Padding = _framePadding;
            }
            else
            {
                _rootFrame.CornerRadius = _defaultCR;
                _rootFrame.Margin = _framePadding;
                _rootFrame.Padding = _emptyPadding;
            }
        }
    }

    /// <summary>
    /// 
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
    /// 
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

    private AutoSuggestBox CreateAutoSuggestBox()
    {
        var autoSuggestBox = new AutoSuggestBox
        {
            MinWidth = 200d,
            PlaceholderText = "Search",
            QueryIcon = new AnimatedIcon
            {
                Source = new AnimatedFindVisualSource(),
                FallbackIconSource = new MUXC.SymbolIconSource { Symbol = Symbol.Find }
            },
            VerticalAlignment = VerticalAlignment.Center,
            KeyboardAcceleratorPlacementMode = KeyboardAcceleratorPlacementMode.Hidden
        };

        var ctrlF = new KeyboardAccelerator { Key = VirtualKey.F, Modifiers = VirtualKeyModifiers.Control };
        ctrlF.Invoked += (_, _) => _navigationViewRoot.AutoSuggestBox?.Focus(FocusState.Programmatic);

        autoSuggestBox.KeyboardAccelerators.Add(ctrlF);
        autoSuggestBox.TextChanged += OnAutoSuggestBoxTextChanged;
        autoSuggestBox.QuerySubmitted += OnAutoSuggestBoxQuerySubmitted;

        return autoSuggestBox;
    }


    /// <summary>
    /// 
    /// </summary>
    void OnAutoSuggestBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.SuggestionChosen)
        {
            var suggestions = new List<QueryNavigationItem>();

            var querySplit = sender.Text.Split(" ");
            foreach (var item in _createdNavItems.Values)
            {
                if (item.SelectsOnInvoked)
                {
                    foreach (var queryToken in querySplit)
                    {
                        var header = item.Content as string;
                        Debug2.Assert(header is not null);
                        Debug2.Assert(header.Length > 0);

                        if (header.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) is not -1)
                        {
                            suggestions.Add(new QueryNavigationItem(header, (Guid)item.Tag));
                            break;
                        }
                    }
                }
            }

            if (suggestions.Count > 0)
            {
                sender.ItemsSource = suggestions.OrderByDescending(i => i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title);
            }
            else
            {
                sender.ItemsSource = new string[] { "No results found" };
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private async void OnAutoSuggestBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is QueryNavigationItem queryNavigationItem)
        {
            await NavigateCoreAsync(queryNavigationItem.Guid, null, null);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private MUXC.NavigationViewItem CreateNavItemUIObject(NavigationItemContext context)
    {
        var guid = new Guid(context.Metadata.Guid);
        var navItem = new MUXC.NavigationViewItem
        {
            Tag = guid,
            Icon = context.Value.Icon,
            Content = context.Metadata.Header,
            SelectsOnInvoked = context.Metadata.PageType is not null
        };

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

    private async Task NavigateCoreAsync(Guid guid, object? parameter, NavigationTransitionInfo? infoOverride)
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
