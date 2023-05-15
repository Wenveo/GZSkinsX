// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.ComponentModel;

using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.Tabs;

using Microsoft.UI.Xaml.Controls;

using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Tabs;

internal sealed class TabContentImpl
{
    private readonly TabViewManager _owner;
    private readonly ITabContent _tabContent;
    private readonly TabViewItem _tabViewItem;

    public ITabContent Tab => _tabContent;

    public TabViewItem UIObject => _tabViewItem;

    public TabContentImpl(TabViewManager owner, ITabContent tabContent)
    {
        _owner = owner;
        _tabContent = tabContent;
        _tabViewItem = new()
        {
            DataContext = this,
            ContextFlyout = _owner._contextMenu
        };

        if (_owner._options.TabViewItemStyle is not null)
            _tabViewItem.Style = _owner._options.TabViewItemStyle;

        _tabViewItem.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        _tabViewItem.Loaded -= OnLoaded;

        UpdateUIState();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ITabContent.Title):
                UpdateTitle();
                break;

            case nameof(ITabContent.IconSource):
                UpdateIconSource();
                break;

            case nameof(ITabContent.ToolTip):
                UpdateToolTip();
                break;

            case nameof(ITabContent.UIObject):
                UpdateUIObject();
                break;

            default:
                break;
        }
    }

    private void UpdateTitle()
    {
        var title = _tabContent.Title!;
        if (!StringComparer.Ordinal.Equals(_tabViewItem.Header, title))
        {
            var localizedName = ResourceHelper.GetResxLocalizedOrDefault(title);
            AutomationProperties.SetName(_tabViewItem, localizedName);
            _tabViewItem.Header = localizedName;
        }
    }

    private void UpdateIconSource()
    {
        var iconSource = _tabContent.IconSource;
        if (iconSource is null)
        {
            _tabViewItem.IconSource = null;
        }
        else
        {
            if (iconSource != _tabViewItem.IconSource)
            {
                _tabViewItem.IconSource = iconSource;
            }
        }
    }

    private void UpdateToolTip()
    {
        var toolTip = _tabContent.ToolTip;
        if (toolTip is not null)
        {
            if (toolTip is string str)
                ToolTipService.SetToolTip(_tabViewItem, ResourceHelper.GetResxLocalizedOrDefault(str));
            else
                ToolTipService.SetToolTip(_tabViewItem, toolTip);
        }
        else
        {
            ToolTipService.SetToolTip(_tabViewItem, null);
        }
    }

    private void UpdateUIObject()
    {
        var uiOjbect = _tabContent.UIObject;
        if (uiOjbect is null)
        {
            _tabViewItem.Content = null;
        }
        else
        {
            if (uiOjbect != _tabViewItem.Content)
            {
                _tabViewItem.Content = uiOjbect;
            }
        }
    }

    private void UpdateUIState()
    {
        UpdateTitle();
        UpdateIconSource();
        UpdateToolTip();
        UpdateUIObject();
    }

    internal void InternalOnAdded()
    {
        if (_tabContent is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
        }

        _tabContent.OnAdded();
    }

    internal void InternalOnRemoved()
    {
        if (_tabContent is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
        }

        _tabContent.OnRemoved();
    }
}
