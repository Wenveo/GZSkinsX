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
using System.Linq;

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.CreatorStudio.Commands;
using GZSkinsX.Api.MRT;

using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace GZSkinsX.Extensions.CreatorStudio.Commands;

[Shared, Export]
internal sealed class CommandBarService
{
    private readonly IEnumerable<Lazy<ICommandItem, CommandItemMetadataAttribute>> _mefItems;
    private readonly Dictionary<string, CommandItemGroupContext> _groupDict;

    private readonly IMRTCoreService _mrtCoreService;
    private readonly CommandBar _commandBar;

    internal CommandBar UIObject => _commandBar;

    [ImportingConstructor]
    public CommandBarService([ImportMany] IEnumerable<Lazy<ICommandItem, CommandItemMetadataAttribute>> mefItems)
    {
        _mefItems = mefItems;
        _mrtCoreService = AppxContext.MRTCoreService;

        _groupDict = new();
        _commandBar = new();

        InitializeGroups();
        InitializeUIObject();
    }

    private void InitializeGroups()
    {
        static bool ParseGroup(string group, out string name, out double order)
        {
            var indexOfSeparator = group.IndexOf(',');
            if (indexOfSeparator == -1 || !double.TryParse(group[..indexOfSeparator++], out order))
            {
                name = string.Empty;
                order = double.NaN;
                return false;
            }

            name = group[indexOfSeparator..];
            return true;
        }

        foreach (var item in _mefItems)
        {
            var groupName = item.Metadata.Group;
            if (!_groupDict.TryGetValue(groupName, out var itemGroup))
            {
                if (ParseGroup(item.Metadata.Group, out var name, out var order))
                {
                    itemGroup = new CommandItemGroupContext(name, order);
                    _groupDict.Add(groupName, itemGroup);
                }
                else
                {
                    continue;
                }
            }

            itemGroup.Items.Add(new CommandItemContext(item));
        }

        foreach (var group in _groupDict.Values)
        {
            group.Items.Sort((a, b) => a.Metadata.Order.CompareTo(b.Metadata.Order));
        }
    }

    private void InitializeUIObject()
    {
        int AddItems(
            IObservableVector<ICommandBarElement> collection,
            IEnumerable<CommandItemContext> items)
        {
            var count = 0;
            foreach (var item in items)
            {
                var value = item.Value;
                var metadata = item.Metadata;

                if (value is ICommandButton button)
                    collection.Add(CreateAppBarButton(button, metadata));
                else if (value is ICommandToggleButton toggleButton)
                    collection.Add(CreateAppBarToggleButton(toggleButton, metadata));
                else if (value is ICommandObject uiObject)
                    collection.Add(CreateElementContainer(uiObject, metadata));
                else
                    continue;

                count++;
            }

            return count;
        }

        _commandBar.HorizontalAlignment = HorizontalAlignment.Left;
        _commandBar.HorizontalContentAlignment = HorizontalAlignment.Center;
        _commandBar.VerticalContentAlignment = VerticalAlignment.Center;
        _commandBar.DefaultLabelPosition = CommandBarDefaultLabelPosition.Right;
        _commandBar.Background = new SolidColorBrush(Colors.Transparent);
        _commandBar.IsOpen = false;

        var orderedGroups = _groupDict.Values.OrderBy(a => a.Order);

        var primaryNeedSeparator = false;
        var secondaryNeedSeparator = false;

        foreach (var group in orderedGroups)
        {
            if (primaryNeedSeparator)
                _commandBar.PrimaryCommands.Add(new AppBarSeparator());
            if (secondaryNeedSeparator)
                _commandBar.SecondaryCommands.Add(new AppBarSeparator());

            primaryNeedSeparator = AddItems(_commandBar.PrimaryCommands,
                group.Items.Where(item => item.Metadata.Placement == CommandPlacement.Primary)) > 0;

            secondaryNeedSeparator = AddItems(_commandBar.SecondaryCommands,
                group.Items.Where(item => item.Metadata.Placement == CommandPlacement.Secondary)) > 0;
        }
    }

