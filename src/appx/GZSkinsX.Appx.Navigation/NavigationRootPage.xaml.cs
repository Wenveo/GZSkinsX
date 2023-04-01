// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Numerics;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Navigation;

using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MUXC = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.Navigation;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class NavigationRootPage : Page
{
    private NavigationService? _navigationService;
    private IAppxTitleBar? _appxTitleBar;

    public NavigationRootPage()
    {
        InitializeComponent();
    }

    internal void OnLoaded(INavigationService navigationService, IAppxTitleBar appxTitleBar)
    {
        _navigationService = (NavigationService)navigationService;
        _navigationService._navigationViewRoot.DisplayModeChanged += OnNavDisplayModeChanged;
        _navigationService._navigationViewRoot.PaneClosing += OnNavPaneClosing;
        _navigationService._navigationViewRoot.PaneOpening += OnNavPaneOpening;
        contentPresenter.Content = _navigationService._navigationViewRoot;

        _appxTitleBar = appxTitleBar;
        _appxTitleBar.SetTitleBar(AppTitleBar);
    }

    internal void OnUnloaded()
    {
        if (_navigationService is not null)
        {
            _navigationService._navigationViewRoot.DisplayModeChanged -= OnNavDisplayModeChanged;
            _navigationService._navigationViewRoot.PaneClosing -= OnNavPaneClosing;
            _navigationService._navigationViewRoot.PaneOpening -= OnNavPaneOpening;
        }

        _appxTitleBar?.SetTitleBar(null);
    }

    private void OnNavPaneClosing(MUXC.NavigationView sender, MUXC.NavigationViewPaneClosingEventArgs args)
    {
        UpdateAppTitleMargin(sender);
    }

    private void OnNavPaneOpening(MUXC.NavigationView sender, object args)
    {
        UpdateAppTitleMargin(sender);
    }

    private void OnNavDisplayModeChanged(MUXC.NavigationView sender, MUXC.NavigationViewDisplayModeChangedEventArgs args)
    {
        var currMargin = AppTitleBar.Margin;
        var leftMargin = sender.DisplayMode == MUXC.NavigationViewDisplayMode.Minimal
            ? sender.CompactPaneLength * 2 : sender.CompactPaneLength;

        AppTitleBar.Margin = new Thickness
        {
            Left = leftMargin,
            Top = currMargin.Top,
            Right = currMargin.Right,
            Bottom = currMargin.Bottom
        };

        UpdateAppTitleMargin(sender);
    }

    private readonly Vector3 _smallLeftIndent = new(4f, 0f, 0f);
    private readonly Vector3 _largeLeftIndent = new(24f, 0f, 0f);

    private void UpdateAppTitleMargin(MUXC.NavigationView sender)
    {
        var leftIndent = (sender.DisplayMode == MUXC.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen)
            || sender.DisplayMode == MUXC.NavigationViewDisplayMode.Minimal ? _smallLeftIndent : _largeLeftIndent;

        // 1809
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            AppTitle.TranslationTransition ??= new Vector3Transition();
            AppTitle.Translation = leftIndent;
        }
        else
        {
            var currMargin = AppTitle.Margin;
            AppTitle.Margin = new Thickness()
            {
                Left = leftIndent.X,
                Top = currMargin.Top,
                Right = currMargin.Right,
                Bottom = currMargin.Bottom
            };
        }
    }
}
