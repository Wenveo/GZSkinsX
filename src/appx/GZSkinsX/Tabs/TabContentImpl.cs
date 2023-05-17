// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.Tabs;

using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Tabs;

internal sealed class TabContentImpl
{
    private sealed class AsyncLoadedUIProvider : IDisposable
    {
        private readonly TabContentImpl _impl;
        private readonly Grid _loadingMask;

        public AsyncLoadedUIProvider(TabContentImpl impl)
        {
            _loadingMask = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var textBlock = new TextBlock
            {
                FontSize = 20d,
                FontWeight = FontWeights.ExtraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = "Loading"
            };

            var progressBar = new MUXC.ProgressBar
            {
                Height = 24,
                MinWidth = 200d,
                IsIndeterminate = true
            };

            Grid.SetRow(textBlock, 0);
            Grid.SetRow(progressBar, 1);

            _loadingMask.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _loadingMask.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _loadingMask.RowSpacing = 12d;

            _loadingMask.Children.Add(textBlock);
            _loadingMask.Children.Add(progressBar);

            _impl = impl;
            _impl._contentPresenter.Content = _loadingMask;
        }

        public async Task LoadUIAsync()
        {
            _impl.Tab.OnInitialize();

            _impl.UpdateTitle();
            _impl.UpdateIconSource();
            _impl.UpdateToolTip();

            await _impl.Tab.OnInitializeAsync();

            _impl.UpdateTitle();
            _impl.UpdateIconSource();
            _impl.UpdateToolTip();
        }

        public void Dispose()
        {
            _impl._contentPresenter.Content = _impl.Tab.UIObject;
        }
    }

    private readonly TabViewManager _owner;
    private readonly ITabContent _tabContent;
    private readonly MUXC.TabViewItem _tabViewItem;
    private readonly ContentPresenter _contentPresenter;
    private bool _isInitialized;

    public ITabContent Tab => _tabContent;

    public MUXC.TabViewItem UIObject => _tabViewItem;

    public TabContentImpl(TabViewManager owner, ITabContent tabContent)
    {
        _owner = owner;
        _tabContent = tabContent;

        _contentPresenter = new()
        {
            ContentTransitions = new TransitionCollection
            {
                new EntranceThemeTransition()
            }
        };

        _tabViewItem = new()
        {
            Content = _contentPresenter,
            ContextFlyout = _owner._contextMenu,
            DataContext = this
        };

        if (_owner._options.TabViewItemStyle is not null)
            _tabViewItem.Style = _owner._options.TabViewItemStyle;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!_isInitialized)
        {
            using var provider = new AsyncLoadedUIProvider(this);
            await provider.LoadUIAsync();

            _isInitialized = true;
        }

        if (_tabContent is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
        }
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
            _contentPresenter.Content = null;
        }
        else
        {
            if (uiOjbect != _contentPresenter.Content)
            {
                _contentPresenter.Content = uiOjbect;
            }
        }
    }

    internal void InternalOnAdded()
    {
        _tabViewItem.Loaded += OnLoaded;
        _tabContent.OnAdded();
    }

    internal void InternalOnRemoved()
    {
        if (_tabContent is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
        }

        _tabViewItem.Loaded -= OnLoaded;
        _tabContent.OnRemoved();
    }
}
