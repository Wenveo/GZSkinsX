// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.ComponentModel;

using GZSkinsX.Api.Commands;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.Commands.Impl;

internal sealed class CommandObjectImpl
{
    private readonly ICommandObject _commandObject;
    private readonly AppBarElementContainer _appBarElementContainer;

    public AppBarElementContainer UIObject => _appBarElementContainer;

    public CommandObjectImpl(ICommandObject commandObject)
    {
        _commandObject = commandObject;

        _appBarElementContainer = new AppBarElementContainer
        {
            VerticalContentAlignment = VerticalAlignment.Center
        };

        _appBarElementContainer.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _appBarElementContainer.Loaded -= OnLoaded;
        _commandObject.OnInitialize();

        var notifyPropertyChanged = _commandObject as INotifyPropertyChanged;
        if (notifyPropertyChanged is not null)
        {
            notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
        }

        _appBarElementContainer.Content = _commandObject.UIObject;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ICommandObject.UIObject):
                _appBarElementContainer.Content = _commandObject.UIObject;
                break;
            default:
                break;
        }
    }
}
