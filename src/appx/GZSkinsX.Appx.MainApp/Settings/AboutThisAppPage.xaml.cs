// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Controls;
using GZSkinsX.Contracts.Extension;
using GZSkinsX.Contracts.Navigation;
using GZSkinsX.Contracts.Utilities;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.Appx.MainApp.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class AboutThisAppPage : Page
{
    public AboutThisAppPage()
    {
        InitializeComponent();
        VersionTextBlock.Text = AppxContext.AppxVersion.ToString();

        var extensions = AppxContext.Resolve<IEnumerable<Lazy<IExtension, ExtensionContractAttribute>>>();
        var configurations = extensions.Select(item => item.Value.ExtensionConfiguration);
        if (configurations.Any())
        {
            ExtensionsViewPanel.Visibility = Visibility.Visible;
            ExtensionsListView.SelectionChanged += (s, e) =>
            {
                PreviewSelectedItemHost.Visibility = BoolToVisibilityConvert.ToVisibility(ExtensionsListView.SelectedItem is not null);
            };
            ExtensionsListView.ItemsSource = configurations;
            ExtensionsListView.SelectedIndex = 0;
        }
    }

    private async void OnNavigateToPrivacy(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new("https://lolgezi.com/contract/"));
    }

    private async void OnNavigateToGitHubRepo(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new("https://github.com/Wenveo/GZSkinsX"));
    }

    private async void OnRateAndReview(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new("ms-windows-store://review/?PFN=61610AakStudio.64359D0915987_8fr87xx5s0whj"));
    }

    [Shared, NavigationItemContract(
        OwnerGuid = NavigationConstants.SETTINGS_NAV_GUID,
        Guid = NavigationConstants.SETTINGS_NAV_ABOUT_GUID,
        Group = NavigationConstants.GROUP_SETTINGS_NAV_DEFAULT,
        Order = NavigationConstants.ORDER_SETTINGS_NAV_ABOUT,
        Header = "resx:GZSkinsX.Appx.MainApp/Resources/Settings_AboutThisApp_Title",
        PageType = typeof(AboutThisAppPage))]
    private sealed class AboutThisAppItem : INavigationItem
    {
        public IconElement Icon => new SegoeFluentIcon { Glyph = "\uE946" };

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        public Task OnNavigatedToAsync(NavigationEventArgs args) => Task.CompletedTask;
    }
}
