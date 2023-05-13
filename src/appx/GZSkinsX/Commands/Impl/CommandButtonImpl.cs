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
using Windows.UI.Xaml.Input;

namespace GZSkinsX.Commands.Impl;

internal sealed class CommandButtonImpl
{
    private readonly ICommandButton _commandButton;
    private readonly AppBarButton _appBarButton;

    public AppBarButton UIObject => _appBarButton;

    public CommandButtonImpl(ICommandButton commandButton)
    {
        _commandButton = commandButton;
        _appBarButton = new AppBarButton();
        _appBarButton.Click += _commandButton.OnClick;

        var notifyPropertyChanged = _commandButton as INotifyPropertyChanged;
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
            case nameof(ICommandButton.DisplayName):
                UpdateDisplayName();
                break;

            case nameof(ICommandButton.Icon):
                UpdateIcon();
                break;

            case nameof(ICommandButton.IsEnabled):
                UpdateIsEnabled();
                break;

            case nameof(ICommandButton.IsVisible):
                UpdateIsVisible();
                break;

            case nameof(ICommandButton.ShortcutKey):
                UpdateShortcutKey();
                break;

            case nameof(ICommandButton.ToolTip):
                UpdateToolTip();
                break;

            default:
                break;
        }
    }

    private void UpdateDisplayName()
    {
        var displayName = _commandButton.DisplayName!;
        if (!StringComparer.Ordinal.Equals(_appBarButton.Label, displayName))
        {
            var localizedName = ResourceHelper.GetResxLocalizedOrDefault(displayName);
            AutomationProperties.SetName(_appBarButton, localizedName);
            _appBarButton.Label = localizedName;
        }
    }

    private void UpdateIcon()
    {
        var icon = _commandButton.Icon;
        if (icon != _appBarButton.Icon)
        {
            _appBarButton.Icon = icon;
        }
    }

    private void UpdateIsEnabled()
    {
        var isEnabled = _commandButton.IsEnabled;
        if (isEnabled != _appBarButton.IsEnabled)
        {
            _appBarButton.IsEnabled = isEnabled;
        }
    }

    private void UpdateIsVisible()
    {
        var isVisible = _commandButton.IsVisible;
        if (isVisible != BoolToVisibilityConvert.ToBoolean(_appBarButton.Visibility))
        {
            _appBarButton.Visibility = BoolToVisibilityConvert.ToVisibility(isVisible);
        }
    }

    private void UpdateShortcutKey()
    {
        var collection = _appBarButton.KeyboardAccelerators;
        var shortcutKey = _commandButton.ShortcutKey;
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
                collection.Add(new KeyboardAccelerator
                {
                    Key = shortcutKey.Key,
                    Modifiers = shortcutKey.Modifiers
                });
            }
        }
        else
        {
            collection.Clear();
        }
    }

    private void UpdateToolTip()
    {
        var toolTip = _commandButton.ToolTip;
        if (toolTip is not null)
        {
            if (toolTip is string str)
                ToolTipService.SetToolTip(_appBarButton, ResourceHelper.GetResxLocalizedOrDefault(str));
            else
                ToolTipService.SetToolTip(_appBarButton, toolTip);
        }
        else
        {
            ToolTipService.SetToolTip(_appBarButton, null);
        }
    }

    private void UpdateUIState()
    {
        UpdateDisplayName();
        UpdateIcon();
        UpdateIsEnabled();
        UpdateIsVisible();
        UpdateShortcutKey();
        UpdateToolTip();
    }
}
