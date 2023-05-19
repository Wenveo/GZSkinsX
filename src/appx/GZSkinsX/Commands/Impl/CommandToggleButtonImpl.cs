// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.ComponentModel;

using GZSkinsX.Api.Commands;
using GZSkinsX.Api.Helpers;
using GZSkinsX.Api.Utilities;

using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Commands.Impl;

internal sealed class CommandToggleButtonImpl
{
    private readonly ICommandToggleButton _commandToggleButton;
    private readonly AppBarToggleButton _appBarToggleButton;

    public AppBarToggleButton UIObject => _appBarToggleButton;

    public CommandToggleButtonImpl(ICommandToggleButton commandToggleButton)
    {
        _commandToggleButton = commandToggleButton;

        _appBarToggleButton = new AppBarToggleButton();
        _appBarToggleButton.Click += _commandToggleButton.OnClick;
        _appBarToggleButton.Checked += _commandToggleButton.OnChecked;
        _appBarToggleButton.Unchecked += _commandToggleButton.OnUnchecked;

        _appBarToggleButton.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        _appBarToggleButton.Loaded -= OnLoaded;
        _commandToggleButton.OnInitialize();

        var notifyPropertyChanged = _commandToggleButton as INotifyPropertyChanged;
        if (notifyPropertyChanged is not null)
        {
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
        }

        UpdateUIState();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ICommandToggleButton.DisplayName):
                UpdateDisplayName();
                break;

            case nameof(ICommandToggleButton.Icon):
                UpdateIcon();
                break;

            case nameof(ICommandToggleButton.IsChecked):
                UpdateIsChecked();
                break;

            case nameof(ICommandToggleButton.IsEnabled):
                UpdateIsEnabled();
                break;

            case nameof(ICommandToggleButton.IsVisible):
                UpdateIsVisible();
                break;

            case nameof(ICommandToggleButton.ShortcutKey):
                UpdateShortcutKey();
                break;

            case nameof(ICommandToggleButton.ToolTip):
                UpdateToolTip();
                break;

            default:
                break;
        }
    }

    private void UpdateDisplayName()
    {
        var displayName = _commandToggleButton.DisplayName!;
        if (!StringComparer.Ordinal.Equals(_appBarToggleButton.Label, displayName))
        {
            var localizedName = ResourceHelper.GetResxLocalizedOrDefault(displayName);
            AutomationProperties.SetName(_appBarToggleButton, localizedName);
            _appBarToggleButton.Label = localizedName;
        }
    }

    private void UpdateIcon()
    {
        var icon = _commandToggleButton.Icon;
        if (icon != _appBarToggleButton.Icon)
        {
            _appBarToggleButton.Icon = icon;
        }
    }

    private void UpdateIsChecked()
    {
        var isChecked = _commandToggleButton.IsChecked;
        if (isChecked != _appBarToggleButton.IsChecked)
        {
            _appBarToggleButton.IsChecked = _commandToggleButton.IsChecked;
        }
    }

    private void UpdateIsEnabled()
    {
        var isEnabled = _commandToggleButton.IsEnabled;
        if (isEnabled != _appBarToggleButton.IsEnabled)
        {
            _appBarToggleButton.IsEnabled = isEnabled;
        }
    }

    private void UpdateIsVisible()
    {
        var isVisible = _commandToggleButton.IsVisible;
        if (isVisible != BoolToVisibilityConvert.ToBoolean(_appBarToggleButton.Visibility))
        {
            _appBarToggleButton.Visibility = BoolToVisibilityConvert.ToVisibility(isVisible);
        }
    }

    private void UpdateShortcutKey()
    {
        var collection = _appBarToggleButton.KeyboardAccelerators;
        var shortcutKey = _commandToggleButton.ShortcutKey;
        if (shortcutKey is not null)
        {
            if (collection.Count == 1)
            {
                var keyboardAccelerator = collection[0];

                var newKey = shortcutKey.Key;
                if (newKey != keyboardAccelerator.Key)
                {
                    keyboardAccelerator.Key = newKey;
                }

                var newModifiers = shortcutKey.Modifiers;
                if (newModifiers != keyboardAccelerator.Modifiers)
                {
                    keyboardAccelerator.Modifiers = newModifiers;
                }
            }
            else
            {
                collection.Clear();
                collection.Add(shortcutKey);
            }
        }
        else
        {
            collection.Clear();
        }
    }

    private void UpdateToolTip()
    {
        var toolTip = _commandToggleButton.ToolTip;
        if (toolTip is not null)
        {
            if (toolTip is string str)
                ToolTipService.SetToolTip(_appBarToggleButton, ResourceHelper.GetResxLocalizedOrDefault(str));
            else
                ToolTipService.SetToolTip(_appBarToggleButton, toolTip);
        }
        else
        {
            ToolTipService.SetToolTip(_appBarToggleButton, null);
        }
    }

    private void UpdateUIState()
    {
        UpdateDisplayName();
        UpdateIcon();
        UpdateIsChecked();
        UpdateIsEnabled();
        UpdateIsVisible();
        UpdateShortcutKey();
        UpdateToolTip();
    }
}
