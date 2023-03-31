// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Numerics;

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
    public NavigationRootPage()
    {
        InitializeComponent();
        Window.Current.SetTitleBar(AppTitleBar);
    }

    internal void Initialize(INavigationService navigationService)
    {
        var navService = (NavigationService)navigationService;
        navService._navigationViewRoot.DisplayModeChanged += OnDisplayModeChanged;
        navService._navigationViewRoot.PaneClosing += OnPaneClosing;
        navService._navigationViewRoot.PaneOpening += OnPaneOpening;
        contentPresenter.Content = navService._navigationViewRoot;
    }

    private void OnPaneClosing(MUXC.NavigationView sender, MUXC.NavigationViewPaneClosingEventArgs args)
    {
        UpdateAppTitleMargin(sender);
    }

    private void OnPaneOpening(MUXC.NavigationView sender, object args)
    {
        UpdateAppTitleMargin(sender);
    }

    private void OnDisplayModeChanged(MUXC.NavigationView sender, MUXC.NavigationViewDisplayModeChangedEventArgs args)
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

    private void UpdateAppTitleMargin(MUXC.NavigationView sender)
    {
        const int smallLeftIndent = 4, largeLeftIndent = 24;

        var leftIndent = (sender.DisplayMode == MUXC.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen)
            || sender.DisplayMode == MUXC.NavigationViewDisplayMode.Minimal ? smallLeftIndent : largeLeftIndent;

        // 1809
        if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
        {
            AppTitle.TranslationTransition = new Vector3Transition();
            AppTitle.Translation = new Vector3(leftIndent, 0, 0);
        }
        else
        {
            var currMargin = AppTitle.Margin;
            AppTitle.Margin = new Thickness()
            {
                Left = leftIndent,
                Top = currMargin.Top,
                Right = currMargin.Right,
                Bottom = currMargin.Bottom
            };
        }
    }
}
