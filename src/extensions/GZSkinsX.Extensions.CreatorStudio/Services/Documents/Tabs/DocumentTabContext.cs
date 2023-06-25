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

using GZSkinsX.Extensions.CreatorStudio.Contracts.Documents.Tabs;

using GZSkinsX.Api.Helpers;

using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Media.Animation;

using MUXC = Microsoft.UI.Xaml.Controls;
using WUXC = Windows.UI.Xaml.Controls;

namespace GZSkinsX.Extensions.CreatorStudio.Services.Documents.Tabs;

internal sealed class DocumentTabContext
{
    private sealed class AsyncLoadedUIProvider : IDisposable
    {
        private readonly DocumentTabContext _parent;
        private readonly WUXC.Grid _loadingMask;

        public AsyncLoadedUIProvider(DocumentTabContext parent)
        {
            _loadingMask = new WUXC.Grid()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var textBlock = new WUXC.TextBlock
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

            WUXC.Grid.SetRow(textBlock, 0);
            WUXC.Grid.SetRow(progressBar, 1);

            _loadingMask.RowDefinitions.Add(new WUXC.RowDefinition { Height = GridLength.Auto });
            _loadingMask.RowDefinitions.Add(new WUXC.RowDefinition { Height = GridLength.Auto });
            _loadingMask.RowSpacing = 12d;

            _loadingMask.Children.Add(textBlock);
            _loadingMask.Children.Add(progressBar);

            _parent = parent;
            _parent._contentPresenter.Content = _loadingMask;
        }

        public async Task LoadUIAsync()
        {
            var tabContent = _parent._tab.Content;
            if (tabContent is not null)
            {
                tabContent.OnInitialize();

                var tabContent2 = tabContent as IDocumentTabContent2;
                if (tabContent2 is not null)
                {
                    await tabContent2.OnInitializeAsync();
                }
            }
        }

        public void Dispose()
        {
            if (_parent._tab.Content is not null)
            {
                _parent._contentPresenter.Content = _parent._tab.Content.UIObject;
            }
        }
    }

    internal readonly IDocumentTab _tab;
    private readonly MUXC.TabViewItem _tabViewItem;
    private readonly WUXC.ContentPresenter _contentPresenter;

    private bool _isInitialized;

    public MUXC.TabViewItem UIObject => _tabViewItem;

    public DocumentTabContext(IDocumentTab tab, WUXC.MenuFlyout menuFlyout)
    {
        _tab = tab;

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
            ContextFlyout = menuFlyout,
            DataContext = this
        };
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateTitle();
        UpdateIconSource();
        UpdateToolTip();

        if (!_isInitialized)
        {
            using var provider = new AsyncLoadedUIProvider(this);
            await provider.LoadUIAsync();

            _isInitialized = true;
        }

        if (_tab is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged += OnTabPropertyChanged;
        }

        if (_tab.Content is INotifyPropertyChanged notifyPropertyChanged2)
        {
            notifyPropertyChanged2.PropertyChanged += OnTabContentPropertyChanged;
        }
    }

    private void OnTabContentPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        void UpdateUIObject()
        {
            if (_tab.Content is null)
            {
                return;
            }

            var uiOjbect = _tab.Content.UIObject;
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

        switch (e.PropertyName)
        {
            case nameof(IDocumentTabContent.UIObject):
                UpdateUIObject();
                break;

            default:
                break;
        }
    }

    private void OnTabPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IDocumentTab.Title):
                UpdateTitle();
                break;

            case nameof(IDocumentTab.IconSource):
                UpdateIconSource();
                break;

            case nameof(IDocumentTab.ToolTip):
                UpdateToolTip();
                break;

            default:
                break;
        }
    }

    private void UpdateTitle()
    {
        var title = _tab.Title!;
        if (!StringComparer.Ordinal.Equals(_tabViewItem.Header, title))
        {
            var localizedName = ResourceHelper.GetResxLocalizedOrDefault(title);
            AutomationProperties.SetName(_tabViewItem, localizedName);
            _tabViewItem.Header = localizedName;
        }
    }

    private void UpdateIconSource()
    {
        var iconSource = _tab.IconSource;
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
        var toolTip = _tab.ToolTip;
        if (toolTip is not null)
        {
            if (toolTip is string str)
                WUXC.ToolTipService.SetToolTip(_tabViewItem, ResourceHelper.GetResxLocalizedOrDefault(str));
            else
                WUXC.ToolTipService.SetToolTip(_tabViewItem, toolTip);
        }
        else
        {
            WUXC.ToolTipService.SetToolTip(_tabViewItem, null);
        }
    }

    internal void InternalOnAdded()
    {
        _tabViewItem.Loaded += OnLoaded;
        _tab.OnAdded();
    }

    internal void InternalOnRemoved()
    {
        if (_tab is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged -= OnTabPropertyChanged;
        }

        if (_tab.Content is INotifyPropertyChanged notifyPropertyChanged2)
        {
            notifyPropertyChanged2.PropertyChanged -= OnTabContentPropertyChanged;
        }

        _tabViewItem.Loaded -= OnLoaded;
        _tab.OnRemoved();
    }
}
