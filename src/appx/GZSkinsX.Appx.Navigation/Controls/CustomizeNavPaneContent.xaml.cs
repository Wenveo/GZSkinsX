// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.Themes;
using GZSkinsX.Api.WindowManager;
using GZSkinsX.DotNet.Diagnostics;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Appx.Navigation.Controls;

public sealed partial class CustomizeNavPaneContent : UserControl
{
    private readonly NavigationService _navigationService;
    private readonly IThemeService _themeService;

    internal CustomizeNavPaneContent(NavigationService navigationService)
    {
        _navigationService = navigationService;
        _themeService = AppxContext.ThemeService;

        InitializeComponent();
    }

    private void OnMainSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.SuggestionChosen)
        {
            var suggestions = new List<QueryNavigationItem>();

            var querySplit = sender.Text.Split(" ");
            foreach (var item in _navigationService._createdNavItems.Values)
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
                            if (item.Icon is FontIcon fontIcon)
                            {
                                suggestions.Add(new(header, fontIcon.Glyph,
                                    fontIcon.FontFamily, (Guid)item.Tag));
                            }
                            else
                            {
                                suggestions.Add(new(header, string.Empty,
                                    item.FontFamily, (Guid)item.Tag));
                            }

                            break;
                        }
                    }
                }
            }

            sender.ItemsSource = suggestions.Count > 0 ? suggestions
                : (new string[] { _navigationService.GetLocalizedOrDefault("resx:GZSkinsX.Appx.Navigation/Resources/MainSearchBox_Query_NotResultsFound") });
        }
    }

    private async void OnMainSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is QueryNavigationItem queryNavigationItem)
        {
            await _navigationService.NavigateCoreAsync(queryNavigationItem.Guid, null, null);
        }
    }

    private void OnSettingsInvoke(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        AppxContext.ServiceLocator.Resolve<IWindowManagerService>().NavigateTo(WindowFrameConstants.Preload_Guid);
    }

    private void OnControlFInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        MainSearchBox.Focus(FocusState.Keyboard);
    }

    private void OnThemeMenuFlyoutOpening(object sender, object e)
    {
        switch (_themeService.CurrentTheme)
        {
            case ElementTheme.Light:
                MenuFlyout_LightTheme_MenuItem.IsChecked = true;
                break;
            case ElementTheme.Dark:
                MenuFlyout_DarkTheme_MenuItem.IsChecked = true;
                break;
            default:
                MenuFlyout_DefaultTheme_MenuItem.IsChecked = true;
                break;
        }
    }

    private async void OnSwitchTheme(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        if (args.Parameter is ElementTheme newTheme &&
            _themeService.CurrentTheme != newTheme)
        {
            await _themeService.SetElementThemeAsync(newTheme);
        }
    }
}

internal record class QueryNavigationItem(string Title, string Glyph, FontFamily FontFamily, Guid Guid)
{
    public override string ToString() => Title;
}