    private AppBarButton CreateAppBarButton(ICommandButton item, CommandItemMetadataAttribute metadata)
    {
        var button = new AppBarButton { Tag = item };

        var displayName = item.DisplayName;
        if (!string.IsNullOrEmpty(displayName))
            button.Label = GetLocalizedOrDefault(displayName!);

        var toolTip = item.ToolTip;
        if (!string.IsNullOrEmpty(toolTip))
            ToolTipService.SetToolTip(button, GetLocalizedOrDefault(toolTip!));

        var iconElement = item.Icon;
        if (iconElement is not null)
            button.Icon = iconElement;

        var commandHotKey = item.HotKey;
        if (commandHotKey is not null)
            button.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = commandHotKey.Key, Modifiers = commandHotKey.Modifiers });

        button.IsEnabled = item.IsEnabled();
        button.Visibility = Bool2Visibility(item.IsVisible());

        static void OnClickEvent(object sender, RoutedEventArgs e)
        {
            var self = (AppBarButton)sender;
            var item = (ICommandButton)self.Tag;

            item.OnClick(new CommandUIContext(sender, e));
        }

        button.Click += OnClickEvent;

        return button;
    }

    private AppBarToggleButton CreateAppBarToggleButton(ICommandToggleButton item, CommandItemMetadataAttribute metadata)
    {
        var toggleButton = new AppBarToggleButton { Tag = item };

        var displayName = item.DisplayName;
        if (!string.IsNullOrEmpty(displayName))
            toggleButton.Label = GetLocalizedOrDefault(displayName!);

        var toolTip = item.ToolTip;
        if (!string.IsNullOrEmpty(toolTip))
            ToolTipService.SetToolTip(toggleButton, GetLocalizedOrDefault(toolTip!));

        var iconElement = item.Icon;
        if (iconElement is not null)
            toggleButton.Icon = iconElement;

        var commandHotKey = item.HotKey;
        if (commandHotKey is not null)
            toggleButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = commandHotKey.Key, Modifiers = commandHotKey.Modifiers });

        toggleButton.IsEnabled = item.IsEnabled();
        toggleButton.Visibility = Bool2Visibility(item.IsVisible());

        static void OnToggleEvent(object sender, RoutedEventArgs e)
        {
            var self = (AppBarToggleButton)sender;
            var item = (ICommandToggleButton)self.Tag;

            item.OnToggle(self.IsChecked ?? false, new CommandUIContext(sender, e));
        }

        toggleButton.Checked += OnToggleEvent;
        toggleButton.Unchecked += OnToggleEvent;

        return toggleButton;
    }

    private AppBarElementContainer CreateElementContainer(ICommandObject item, CommandItemMetadataAttribute metadata)
    {
        var elementContainer = new AppBarElementContainer
        {
            Tag = item,
            Content = item.UIObject,
            IsEnabled = item.IsEnabled(),
            Visibility = Bool2Visibility(item.IsVisible()),
            VerticalContentAlignment = VerticalAlignment.Center,
        };

        return elementContainer;
    }

    private Visibility Bool2Visibility(bool value)
    {
        return value ? Visibility.Visible : Visibility.Collapsed;
    }

    private string GetLocalizedOrDefault(string resourceKey)
    {
        if (resourceKey.StartsWith("resx:"))
        {
            return _mrtCoreService.MainResourceMap.GetString(resourceKey[5..]);
        }
        else
        {
            return resourceKey;
        }
    }

    public void RefreshUI()
    {
        void Refresh(IObservableVector<ICommandBarElement> collection)
        {
            foreach (var item in collection)
            {
                if (item is Control control && control.Tag is ICommandItem commandItem)
                {
                    control.IsEnabled = commandItem.IsVisible();
                    control.Visibility = Bool2Visibility(commandItem.IsVisible());
                }
            }
        }

        Refresh(_commandBar.PrimaryCommands);
        Refresh(_commandBar.SecondaryCommands);
    }
}
